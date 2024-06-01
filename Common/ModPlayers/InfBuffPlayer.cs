using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Content.Items.ItemContainer;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.Items;
using ImproveGame.UI.ExtremeStorage;
using Terraria.DataStructures;

namespace ImproveGame.Common.ModPlayers;

/// <summary>
/// 22/11/9 无尽Buff流程<br/>
/// 1) 每隔 <see cref="SetupBuffListCooldownTime"/> 会更新一次Buff列表 <see cref="AvailableItems"/><br/>
/// 2) 每帧遍历 <see cref="AvailableItems"/> 对于所有 <see cref="CheckInfBuffEnable"/> 为 <see langword="true"/> 的Buff实现效果<br/>
/// 节省性能
/// </summary>
public class InfBuffPlayer : ModPlayer
{
    /// <summary>
    /// 用于记录玩家总获取的无尽Buff物品
    /// </summary>
    internal List<Item> AvailableItems = new();

    /// <summary>
    /// 储存管理器里面的无尽Buff物品
    /// </summary>
    internal HashSet<Item> ExStorageAvailableItems = new();

    /// <summary>
    /// 储存管理器+玩家储存里面的无尽Buff物品 <br/>
    /// 另见 <see cref="HandleClonedItem"/>
    /// </summary>
    internal HashSet<Item> AvailableItemsHash = new();

    /// <summary>
    /// 每隔多久统计一次Buff
    /// </summary>
    public static int SetupBuffListCooldown;

    public const int SetupBuffListCooldownTime = 120;

    /// <summary>
    /// 幸运药水特判
    /// </summary>
    public float LuckPotionBoost;

    #region 杂项

    public static InfBuffPlayer Get(Player player) => player.GetModPlayer<InfBuffPlayer>();
    public static bool TryGet(Player player, out InfBuffPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);

    public override void Load()
    {
        On_Player.AddBuff += BanBuffs;
    }

    public override void OnEnterWorld()
    {
        SetupItemsList();
    }

    public override void ModifyLuck(ref float luck)
    {
        luck += LuckPotionBoost;
        LuckPotionBoost = 0;
    }

    #endregion

    /// <summary>
    /// 与 <see cref="HideBuffSystem.PostDrawInterface"/> 相关，要更改的时候记得改那边
    /// </summary>
    public override void PostUpdateBuffs()
    {
        if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
            return;
        
        // 重设部分Buff站效果
        ApplyBuffStation.Reset();

        // 从玩家身上获取所有的无尽Buff物品
        ApplyAvailableBuffsFromPlayer(Player);
        if (Config.ShareInfBuffs)
            CheckTeamPlayers(Player.whoAmI, ApplyAvailableBuffsFromPlayer, checkDead: false);

        // 从TE中获取所有的无尽Buff物品
        ApplyAvailableBuffs(Get(Player).ExStorageAvailableItems);

        // 每隔一段时间更新一次Buff列表
        SetupBuffListCooldown++;
        if (SetupBuffListCooldown % SetupBuffListCooldownTime != 0)
            return;

        SetupItemsList();
    }

    /// <summary>
    /// 获取物品列表源玩家的Buff列表并应用
    /// </summary>
    /// <param name="storageSource">Buff物品列表源，用于获取物品列表，实际应用固定到LocalPlayer上</param>
    private static void ApplyAvailableBuffsFromPlayer(Player storageSource) =>
        ApplyAvailableBuffs(Get(storageSource).AvailableItems);

    /// <summary>
    /// 应用可用的Buff物品
    /// </summary>
    private static void ApplyAvailableBuffs(IEnumerable<Item> items)
    {
        foreach (Item item in items)
        {
            // 侏儒特判
            if (item.createTile is TileID.GardenGnome)
                ApplyBuffStation.HasGardenGnome = true;

            var buffTypes = ApplyBuffItem.GetItemBuffType(item);

            buffTypes.ForEach(buffType =>
            {
                // 饱食三级Buff不应该覆盖，而是取最高级
                bool wellFed3Enabled = Main.LocalPlayer.FindBuffIndex(BuffID.WellFed3) != -1;
                bool wellFed2Enabled = Main.LocalPlayer.FindBuffIndex(BuffID.WellFed2) != -1;

                if (!CheckInfBuffEnable(buffType))
                    return;

                // Buff
                switch (buffType)
                {
                    case BuffID.WellFed when wellFed2Enabled || wellFed3Enabled:
                    case BuffID.WellFed2 when wellFed3Enabled:
                        return;
                    case -1:
                        break;
                    default:
                        Main.LocalPlayer.AddBuff(buffType, 30);

                        // 本地玩家的对应ModPlayer
                        var modPlayer = Get(Main.LocalPlayer);
                        // 幸运药水
                        modPlayer.LuckPotionBoost = item.type switch
                        {
                            ItemID.LuckPotion => Math.Max(modPlayer.LuckPotionBoost, 0.1f),
                            ItemID.LuckPotionGreater => Math.Max(modPlayer.LuckPotionBoost, 0.2f),
                            _ => modPlayer.LuckPotionBoost
                        };
                        break;
                }

                // Buff站效果设置
                if (!Config.NoPlace_BUFFTile)
                    return;

                switch (buffType)
                {
                    case BuffID.Campfire:
                        ApplyBuffStation.HasCampfire = true;
                        break;
                    case BuffID.HeartLamp:
                        ApplyBuffStation.HasHeartLantern = true;
                        break;
                    case BuffID.StarInBottle:
                        ApplyBuffStation.HasStarInBottle = true;
                        break;
                    case BuffID.Sunflower:
                        ApplyBuffStation.HasSunflower = true;
                        break;
                    case BuffID.WaterCandle:
                        ApplyBuffStation.HasWaterCandle = true;
                        break;
                    case BuffID.PeaceCandle:
                        ApplyBuffStation.HasPeaceCandle = true;
                        break;
                    case BuffID.ShadowCandle:
                        ApplyBuffStation.HasShadowCandle = true;
                        break;
                }
            });
        }
    }

