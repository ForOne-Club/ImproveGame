using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.ModSystems;
using ImproveGame.Components;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Content.Items.ItemContainer;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.Items;
using ImproveGame.UI.ExtremeStorage;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.IndependentModules.InfiniteBuff;

/// <summary>
/// 无限 Buff 的玩家侧核心系统。
/// </summary>
/// <remarks>
/// 主流程分两段：
/// <br/>1) 每帧从已缓存物品应用 Buff；
/// <br/>2) 每隔 <see cref="SetupBuffListCooldownTime"/> 帧重建一次可用物品缓存。
/// </remarks>
public class InfiniteBuffPlayer : ModPlayer
{
    private readonly TickTimer _rebuild;

    public InfiniteBuffPlayer()
    {
        _rebuild = new TickTimer(1d, RebuildAvailableItems);
    }

    /// <summary>
    /// 无限 Buff 黑名单：记录“本可无限但被用户禁用”的 Buff。
    /// </summary>
    public BuffKeySet Blacklist { get; } = new();

    /// <summary>
    /// 收藏（星标）Buff 集合，仅用于 UI 偏好展示。
    /// </summary>
    public BuffKeySet Favorites { get; } = new();

    /// <summary>
    /// 玩家自身来源的可用无限 Buff 物品。
    /// </summary>
    public List<Item> PlayerAvailableItems { get; } = [];

    /// <summary>
    /// 储存系统来源（TE）的可用无限 Buff 物品。
    /// </summary>
    public List<Item> ExStorageAvailableItems { get; } = [];

    /// <summary>
    /// 全部来源的可用物品集合（玩家 + 储存系统）。
    /// </summary>
    /// <remarks>
    /// 用于快速判定某个 <see cref="Item"/> 是否属于“无限 Buff 可用项”。
    /// 另见 <see cref="HandleClonedItem"/>。
    /// </remarks>
    public HashSet<Item> AvailableItems { get; private set; } = [];

    /// <summary>
    /// 幸运药水带来的本帧幸运增量。
    /// </summary>
    /// <remarks>
    /// 在 <see cref="ModifyLuck"/> 消费后会清零。
    /// </remarks>
    public float LuckPotionBoost;

    /// <summary>
    /// 临时放行集合：本帧允许保留的黑名单 Buff。
    /// </summary>
    /// <remarks>
    /// 初值复制自 <see cref="Blacklist"/>，
    /// 之后按“当前可用 Buff”移除条目，用于 AddBuff/删 Buff 的放行判定。
    /// </remarks>
    private static BuffKeySet ClearBuffBan { get; } = new();

    #region 杂项

    /// <summary>
    /// 尝试获取玩家对应的 <see cref="InfiniteBuffPlayer"/>。
    /// </summary>
    public static bool TryGet(Player player, out InfiniteBuffPlayer infinitePlayer) => player.TryGetModPlayer(out infinitePlayer);

    /// <summary>
    /// 注册 AddBuff Hook，用于拦截黑名单 Buff 的添加。
    /// </summary>
    public override void Load() => On_Player.AddBuff += BanBuffs;

    private void BanBuffs(On_Player.orig_AddBuff orig,
        Player player, int type, int timeToAdd, bool quiet, bool foodHack)
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

    /// <summary>
    /// 玩家进世界时立即建立一次可用物品缓存。
    /// </summary>
    public override void OnEnterWorld()
    {
        RebuildAvailableItems();
    }

    /// <summary>
    /// 注入本帧累计的幸运值加成（来自无限幸运药水），并重置缓存值。
    /// </summary>
    public override void ModifyLuck(ref float luck)
    {
        luck += LuckPotionBoost;
        LuckPotionBoost = 0;
    }

    #endregion

