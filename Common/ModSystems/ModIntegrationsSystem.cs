using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions;
using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Content.Functions.HomeTeleporting;
using ImproveGame.Content.Items;
using ImproveGame.Content.Items.Globes;
using ImproveGame.Content.Items.IconDummies;
using ImproveGame.Content.Items.ItemContainer;
using ImproveGame.Content.Items.Placeable;
using ImproveGame.Packets;
using ImproveGame.UI.PlayerStats;
using System.Reflection;

namespace ImproveGame.Common.ModSystems;

public record FishingStat(int Power = 0, float SpeedMultiplier = 1f, bool TackleBox = false, bool LavaFishing = false);

/// <summary>
/// 处理几乎所有跨模组的集成
/// </summary>
public class ModIntegrationsSystem : ModSystem
{
    /// <summary>
    /// 储存独立添加支持的模组放置类Buff（也就是只需要1堆叠即可生效），由于这个是加载后执行的，直接存ID即可
    /// <br>Key为物品ID，Value为BuffID</br>
    /// </summary>
    internal static Dictionary<int, List<int>> ModdedPlaceableItemBuffs = new();

    /// <summary>
    /// 储存独立添加支持的模组药水类Buff（也就是需要30堆叠生效），由于这个是加载后执行的，直接存ID即可
    /// <br>Key为物品ID，Value为BuffID</br>
    /// </summary>
    internal static Dictionary<int, List<int>> ModdedPotionBuffs = new();

    /// <summary>
    /// A list of the items that don't apply infinite buffs
    /// </summary>
    internal static HashSet<int> ModdedInfBuffsIgnore = new();

    /// <summary>
    /// 储存即使在触发无限增益的情况下，也会正常消耗的物品列表
    /// </summary>
    internal static HashSet<int> ModdedInfBuffsConsume = new();

    /// <summary>
    /// 储存冲突Buff（当玩家拥有某个增益时，一个/些增益会被清除）列表
    /// <br>Key为BuffID，Value为会被清除的BuffIDs</br>
    /// </summary>
    internal static Dictionary<int, List<int>> ModdedBuffConflicts = new()
    {
        // 高等级的饱腹会覆盖低等级的
        {BuffID.WellFed3, [BuffID.WellFed2, BuffID.WellFed] },
        {BuffID.WellFed2, [BuffID.WellFed] }
    };

    /// <summary>
    /// 添加物品ID对应的一系列Tile
    /// <br>Key为物品ID，Value为一个TileID的列表</br>
    /// </summary>
    internal static Dictionary<int, List<int>> PortableStations = new();

    /// <summary>
    /// 添加物品ID对应的自动钓鱼属性
    /// <br>Key代表物品ID，Value是物品属性</br>
    /// </summary>
    internal static Dictionary<int, FishingStat> FishingStatLookup = new()
    {
        // 配饰为AnglerEarring可使钓鱼速度*200%
        // 配饰为AnglerTackleBag可使钓鱼速度*250%
        // 配饰为LavaproofTackleBag可使钓鱼速度*300%
        { ItemID.TackleBox, new FishingStat(TackleBox: true) }, // 钓具箱
        { ItemID.AnglerEarring, new FishingStat(10, 2f) }, // 渔夫耳环
        { ItemID.AnglerTackleBag, new FishingStat(10, 2.5f, TackleBox: true) }, // 渔夫渔具袋
        { ItemID.LavaFishingHook, new FishingStat(LavaFishing: true) }, // 防熔岩钓钩
        { ItemID.LavaproofTackleBag, new FishingStat(10, 3f, true, true) }, // 防熔岩渔具袋
        { ItemID.None, new FishingStat()}
    };

    internal static bool NoLakeSizePenaltyLoaded = false;
    internal static bool WMITFLoaded = false;
    internal static bool DialogueTweakLoaded = false;

    internal static int UnloadedItemType;