    /// <summary>
    /// 设置物品列表
    /// </summary>
    private void SetupItemsList()
    {
        var oldAvailableItems = new List<Item>(AvailableItems);

        AvailableItems = new List<Item>();
        ExStorageAvailableItems = new HashSet<Item>();

        // 玩家身上的无尽Buff
        var items = GetAllInventoryItemsList(Main.LocalPlayer);
        AvailableItems = GetAvailableItemsFromItems(items);

        // 只有不同才发包
        if (!oldAvailableItems.SequenceEqual(AvailableItems))
        {
            InfBuffItemPacket.Get(this).Send();
        }

        // 从TE中获取所有的无尽Buff物品
        foreach ((int _, TileEntity tileEntity) in TileEntity.ByID)
        {
            if (tileEntity is not TEExtremeStorage {UseUnlimitedBuffs: true} storage)
            {
                continue;
            }

            var alchemyItems = storage.FindAllNearbyChestsWithGroup(ItemGroup.Alchemy);
            alchemyItems.ForEach(i => GetAvailableItemsFromItems(Main.chest[i].item).ForEach(j => ExStorageAvailableItems.Add(j)) );
        }

        AvailableItemsHash = AvailableItems.Concat(ExStorageAvailableItems).ToHashSet();
    }

    public static List<Item> GetAvailableItemsFromItems(IEnumerable<Item> items)
    {
        var availableItems = new List<Item>();
        if (items is null) return availableItems;

        foreach (var item in items)
        {
            if (item is null) continue;

            HandleBuffItem(item, availableItems);
            if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.ItemContainer.Count > 0)
            {
                foreach (var p in potionBag.ItemContainer)
                {
                    HandleBuffItem(p, availableItems);
                }
            }
        }

        return availableItems;
    }

    public static void HandleBuffItem(Item item, List<Item> availableItems)
    {
        // 增益物品
        var buffTypes = ApplyBuffItem.GetItemBuffType(item);
        if (buffTypes.Count > 0 || item.createTile is TileID.GardenGnome)
        {
            availableItems.Add(item);
        }

        if (item.IsAvailableRedPotionExtension())
        {
            void AddPotion(int type) => availableItems.Add(new Item(type, 9999));
            AddPotion(ItemID.ObsidianSkinPotion);
            AddPotion(ItemID.RegenerationPotion);
            AddPotion(ItemID.SwiftnessPotion);
            AddPotion(ItemID.IronskinPotion);
            AddPotion(ItemID.ManaRegenerationPotion);
            AddPotion(ItemID.MagicPowerPotion);
            AddPotion(ItemID.FeatherfallPotion);
            AddPotion(ItemID.SpelunkerPotion);
            AddPotion(ItemID.ArcheryPotion);
            AddPotion(ItemID.HeartreachPotion);
            AddPotion(ItemID.HunterPotion);
            AddPotion(ItemID.EndurancePotion);
            AddPotion(ItemID.LifeforcePotion);
            AddPotion(ItemID.InfernoPotion);
            AddPotion(ItemID.MiningPotion);
            AddPotion(ItemID.RagePotion);
            AddPotion(ItemID.WrathPotion);
            AddPotion(ItemID.TrapsightPotion);
            availableItems.Add(item);
        }
    }

    // 新加入时的同步
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        // 按照Example的写法 - 直接写就完了！
        InfBuffItemPacket.Get(this).Send(toWho, fromWho);
    }

    private void BanBuffs(On_Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet,
        bool foodHack)
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
                        if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) &&
                            Player.buffType[i] == modBuff.Type)
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
        {
            // 原版
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
        {
            // 原版
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