    /// <summary>
    /// 无限 Buff 的每帧执行入口。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="HideBuffSystem.PostDrawInterface"/> 的数据来源顺序保持一致：
    /// 本地玩家 -> 队友共享（可选）-> 储存系统。
    /// </remarks>
    public override void PostUpdateBuffs()
    {

        var player = Player;
        if (player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server) return;

        // 先重置“随身增益站”状态，再由本帧命中的 Buff 回填。
        ApplyBuffStation.Reset();

        // 复制黑名单到“临时放行集合”；后续会按当前可用 Buff 逐步移除。
        ClearBuffBan.CopyFrom(Blacklist);

        // 1) 应用本地玩家来源。
        ApplyAvailableBuffs(PlayerAvailableItems);

        if (Config.ShareInfBuffs)
        {
            // 2) 应用同队共享来源（含距离规则，见 CheckTeamPlayers）。
            ForEachTeammate(player.whoAmI, (teammate) =>
            {
                ApplyAvailableBuffs(teammate.GetModPlayer<InfiniteBuffPlayer>().PlayerAvailableItems);
            }, requireAlive: false);
        }

        // 3) 应用储存系统来源（TE）。
        ApplyAvailableBuffs(ExStorageAvailableItems);

        _rebuild?.Tick(Main.gameTimeCache);

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

        // 定时重建可用物品缓存，避免每帧全量扫描背包/储存。
    }

    /// <summary>
    /// 根据给定物品集合应用无限 Buff，并处理冲突与放行集合。
    /// </summary>
    /// <remarks>
    /// 该方法会：
    /// <br/>1) 汇总可提供的 BuffType；
    /// <br/>2) 应用可启用 Buff；
    /// <br/>3) 回填随身增益站标记；
    /// <br/>4) 清理冲突 Buff；
    /// <br/>5) 更新 <see cref="ClearBuffBan"/>。
    /// </remarks>
    private static void ApplyAvailableBuffs(IEnumerable<Item> items)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return;

        HashSet<int> buffTypes = [];
        foreach (Item item in items)
        {
            // 侏儒不通过 AddBuff 生效，这里单独打标记。
            if (item.createTile is TileID.GardenGnome)
                ApplyBuffStation.HasGardenGnome = true;

            ApplyBuffItem.GetItemBuffType(item).ForEach(buffType => buffTypes.Add(buffType));

            // 幸运药水取更高档位，统一在 ModifyLuck 中结算。
            infinitePlayer.LuckPotionBoost = item.type switch
            {
                ItemID.LuckPotion => Math.Max(infinitePlayer.LuckPotionBoost, 0.1f),
                ItemID.LuckPotionGreater => Math.Max(infinitePlayer.LuckPotionBoost, 0.2f),
                _ => infinitePlayer.LuckPotionBoost
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
            if (!buffTypes.Contains(buffType) || InBlacklist(buffType) || clearBuffTypes.Contains(buffType)) continue;
            value.ForEach(i => clearBuffTypes.Add(i));
        }

        // 不直接从候选集中删除，统一在应用阶段跳过，并在最后做二次清理。

        #endregion

        foreach (var buffType in buffTypes.Where(type => !InBlacklist(type)))
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

            // 未开启“随身增益站”时不回填场景标记。
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

        // 二次清理：移除可能在本帧前半段已存在的冲突 Buff。
        foreach (var buffType in clearBuffTypes)
        {
            Main.LocalPlayer.ClearBuff(buffType);
        }

        // 从临时放行集合中移除当前已可用的 Buff，避免其被 Ban 逻辑误拦截。
        ClearBuffBan.Ids?.RemoveWhere(buffTypes.Contains);
    }

