using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Content.Items.ItemContainer;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.Items;
using ImproveGame.UI.ExtremeStorage;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ModPlayers;

/// <summary>
/// 22/11/9 无尽Buff流程<br/>
/// 1) 每隔 <see cref="SetupBuffListCooldownTime"/> 会更新一次Buff列表 <see cref="AvailableItems"/><br/>
/// 2) 每帧遍历 <see cref="AvailableItems"/> 对于所有 <see cref="CheckInfiniteBuffEnable"/> 为 <see langword="true"/> 的Buff实现效果<br/>
/// 节省性能
/// </summary>
public class InfBuffPlayer : ModPlayer
{
    public const int SetupBuffListCooldownTime = 120;

    /// <summary>
    /// 用于记录玩家总获取的无尽Buff物品
    /// </summary>
    internal List<Item> AvailableItems = [];

    /// <summary>
    /// 储存管理器里面的无尽Buff物品
    /// </summary>
    internal HashSet<Item> ExStorageAvailableItems = [];

    /// <summary>
    /// 储存管理器+玩家储存里面的无尽Buff物品 <br/>
    /// 另见 <see cref="HandleClonedItem"/>
    /// </summary>
    internal HashSet<Item> AvailableItemsHash = [];

    /// <summary>
    /// 每隔多久统计一次Buff
    /// </summary>
    public static int SetupBuffListCooldown { get; private set; }

    /// <summary>
    /// 幸运药水特判
    /// </summary>
    public float LuckPotionBoost;

    public readonly static BuffKeySet ClearBuffBan = new();

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

        // 赋值，用于下面判断玩家不可开关的被Ban无限Buff
        DataPlayer.TryGet(Main.LocalPlayer, out var dataPlayer);
        if (dataPlayer != null)
        {
            ClearBuffBan.Ids.Clear();
            ClearBuffBan.Ids.UnionWith(Blacklist.Ids);
            ClearBuffBan.FullNames.Clear();
            ClearBuffBan.FullNames.UnionWith(Blacklist.FullNames);
        }

        // 从玩家身上获取所有的无尽Buff物品
        ApplyAvailableBuffsFromPlayer(Player);
        if (Config.ShareInfBuffs)
            CheckTeamPlayers(Player.whoAmI, ApplyAvailableBuffsFromPlayer, checkDead: false);

        // 从TE中获取所有的无尽Buff物品
        ApplyAvailableBuffs(Get(Player).ExStorageAvailableItems);

        // #region 清除冲突的Buff
        //
        // List<int> clearBuffTypes = [];
        // foreach ((int buffType, List<int> value) in ModIntegrationsSystem.ModdedBuffConflicts)
        // {
        //     if (!Main.LocalPlayer.HasBuff(buffType)) continue;
        //     clearBuffTypes.AddRange(value);
        // }
        //
        // clearBuffTypes.ForEach(Main.LocalPlayer.ClearBuff);
        //
        // #endregion

        // 清除冲突的Buff
        // foreach (int buffType in ModIntegrationsSystem.ModdedBuffConflicts.Keys)
        // {
        //     if (Main.LocalPlayer.HasBuff(buffType))
        //     {
        //         foreach (int clearedBuffType in ModIntegrationsSystem.ModdedBuffConflicts[buffType])
        //         {
        //             Main.LocalPlayer.ClearBuff(clearedBuffType);
        //         }
        //     }
        // }

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
        var infBuffPlayer = Get(Main.LocalPlayer);

        HashSet<int> buffTypes = [];
        foreach (Item item in items)
        {
            // 侏儒特判
            if (item.createTile is TileID.GardenGnome)
                ApplyBuffStation.HasGardenGnome = true;

            ApplyBuffItem.GetItemBuffType(item).ForEach(buffType => buffTypes.Add(buffType));

            // 幸运药水
            infBuffPlayer.LuckPotionBoost = item.type switch
            {
                ItemID.LuckPotion => Math.Max(infBuffPlayer.LuckPotionBoost, 0.1f),
                ItemID.LuckPotionGreater => Math.Max(infBuffPlayer.LuckPotionBoost, 0.2f),
                _ => infBuffPlayer.LuckPotionBoost
            };
        }