    public override void PostSetupContent()
    {
        DoCalamityModIntegration();
        DoThoriumModIntegration();
        DoFargowiltasIntegration();
        DoGensokyoIntegration();
        DoRecipeBrowserIntegration();
        DoDialogueTweakIntegration();
        DoModLoaderIntegration();
        DoShopLookupIntegration();
        NoLakeSizePenaltyLoaded = ModLoader.HasMod("NoLakeSizePenalty");
        WMITFLoaded = ModLoader.HasMod("WMITF");
    }

    private static void DoRecipeBrowserIntegration()
    {
        if (!ModLoader.TryGetMod("RecipeBrowser", out Mod recipeBrowser))
            return;

        // 给合成表的计量材料数量功能添加储存管理器和大背包支持
        var providers = recipeBrowser.Code.GetTypes()
            .Where(t => !t.IsAbstract && !t.ContainsGenericParameters)
            .Where(t => t.IsAssignableTo(typeof(UIElement)));

        var detourMethod = providers.First(i => i.Name is "UITrackIngredientSlot")
            .GetMethod("CountItemGroups", BindingFlags.Public | BindingFlags.Instance);
        var itemField = providers.First(i => i.Name is "UIItemSlot")
            .GetField("item", BindingFlags.Public | BindingFlags.Instance);

        if (detourMethod is null || itemField is null)
        {
            return;
        }

        MonoModHooks.Add(detourMethod, (Func<object, Player, Recipe, int, int, int> orig,
            object ingredientSlot, Player player, Recipe recipe, int type, int stopCountingAt) =>
        {
            int count = orig(ingredientSlot, player, recipe, type, stopCountingAt);
            var item = itemField.GetValue(ingredientSlot) as Item;

            if (type == 0 || item is null)
            {
                return count;
            }

            var items = new List<Item>();
            if (player.TryGetModPlayer<DataPlayer>(out var dataPlayer))
                items.AddRange(dataPlayer.AddMaterialsForCrafting(out _) ?? new List<Item>());
            if (player.TryGetModPlayer<StorageMaterialConsumer>(out var storagePlayer))
                items.AddRange(storagePlayer.AddMaterialsForCrafting(out _) ?? new List<Item>());

            foreach (var currentItem in from i in items where !i.IsAir select i)
            {
                if (recipe.AcceptedByItemGroups(currentItem.type, item.type))
                {
                    count += currentItem.stack;
                }
                else if (currentItem.type == type)
                {
                    count += currentItem.stack;
                }
            }

            return count >= stopCountingAt ? stopCountingAt : count;
        });
    }

    private static void DoCalamityModIntegration()
    {
        if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            return;

        AddBuffIntegration(calamityMod, "WeightlessCandle", true, "CirrusBlueCandleBuff");
        AddBuffIntegration(calamityMod, "VigorousCandle", true, "CirrusPinkCandleBuff");
        AddBuffIntegration(calamityMod, "SpitefulCandle", true, "CirrusYellowCandleBuff");
        AddBuffIntegration(calamityMod, "ResilientCandle", true, "CirrusPurpleCandleBuff");
        AddBuffIntegration(calamityMod, "ChaosCandle", true, "ChaosCandleBuff");
        AddBuffIntegration(calamityMod, "TranquilityCandle", true, "TranquilityCandleBuff");
        AddBuffIntegration(calamityMod, "EffigyOfDecay", true, "EffigyOfDecayBuff");
        AddBuffIntegration(calamityMod, "CrimsonEffigy", true, "CrimsonEffigyBuff");
        AddBuffIntegration(calamityMod, "CorruptionEffigy", true, "CorruptionEffigyBuff");
        AddFishingAccIntegration(calamityMod, "EnchantedPearl", 2f, 10, false, false);
        AddFishingAccIntegration(calamityMod, "AlluringBait", 2f, 30, false, false);
        AddFishingAccIntegration(calamityMod, "SupremeBaitTackleBoxFishingStation", 5f, 80, true, true);
        PlayerStatsSystem.CalamityIntegration(calamityMod);
    }