    /// <summary>
    /// 遍历并对与指定玩家同队的其他玩家执行 <paramref name="teammateAction"/>。
    /// </summary>
    /// <param name="whoAmI">自身玩家在 <see cref="Main.player"/> 中的索引。</param>
    /// <param name="teammateAction">对每个满足条件的队友执行的操作。</param>
    /// <param name="requireAlive">
    /// 是否跳过死亡或幽灵状态的玩家；为 <c>true</c> 时会忽略 <see cref="Player.DeadOrGhost"/> 的玩家。
    /// </param>
    /// <remarks>
    /// 仅在多人模式下生效；单人模式会直接返回。
    /// 仅处理：已激活、与自身同队且队伍不为 0 的玩家（不包含自身）。
    /// 当 <c>Config.ShareRange != -1</c> 时，还会要求队友与自身的距离（以格为单位）不超过该范围。
    /// </remarks>
    public static void ForEachTeammate(int whoAmI, Action<Player> teammateAction, bool requireAlive = true)
    {
        var players = Main.player.AsSpan();
        if (whoAmI < 0 || whoAmI >= players.Length ||
            Main.netMode is NetmodeID.SinglePlayer) return;

        var myself = players[whoAmI];

        for (int i = 0; i < players.Length; i++)
        {
            if (i == whoAmI) continue;

            var player = players[i];
            if (!player.active) continue;
            if (requireAlive && player.DeadOrGhost) continue;
            if (player.team == 0 || player.team != myself.team) continue;

            if (Config.ShareRange != -1)
            {
                // 分享距离
                if (player.Distance(myself.Center) / 16f > Config.ShareRange) continue;
            }

            teammateAction(player);
        }
    }

    /// <summary>
    /// 重建可用物品缓存（玩家来源 + 储存来源）。
    /// </summary>
    /// <remarks>
    /// 该方法是较重路径，因此由 <see cref="PostUpdateBuffs"/> 按间隔触发。
    /// </remarks>
    private void RebuildAvailableItems()
    {
        var oldAvailableItems = new List<Item>(PlayerAvailableItems);

        // 1) 统计玩家自身来源。
        PlayerAvailableItems.Clear();
        CollectAvailableItems(PlayerAvailableItems, GetAllInventoryItemsList(Main.LocalPlayer));

        // 仅在列表发生变化时同步，避免无效网络包。
        if (!oldAvailableItems.SequenceEqual(PlayerAvailableItems))
        {
            InfiniteBuffItemPacket.GetInstance(Player.whoAmI, PlayerAvailableItems).Send();
        }

        // 2) 统计储存系统来源（仅启用了无限 Buff 的储存）。
        ExStorageAvailableItems.Clear();
        foreach ((var _, var tileEntity) in TileEntity.ByID)
        {
            if (tileEntity is not TEExtremeStorage { UseUnlimitedBuffs: true } storage) continue;

            var alchemyItems = storage.FindAllNearbyChestsWithGroup(ItemGroup.Alchemy);
            foreach (var alchemyItem in alchemyItems)
            {
                CollectAvailableItems(ExStorageAvailableItems, Main.chest[alchemyItem].item);
            }
        }

        // 3) 合并为快速判定集合。
        AvailableItems = [.. PlayerAvailableItems, .. ExStorageAvailableItems];
    }

    /// <summary>
    /// 从一组物品中收集可用于“无限 Buff”的物品。
    /// </summary>
    /// <remarks>
    /// 会展开 <see cref="PotionBag"/> 内部物品，并对每个物品调用
    /// <see cref="CollectAvailableItemsFromItem"/> 进行判定与收集。
    /// </remarks>
    private static List<Item> CollectAvailableItems(List<Item> availableItems, IEnumerable<Item> targetItems)
    {
        if (targetItems is null) return availableItems;

        var rpActivated = false;

        foreach (var item in targetItems)
        {
            if (item is null || item.IsAir) continue;

            CollectAvailableItemsFromItem(item, availableItems, ref rpActivated);

            if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.ItemContainer.Count > 0)
            {
                foreach (var p in potionBag.ItemContainer)
                {
                    CollectAvailableItemsFromItem(p, availableItems, ref rpActivated);
                }
            }
        }

