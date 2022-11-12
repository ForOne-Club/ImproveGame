using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;

namespace ImproveGame.Common.Players
{
    /// <summary>
    /// 物品堆叠和 Buff
    /// </summary>
    public class StackAndBuff
    {
        public int stack;
        public int buffType;
    }

    /// <summary>
    /// 22/11/9 无尽Buff流程<br/>
    /// 1) 每隔 <see cref="SetupBuffListCooldownTime"/> 会更新一次Buff列表 <see cref="AvailableItems"/><br/>
    /// 2) 每帧遍历 <see cref="AvailableItems"/> 对于所有 <see cref="CheckInfBuffEnable"/> 为 <see langword="true"/> 的Buff实现效果<br/>
    /// 节省性能
    /// </summary>
    public class InfBuffPlayer : ModPlayer
    {
        /// <summary>
        /// 记录玩家所有收纳空间中每种药水的总量
        /// </summary>
        // public readonly Dictionary<int, StackAndBuff> PotionStackAndBuff = new();

        /// <summary>
        /// 用于记录玩家总获取的的无尽Buff物品
        /// </summary>
        internal List<Item> AvailableItems = new();

        /// <summary>
        /// 每隔多久统计一次Buff
        /// </summary>
        public static int SetupBuffListCooldown;
        public const int SetupBuffListCooldownTime = 30;

        /// <summary>
        /// 幸运药水特判
        /// </summary>
        public float LuckPotionBoost;

        #region 杂项

        public static InfBuffPlayer Get(Player player) => player.GetModPlayer<InfBuffPlayer>();
        public static bool TryGet(Player player, out InfBuffPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);

        public override void Load()
        {
            On.Terraria.Player.AddBuff += BanBuffs;
        }

        public override void OnEnterWorld(Player player)
        {
            SetupBuffList(player);
        }

        public override void ModifyLuck(ref float luck)
        {
            luck += LuckPotionBoost;
            LuckPotionBoost = 0;
        }

        #endregion

        public override void PostUpdateBuffs() {
            if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
                return;

            #region 应用增益

            foreach (var item in AvailableItems)
            {
                // 侏儒特判
                if (item.createTile is TileID.GardenGnome)
                    HideBuffSystem.HasGardenGnome = true;

                int buffType = ApplyBuffItem.GetItemBuffType(item);

                // 饱食三级Buff不应该覆盖，而是取最高级
                bool wellFed3Enabled = Main.LocalPlayer.FindBuffIndex(BuffID.WellFed3) != -1;
                bool wellFed2Enabled = Main.LocalPlayer.FindBuffIndex(BuffID.WellFed2) != -1;

                if (!CheckInfBuffEnable(buffType))
                    continue;

                // Buff
                switch (buffType)
                {
                    case BuffID.WellFed when wellFed2Enabled || wellFed3Enabled:
                    case BuffID.WellFed2 when wellFed3Enabled:
                        continue;
                    case -1:
                        break;
                    default:
                        Main.LocalPlayer.AddBuff(buffType, 2);

                        // 幸运药水
                        LuckPotionBoost = item.type switch
                        {
                            ItemID.LuckPotion => Math.Max(LuckPotionBoost, 0.1f),
                            ItemID.LuckPotionGreater => Math.Max(LuckPotionBoost, 0.2f),
                            _ => LuckPotionBoost
                        };
                        break;
                }

                // Buff站效果设置
                if (!Config.NoPlace_BUFFTile)
                    return;

                switch (buffType)
                {
                    case BuffID.Campfire:
                        HideBuffSystem.HasCampfire = true;
                        break;
                    case BuffID.HeartLamp:
                        HideBuffSystem.HasHeartLantern = true;
                        break;
                    case BuffID.StarInBottle:
                        HideBuffSystem.HasStarInBottle = true;
                        break;
                    case BuffID.Sunflower:
                        HideBuffSystem.HasSunflower = true;
                        break;
                }
            }

            #endregion

            SetupBuffListCooldown++;
            if (SetupBuffListCooldown % SetupBuffListCooldownTime != 0)
                return;

            SetupBuffList(Player);
            if (Config.ShareInfBuffs)
                CheckTeamPlayers(Player.whoAmI, SetupBuffList);
        }

        #region 设置增益列表