        HashSet<string> hashModBuffs =
        [
            ..buffTypes
                    .Select(BuffLoader.GetBuff)
                    .Where(modBuff => modBuff != null)
                    .Select(modBuff => $"{modBuff.Mod.Name}/{modBuff.Name}")
        ];

        ClearBuffBan.FullNames.RemoveWhere(hashModBuffs.Contains);

        #region 清除冲突 Buff

        HashSet<int> clearBuffTypes = [];
        foreach ((int buffType, List<int> value) in ModIntegrationsSystem.ModdedBuffConflicts)
        {
            if (!buffTypes.Contains(buffType) || !CheckInfiniteBuffEnable(buffType) || clearBuffTypes.Contains(buffType)) continue;
            value.ForEach(i => clearBuffTypes.Add(i));
        }

        // clearBuffTypes.ForEach(buffType => buffTypes.Remove(buffType));

        #endregion

        foreach (var buffType in buffTypes.Where(CheckInfiniteBuffEnable))
        {
            if (clearBuffTypes.Contains(buffType))
                continue;

            switch (buffType)
            {
                case -1:
                    break;
                default:
                    Main.LocalPlayer.AddBuff(buffType, 30);
                    break;
            }

            if (!Config.NoPlace_BUFFTile)
                continue;

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
        }

        // 二次清理冲突 Buff
        foreach (var buffType in clearBuffTypes)
        {
            Main.LocalPlayer.ClearBuff(buffType);
        }

        // 清除玩家可以开关的被Ban无限Buff
        ClearBuffBan.Ids?.RemoveWhere(buffTypes.Contains);
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
            if (tileEntity is not TEExtremeStorage { UseUnlimitedBuffs: true } storage)
            {
                continue;
            }