        return availableItems;
    }

    /// <summary>
    /// 判断单个物品是否可用于“无限 Buff”，并将其（或其等效项）加入列表。
    /// </summary>
    /// <param name="item">待判定物品。</param>
    /// <param name="availableItems">收集结果。</param>
    /// <param name="rpActivated">红药扩展是否已触发（仅触发一次）。</param>
    /// <remarks>
    /// 规则：
    /// <list type="bullet">
    /// <item><description>普通 Buff 物品（含花园侏儒）直接加入。</description></item>
    /// <item><description>命中红药扩展时，额外加入一组等效药水占位项，并标记为已触发。</description></item>
    /// </list>
    /// </remarks>
    private static void CollectAvailableItemsFromItem(Item item, List<Item> availableItems, ref bool rpActivated)
    {
        // 常规可提供 Buff 的物品（含花园侏儒）。
        if (ApplyBuffItem.GetItemBuffType(item).Count > 0 || item.createTile is TileID.GardenGnome)
        {
            availableItems.Add(item);
        }

        // 红药扩展：把扩展效果拆解为多种标准药水以复用后续逻辑（仅一次）。
        if (rpActivated || !item.CanActivateRedPotionExtension()) return;

        rpActivated = true;

        availableItems.Add(item);
        availableItems.Add(new Item(ItemID.ObsidianSkinPotion, 9999));
        availableItems.Add(new Item(ItemID.RegenerationPotion, 9999));
        availableItems.Add(new Item(ItemID.SwiftnessPotion, 9999));
        availableItems.Add(new Item(ItemID.IronskinPotion, 9999));
        availableItems.Add(new Item(ItemID.ManaRegenerationPotion, 9999));
        availableItems.Add(new Item(ItemID.MagicPowerPotion, 9999));
        availableItems.Add(new Item(ItemID.FeatherfallPotion, 9999));
        availableItems.Add(new Item(ItemID.SpelunkerPotion, 9999));
        availableItems.Add(new Item(ItemID.ArcheryPotion, 9999));
        availableItems.Add(new Item(ItemID.HeartreachPotion, 9999));
        availableItems.Add(new Item(ItemID.HunterPotion, 9999));
        availableItems.Add(new Item(ItemID.EndurancePotion, 9999));
        availableItems.Add(new Item(ItemID.LifeforcePotion, 9999));
        availableItems.Add(new Item(ItemID.InfernoPotion, 9999));
        availableItems.Add(new Item(ItemID.MiningPotion, 9999));
        availableItems.Add(new Item(ItemID.RagePotion, 9999));
        availableItems.Add(new Item(ItemID.WrathPotion, 9999));
        availableItems.Add(new Item(ItemID.TrapsightPotion, 9999));
    }

    /// <summary>
    /// 新玩家加入时同步当前无限 Buff 物品状态。
    /// </summary>
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        InfiniteBuffItemPacket.GetInstance(Player.whoAmI, PlayerAvailableItems).Send(toWho, fromWho);
    }

    /// <summary>
    /// 本地玩家每帧预处理：清理黑名单 Buff。
    /// </summary>
    public override void PreUpdateBuffs()
    {
        var player = Player;
        if (player.whoAmI != Main.myPlayer) return;

        // 清理
        var maxQuantity = Player.MaxBuffs;
        for (int i = 0; i < maxQuantity; i++)
        {
            var typeInSlot = player.buffType[i];
            if (typeInSlot <= 0) continue;

            if (Blacklist.ContainsByType(typeInSlot) &&
                !ClearBuffBan.ContainsByType(typeInSlot))
            {
                player.DelBuff(i--);
            }
        }
    }

    /// <summary>
    /// 判断某个 Buff 在当前本地配置下是否允许作为 “无限 Buff” 生效。
    /// </summary>
    /// <param name="buffType">要检查的 BuffType。</param>
    /// <returns>在本地玩家的黑名单中时返回 <see langword="true"/>。</returns>
    /// <remarks>
    /// 由于多人共享选项逻辑，这里统一以 <see cref="Main.LocalPlayer"/> 的配置为准。
    /// </remarks>
    public static bool InBlacklist(int buffType)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer))
            throw new Exception($"{nameof(InfiniteBuffPlayer)} is not found.");

        return infinitePlayer.Blacklist.ContainsByType(buffType);
    }

    /// <summary>
    /// 读取玩家的黑名单与收藏数据。
    /// </summary>
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

    /// <summary>
    /// 保存玩家的黑名单与收藏数据。
    /// </summary>
    public override void SaveData(TagCompound tag)
    {
        tag[nameof(Blacklist)] = Blacklist.GetData();
        tag[nameof(Favorites)] = Favorites.GetData();
    }
}