    private static void DoThoriumModIntegration()
    {
        if (!ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod))
            return;

        AddBuffIntegration(thoriumMod, "Altar", true, "AltarBuff");
        AddBuffIntegration(thoriumMod, "ConductorsStand", true, "ConductorsStandBuff");
        AddBuffIntegration(thoriumMod, "Mistletoe", true, "MistletoeBuff");
        AddBuffIntegration(thoriumMod, "NinjaRack", true, "NinjaBuff");
        AddHomeTpIntegration(thoriumMod, "WishingGlass", false, false);
        PlayerStatsSystem.ThoriumIntegration(thoriumMod);
    }

    private static void DoFargowiltasIntegration()
    {
        if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargowiltas))
            return;

        AddBuffIntegration(fargowiltas, "Omnistation", true, "Omnistation");
        AddBuffIntegration(fargowiltas, "Omnistation2", true, "Omnistation");
        
        if (!ModLoader.TryGetMod("FargowiltasSouls", out Mod fargowiltasSouls))
            return;
        
        AddFishingAccIntegration(fargowiltasSouls, "AnglerEnchant", 5f, 10, true, true);
        AddFishingAccIntegration(fargowiltasSouls, "TrawlerSoul", 5f, 60, true, true);
        AddFishingAccIntegration(fargowiltasSouls, "DimensionSoul", 5f, 60, true, true);
        AddFishingAccIntegration(fargowiltasSouls, "EternitySoul", 5f, 60, true, true);
    }

    private static void DoGensokyoIntegration()
    {
        if (!ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            return;


        AddBuffIntegration(gensokyo, "ButterflyPheromones", true, "Buff_ButterflyPheromones");
        AddBuffIntegration(gensokyo, "OniSake", true, "Buff_SakeBoth");
        AddBuffIntegration(gensokyo, "HoshigumaDish", true, "Debuff_SakeHoshiguma");
        AddBuffIntegration(gensokyo, "IbarakiBox", true, "Buff_SakeIbaraki");
        AddBuffIntegration(gensokyo, "EagleRaviProvisions", false, "Buff_DangoPower1", "Buff_DangoPower2",
            "Buff_DangoPower3", "Buff_DangoPower4", "Buff_DangoPower5");
        AddInfBuffsConsume(gensokyo, "JellyStone");
        AddBuffConflicts(gensokyo, "Buff_SakeBoth", BuffID.Tipsy, "Buff_SakeIbaraki", "Debuff_SakeHoshiguma");
        AddBuffConflicts(gensokyo, "Buff_SakeIbaraki", BuffID.Tipsy, "Debuff_SakeHoshiguma");
        AddBuffConflicts(gensokyo, "Debuff_SakeHoshiguma", BuffID.Tipsy);
        AddBuffConflicts(gensokyo, "Buff_DangoPower5", "Buff_DangoPower4", "Buff_DangoPower3", "Buff_DangoPower2",
            "Buff_DangoPower1");
        AddBuffConflicts(gensokyo, "Buff_DangoPower4", "Buff_DangoPower3", "Buff_DangoPower2", "Buff_DangoPower1");
        AddBuffConflicts(gensokyo, "Buff_DangoPower3", "Buff_DangoPower2", "Buff_DangoPower1");
        AddBuffConflicts(gensokyo, "Buff_DangoPower2", "Buff_DangoPower1");
    }

    private static void DoDialogueTweakIntegration()
    {
        if (!ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak))
            return;

        DialogueTweakLoaded = true;
        dialogueTweak.Call("AddButton",
            NPCID.TravellingMerchant, // NPC ID
            () => RefreshTravelShopSystem.DisplayText, // 文本
            "DialogueTweak/Interfaces/Assets/Icon_Help", // 显示的icon
            () => // 点击操作
            {
                if (Main.mouseLeft && !RefreshTravelShopSystem.OldMouseLeft)
                {
                    RefreshShopPacket.Get().Send(runLocally: true);
                }
            },
            () => Config.TravellingMerchantRefresh // 什么时候可用
        );
    }

    private static void DoModLoaderIntegration()
    {
        if (!ModLoader.TryGetMod("ModLoader", out Mod modloader))
            return;

        UnloadedItemType = modloader.Find<ModItem>("UnloadedItem").Type;
    }

    private static void AddCraftStationIntegration(Mod mod, string itemName, List<int> tileIDs)
    {
        PortableStations[mod.Find<ModItem>(itemName).Type] = tileIDs;
    }

    private static void AddBuffIntegration(Mod mod, string itemName, bool isPlaceable, params string[] buffNames)
    {
        List<int> buffs = [];
        foreach (string buffName in buffNames)
            buffs.Add(mod.Find<ModBuff>(buffName).Type);

        if (isPlaceable)
            ModdedPlaceableItemBuffs[mod.Find<ModItem>(itemName).Type] = buffs;
        else
            ModdedPotionBuffs[mod.Find<ModItem>(itemName).Type] = buffs;
    }

    private static void AddInfBuffsConsume(Mod mod, string itemName)
    {
        ModdedInfBuffsConsume.Add(mod.Find<ModItem>(itemName).Type);
    }

    /// <param name="BuffIDOrName">填int（原版）或string（Mod）</param>
    /// <param name="removeBuffIDOrName">填int（原版）或string（Mod）</param>
    public static void AddBuffConflicts(Mod mod, object BuffIDOrName, params object[] removeBuffIDOrName)
    {
        List<int> buffs = [];
        foreach (object obj in removeBuffIDOrName)
        {
            if (obj is int removeBuffID) buffs.Add(removeBuffID);
            if (obj is string removeBuffName) buffs.Add(mod.Find<ModBuff>(removeBuffName).Type);
        }

        if (BuffIDOrName is int buffID) ModdedBuffConflicts[buffID] = buffs;
        if (BuffIDOrName is string buffName) ModdedBuffConflicts[mod.Find<ModBuff>(buffName).Type] = buffs;
    }

    private static void AddHomeTpIntegration(Mod mod, string itemName, bool isPotion, bool isComebackItem)
    {
        // 属于是自我测试了
        Call("AddHomeTpItem", mod.Find<ModItem>(itemName).Type, isPotion, isComebackItem);
    }

    private static void AddFishingAccIntegration(Mod mod, string itemName, float speed, int power, bool tackleBox,
        bool lavaFishing)
    {
        FishingStatLookup[mod.Find<ModItem>(itemName).Type] = new FishingStat(power, speed, tackleBox, lavaFishing);
    }

    private static void DoShopLookupIntegration()
    {
        if (!ModLoader.TryGetMod("ShopLookup", out Mod mod))
            return;

        mod.Call(3, ImproveGame.Instance, "Wand", TextureAssets.Item[ModContent.ItemType<StarburstWand>()].Value,
            new NPCShop(-1, "Wand")
                .Add<CreateWand>(ConfigCondition.AvailableCreateWandC, gold: 10)
                .Add<MagickWand>(ConfigCondition.AvailableMagickWandC, gold: 10)
                .Add<PaintWand>(ConfigCondition.AvailablePaintWandC, gold: 5)
                .Add<MoveChest>(ConfigCondition.AvailableMoveChestC, gold: 15)
                .Add<WallPlace>(Item.buyPrice(gold: 8), Condition.DownedKingSlime, ConfigCondition.AvailableWallPlaceC)
                .Add<SpaceWand>(Item.buyPrice(gold: 12, silver: 50), Condition.DownedKingSlime,
                    ConfigCondition.AvailableSpaceWandC)
                .Add<LiquidWand>(Item.buyPrice(gold: 20), Condition.DownedEowOrBoc,
                    ConfigCondition.AvailableLiquidWandC)
                .Add<StarburstWand>(Item.buyPrice(gold: 50), Condition.Hardmode,
                    ConfigCondition.AvailableStarburstWandC)
                .Add<ConstructWand>(Item.buyPrice(gold: 30), Condition.Hardmode,
                    ConfigCondition.AvailableConstructWandC));
        mod.Call(3, ImproveGame.Instance, "Locator", TextureAssets.Item[ModContent.ItemType<AetherGlobe>()].Value,
            new NPCShop(-1, "Locator")
                .Add<FloatingIslandGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 8)
                .Add<PyramidGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 16)
                .Add<AetherGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 30)
                .Add<DungeonGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 1, silver: 80)
                .Add<GraniteCaveGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 1)
                .Add<MarbleCaveGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 1)
                .Add<EnchantedSwordGlobe>(ConfigCondition.EnableMinimapMarkC, gold: 8)
                .Add<PlanteraGlobe>(Item.buyPrice(gold: 16), Condition.Hardmode, ConfigCondition.EnableMinimapMarkC)
                .Add<TempleGlobe>(Item.buyPrice(gold: 20), Condition.DownedPlantera, ConfigCondition.EnableMinimapMarkC));
        mod.Call(3, ImproveGame.Instance, "Other", TextureAssets.Item[ModContent.ItemType<ExtremeStorage>()].Value,
            new NPCShop(-1, "Other")
                .Add<BannerChest>(ConfigCondition.AvailableBannerChestC, gold: 2, silver: 50)
                .Add<ExtremeStorage>(ConfigCondition.AvailableExtremeStorageC, gold: 12)
                .Add<Autofisher>(ConfigCondition.AvailableAutofisherC, gold: 4)
                .Add<DetectorDrone>(ConfigCondition.AvailableDetectorDroneC, gold: 2, silver: 20)
                .Add<StorageCommunicator>(Item.buyPrice(gold: 50), Condition.Hardmode,
                    ConfigCondition.AvailableExtremeStorageC)
                .Add<BaitSupplier>(ConfigCondition.AvailableBaitSupplierC, gold: 30)
                .Add<PotionBag>(ConfigCondition.AvailablePotionBagC, gold: 2, silver: 50)
                .Add<Dummy>(silver: 50)
                .Add<ShellShipInBottle>(ConfigCondition.EnableQuickShimmerC, gold: 5)
                .Add<WeatherBook>(Item.buyPrice(gold: 25, silver: 60), Condition.DownedEowOrBoc, ConfigCondition.EnableWeatherControlC));
    }

    public override void Unload()
    {
        ModdedPlaceableItemBuffs = null;
        ModdedPotionBuffs = null;
        NoLakeSizePenaltyLoaded = false;
    }

    public static object Call(params object[] args)
    {
        try
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
            }

            if (args.Length == 0)
            {
                throw new ArgumentException("Arguments cannot be empty!");
            }

            if (args[0] is string msg)
            {
                switch (msg)
                {
                    case "IgnoreInfItem":
                        {
                            List<int> ignores = AsListOfInt(args[1]); // Potion IDs
                            foreach (int ignore in ignores)
                            {
                                ModdedInfBuffsIgnore.Add(ignore);
                            }

                            return true;
                        }
                    case "AddPotion":
                        {
                            int itemType = Convert.ToInt32(args[1]); // Item ID
                            List<int> buffTypes = AsListOfInt(args[2]); // Buff IDs
                            ModdedPotionBuffs[itemType] = buffTypes;
                            return true;
                        }
                    case "ConsumePotion":
                        {
                            List<int> consumes = AsListOfInt(args[1]); // Potion IDs
                            foreach (int consume in consumes)
                            {
                                ModdedInfBuffsConsume.Add(consume);
                            }

                            return true;
                        }
                    case "BuffConflict":
                        {
                            int buffType = Convert.ToInt32(args[1]); // Buff ID
                            List<int> ClearedBuffTypes = AsListOfInt(args[2]); // Cleared Buff IDs
                            ModdedBuffConflicts[buffType] = ClearedBuffTypes;
                            return true;
                        }
                    case "AddStation":
                        {
                            int itemType = Convert.ToInt32(args[1]); // Item ID
                            List<int> buffTypes = AsListOfInt(args[2]); // Buff IDs
                            ModdedPlaceableItemBuffs[itemType] = buffTypes;
                            return true;
                        }
                    case "AddPortableCraftingStation":
                        {
                            int itemType = Convert.ToInt32(args[1]); // Item ID
                            List<int> tileIDs = AsListOfInt(args[2]); // Tile IDs
                            PortableStations[itemType] = tileIDs;
                            return true;
                        }
                    case "AddFishingAccessory":
                        {
                            int itemType = Convert.ToInt32(args[1]); // Item ID
                            float speed = Convert.ToSingle(args[2]); // Fishing Speed Bonus
                            int power = Convert.ToInt32(args[3]); // Fishing Power
                            bool tackleBox = Convert.ToBoolean(args[4]); // Tackle Box
                            bool lavaFishing = Convert.ToBoolean(args[5]); // Lava Fishing
                            FishingStatLookup[itemType] = new FishingStat(power, speed, tackleBox, lavaFishing);
                            return true;
                        }
                    // 添加属性类别
                    case "AddStatCategory":
                        {
                            string category = Convert.ToString(args[1]);
                            Texture2D texture = (Texture2D)args[2];
                            string nameKey = Convert.ToString(args[3]);
                            Texture2D modIcon = (Texture2D)args[4];

                            if (PlayerStatsSystem.Instance.StatsCategories.ContainsKey(category))
                            {
                                return false;
                            }

                            PlayerStatsSystem.Instance.StatsCategories.Add(category,
                                new BaseStatsCategory(texture, nameKey, true, modIcon));

                            return true;
                        }
                    // 添加属性到指定类别
                    case "AddStat":
                        {
                            string category = Convert.ToString(args[1]);
                            string nameKey = Convert.ToString(args[2]);
                            Func<string> value = (Func<string>)args[3];

                            if (PlayerStatsSystem.Instance.StatsCategories.TryGetValue(category,
                                    out BaseStatsCategory proCat))
                            {
                                proCat.BaseProperties.Add(new BaseStat(proCat, nameKey, value, true));
                                return true;
                            }

                            return false;
                        }
                    // 添加回家物品
                    case "AddHomeTpItem":
                        {
                            List<int> items = AsListOfInt(args[1]); // Item IDs
                            bool
                                isPotion = Convert
                                    .ToBoolean(args[
                                        2]); // Whether the item should meet the Infinite Potion requirement (stacked over 30 by default and can be changed via mod config)
                            bool isComebackItem = Convert.ToBoolean(args[3]); // Potion of Return like item

                            foreach (int item in items)
                            {
                                HomeTeleportingPlayer.HomeTeleportingItems.Add(
                                    new HomeTeleportingItem(item, isPotion, isComebackItem));
                            }

                            return false;
                        }
                    // 获取弹药链序列
                    case "GetAmmoChainSequence":
                        {
                            Item item = (Item)args[1];
                            if (item is null || item.IsAir ||
                                !item.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalItem) ||
                                globalItem.Chain is null)
                                return null;
                            return globalItem.Chain.SerializeData();
                        }
                    // 获取大背包物品
                    case "GetBigBagItems":
                        {
                            Player player = (Player)args[1];
                            return GetAllInventoryItemsList(player, "portable, inv", 110);
                        }
                    // 获取“任意弹药”物品的ID
                    case "GetUniversalAmmoId":
                        {
                            return ModContent.ItemType<UniversalAmmoIcon>();
                        }
                    default:
                        ImproveGame.Instance.Logger.Error($"Replacement type \"{msg}\" not found.");
                        return false;
                }
            }
        }
        catch (Exception e)
        {
            ImproveGame.Instance.Logger.Error($"{e.StackTrace} {e.Message}");
        }

        static List<int> AsListOfInt(object data) =>
            data as List<int> ?? new List<int> { Convert.ToInt32(data) };

        return false;
    }
}