            var alchemyItems = storage.FindAllNearbyChestsWithGroup(ItemGroup.Alchemy);
            alchemyItems.ForEach(i =>
                GetAvailableItemsFromItems(Main.chest[i].item).ForEach(j => ExStorageAvailableItems.Add(j)));
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
        // 按照 Example 的写法 - 直接写就完了！
        InfBuffItemPacket.Get(this).Send(toWho, fromWho);
    }

    private void BanBuffs(On_Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet,
        bool foodHack)
    {
        if (Main.myPlayer == player.whoAmI && DataPlayer.TryGet(player, out var dataPlayer))
        {
            foreach (int buffType in Blacklist.Ids)
            {
                if (type == buffType && !ClearBuffBan.Ids.Contains(buffType))
                {
                    return;
                }
            }

            foreach (string buffFullName in Blacklist.FullNames)
            {
                string[] names = buffFullName.Split('/');
                string modName = names[0];
                string buffName = names[1];
                if (!ClearBuffBan.FullNames.Contains(buffFullName) &&
                    ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && type == modBuff.Type)
                {
                    return;
                }
            }
        }

        orig.Invoke(player, type, timeToAdd, quiet, foodHack);
    }

    public override void PreUpdateBuffs()
    {
        if (Main.myPlayer != Player.whoAmI) return;

        DeleteBuffs();
    }

    /// <summary>
    /// 在本地玩家每帧 Buff 更新前，移除命中“禁用无限 Buff”黑名单的 Buff。
    /// </summary>
    /// <remarks>
    /// 同时处理两类黑名单：原版 Buff 数值 ID 与 Mod Buff 全名（模组名/Buff 名）。
    /// 删除 Buff 后会导致槽位变化，因此每次删除后回退索引，避免漏检连续命中项。
    /// </remarks>
    public void DeleteBuffs()
    {
        // 逐槽位扫描当前 Buff；使用索引循环，便于删除后手动回退索引。
        for (int i = 0; i < Player.MaxBuffs; i++)
        {
            var type = Player.buffType[i];
            if (type <= 0) continue;

            // 原版 Buff：命中黑名单且不在临时放行集合时，立即删除。
            foreach (int buffType in Blacklist.Ids)
            {
                if (type == buffType && !ClearBuffBan.Ids.Contains(buffType))
                {
                    Player.DelBuff(i);
                    // 删除会改变后续槽位，回退一位以便在下一轮重新检查当前位置。
                    i--;
                }
            }

            // Mod Buff：按“模组名/Buff名”查找并匹配类型，且不在临时放行集合时删除。
            foreach (string buffFullName in Blacklist.FullNames)
            {
                var names = buffFullName.Split('/');
                var modName = names[0];
                var buffName = names[1];

                if (!ClearBuffBan.FullNames.Contains(buffFullName) &&
                    ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) &&
                    type == modBuff.Type)
                {
                    Player.DelBuff(i);
                    // 与原版 Buff 相同，删除后回退索引，避免跳过紧随其后的 Buff。
                    i--;
                }
            }
        }
    }

    /// <summary>
    /// 由于多人模式共享选项，这里原有的Player改成了Main.LocalPlayer，然后用了static
    /// </summary>
    public static bool CheckInfiniteBuffEnable(int buffType)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<InfBuffPlayer>(out var infinitePlayer)) throw new Exception("InfiniteBuffPlayer is not found.");

        var modBuff = BuffLoader.GetBuff(buffType);
        if (modBuff is null)
        {
            return !infinitePlayer.Blacklist.Ids.Contains(buffType);
        }
        else
        {
            var fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";

            return !infinitePlayer.Blacklist.FullNames.Contains(fullName);
        }
    }

    /// <summary>
    /// 切换指定 Buff 的无限效果开关：若目标已在黑名单中则移除，否则加入黑名单。
    /// </summary>
    /// <param name="buffType">要切换的 Buff 类型 ID。</param>
    public void ToggleInfiniteBuff(int buffType)
    {
        if (BuffLoader.GetBuff(buffType) is { } modBuff)
        {
            // Mod Buff 使用“模组名/Buff名”作为键，避免与原版或其他模组的数值 ID 冲突。
            var fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";

            if (!Blacklist.FullNames.Add(fullName))
            {
                Blacklist.FullNames.Remove(fullName);
            }
        }
        else
        {
            // 原版 Buff 直接以数值 ID 作为键进行开关切换。
            if (!Blacklist.Ids.Add(buffType))
            {
                Blacklist.Ids.Remove(buffType);
            }
        }
    }

    /// <summary>
    /// 无限 Buff 黑名单，记录哪些无限的但是仍然禁用的 Buffs
    /// </summary>
    public readonly BuffKeySet Blacklist = new();

    /// <summary>
    /// 星标列表
    /// </summary>
    public readonly BuffKeySet Favorites = new();

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<TagCompound>(nameof(Blacklist), out var blacklist))
        {
            Blacklist.LoadData(blacklist);
        }

        if (tag.TryGet<TagCompound>(nameof(Favorites), out var favorites))
        {
            Favorites.LoadData(favorites);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(Blacklist)] = Blacklist.GetData();
        tag[nameof(Favorites)] = Favorites.GetData();
    }
}

/// <summary>
/// Buff 键集合
/// </summary>
public class BuffKeySet
{
    /// <summary>
    /// Id，int 决定了是原版的
    /// </summary>
    public readonly HashSet<int> Ids = [];

    /// <summary>
    /// 全名，string 决定了是模组的
    /// </summary>
    public readonly HashSet<string> FullNames = [];

    public void LoadData(TagCompound tag)
    {
        if (tag.TryGet<int[]>(nameof(Ids), out var ids))
        {
            Ids.Clear();

            foreach (var id in ids)
            {
                Ids.Add(id);
            }
        }

        if (tag.TryGet<string[]>(nameof(FullNames), out var names))
        {
            FullNames.Clear();

            foreach (var name in names)
            {
                FullNames.Add(name);
            }
        }
    }

    public TagCompound GetData()
    {
        return new TagCompound
        {
            [nameof(Ids)] = Ids.ToArray(),
            [nameof(FullNames)] = FullNames.ToArray()
        };
    }

}