        /// <summary>
        /// 设立无尽Buff列表
        /// </summary>
        /// <param name="player">从某个玩家身上获取物品列表</param>
        public void SetupBuffList(Player player)
        {
            // Get(player) 的好像就是当前这个对象。
            Get(player).AvailableItems.Clear();
            // 药水的总数和对应增益
            // PotionStackAndBuff.Clear();

            var items = GetAllInventoryItemsList(player, false);
            foreach (var item in items)
            {
                HandleBuffItem(item);
                if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.storedPotions.Count > 0)
                {
                    foreach (var p in potionBag.storedPotions)
                    {
                        HandleBuffItem(p);
                    }
                }
            }
        }

        public void HandleBuffItem(Item item)
        {
            // 增益物品
            int buffType = ApplyBuffItem.GetItemBuffType(item);
            // StackAndBuff Code
            /*if (buffType != -1)
            {
                if (PotionStackAndBuff.ContainsKey(item.type))
                {
                    PotionStackAndBuff[item.type].stack += item.stack;
                }
                else
                    PotionStackAndBuff.Add(item.type, new() { stack = item.stack, buffType = buffType });
            }*/
            if (buffType is not -1 || item.createTile is TileID.GardenGnome)
            {
                AvailableItems.Add(item);
            }
        }

        #endregion

        private void BanBuffs(On.Terraria.Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (Main.myPlayer == player.whoAmI && DataPlayer.TryGet(player, out var dataPlayer))
            {
                if (dataPlayer.InfBuffDisabledVanilla is not null)
                {
                    foreach (int buffType in dataPlayer.InfBuffDisabledVanilla)
                    {
                        if (type == buffType)
                        {
                            return;
                        }
                    }
                }
                if (dataPlayer.InfBuffDisabledMod is not null)
                {
                    foreach (string buffFullName in dataPlayer.InfBuffDisabledMod)
                    {
                        string[] names = buffFullName.Split('/');
                        string modName = names[0];
                        string buffName = names[1];
                        if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && type == modBuff.Type)
                        {
                            return;
                        }
                    }
                }
            }
            orig.Invoke(player, type, timeToAdd, quiet, foodHack);
        }

        public override void PreUpdateBuffs()
        {
            if (Main.myPlayer != Player.whoAmI || !DataPlayer.TryGet(Player, out var dataPlayer))
                return;
            DeleteBuffs(dataPlayer);
        }

        public void DeleteBuffs(DataPlayer dataPlayer)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Player.buffType[i] > 0)
                {
                    if (dataPlayer.InfBuffDisabledVanilla is not null)
                    {
                        foreach (int buffType in dataPlayer.InfBuffDisabledVanilla)
                        {
                            if (Player.buffType[i] == buffType)
                            {
                                Player.DelBuff(i);
                                i--;
                            }
                        }
                    }
                    if (dataPlayer.InfBuffDisabledVanilla is not null)
                    {
                        foreach (string buffFullName in dataPlayer.InfBuffDisabledMod)
                        {
                            string[] names = buffFullName.Split('/');
                            string modName = names[0];
                            string buffName = names[1];
                            if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && Player.buffType[i] == modBuff.Type)
                            {
                                Player.DelBuff(i);
                                i--;
                            }
                        }
                    }
                }
            }
        }

        // 由于多人模式共享选项，这里原有的Player改成了Main.LocalPlayer，然后用了static
        public static bool CheckInfBuffEnable(int buffType)
        {
            DataPlayer dataPlayer = DataPlayer.Get(Main.LocalPlayer);
            ModBuff modBuff = BuffLoader.GetBuff(buffType);
            if (modBuff is null)
            { // 原版
                if (dataPlayer.InfBuffDisabledVanilla is null || !dataPlayer.InfBuffDisabledVanilla.Contains(buffType))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                string fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (dataPlayer.InfBuffDisabledMod is null || !dataPlayer.InfBuffDisabledMod.Contains(fullName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 开关无限Buff
        /// </summary>
        /// <param name="buffType">Buff的ID</param>
        public void ToggleInfBuff(int buffType)
        {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            ModBuff modBuff = BuffLoader.GetBuff(buffType);
            if (modBuff is null)
            { // 原版
                if (!dataPlayer.InfBuffDisabledVanilla.Contains(buffType))
                {
                    dataPlayer.InfBuffDisabledVanilla.Add(buffType);
                }
                else
                {
                    dataPlayer.InfBuffDisabledVanilla.Remove(buffType);
                }
            }
            else
            {
                string fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (!dataPlayer.InfBuffDisabledMod.Contains(fullName))
                {
                    dataPlayer.InfBuffDisabledMod.Add(fullName);
                }
                else
                {
                    dataPlayer.InfBuffDisabledMod.Remove(fullName);
                }
            }
        }
    }
}
