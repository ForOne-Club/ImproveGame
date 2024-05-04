﻿using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions;
using ImproveGame.Content.Functions.HomeTeleporting;
using ImproveGame.Content.Items;
using ImproveGame.Content.Items.Globes;
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
    internal static List<int> ModdedInfBuffsIgnore = new();

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

        AddBuffIntegration(calamityMod, "WeightlessCandle", "CirrusBlueCandleBuff", true);
        AddBuffIntegration(calamityMod, "VigorousCandle", "CirrusPinkCandleBuff", true);
        AddBuffIntegration(calamityMod, "SpitefulCandle", "CirrusYellowCandleBuff", true);
        AddBuffIntegration(calamityMod, "ResilientCandle", "CirrusPurpleCandleBuff", true);
        AddBuffIntegration(calamityMod, "ChaosCandle", "ChaosCandleBuff", true);
        AddBuffIntegration(calamityMod, "TranquilityCandle", "TranquilityCandleBuff", true);
        AddBuffIntegration(calamityMod, "EffigyOfDecay", "EffigyOfDecayBuff", true);
        AddBuffIntegration(calamityMod, "CrimsonEffigy", "CrimsonEffigyBuff", true);
        AddBuffIntegration(calamityMod, "CorruptionEffigy", "CorruptionEffigyBuff", true);
        PlayerStatsSystem.CalamityIntegration(calamityMod);
    }

    private static void DoThoriumModIntegration()
    {
        if (!ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod))
            return;

        AddBuffIntegration(thoriumMod, "Altar", "AltarBuff", true);
        AddBuffIntegration(thoriumMod, "ConductorsStand", "ConductorsStandBuff", true);
        AddBuffIntegration(thoriumMod, "Mistletoe", "MistletoeBuff", true);
        AddBuffIntegration(thoriumMod, "NinjaRack", "NinjaBuff", true);
        AddHomeTpIntegration(thoriumMod, "WishingGlass", false, false);
        PlayerStatsSystem.ThoriumIntegration(thoriumMod);
    }

    private static void DoFargowiltasIntegration()
    {
        if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargowiltas))
            return;

        AddBuffIntegration(fargowiltas, "Omnistation", "Omnistation", true);
        AddBuffIntegration(fargowiltas, "Omnistation2", "Omnistation", true);
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

    private static void AddBuffIntegration(Mod mod, string itemName, string buffName, bool isPlaceable)
    {
        if (isPlaceable)
            ModdedPlaceableItemBuffs[mod.Find<ModItem>(itemName).Type] = new List<int> { mod.Find<ModBuff>(buffName).Type };
        else
            ModdedPotionBuffs[mod.Find<ModItem>(itemName).Type] = new List<int> { mod.Find<ModBuff>(buffName).Type };
    }

    private static void AddHomeTpIntegration(Mod mod, string itemName, bool isPotion, bool isComebackItem)
    {
        // 属于是自我测试了
        Call("AddHomeTpItem", mod.Find<ModItem>(itemName).Type, isPotion, isComebackItem);
    }
    private static void DoShopLookupIntegration()
    {
        if (!ModLoader.TryGetMod("ShopLookup", out Mod mod))
            return;

        mod.Call(3, ImproveGame.Instance, "Wand", TextureAssets.Item[ModContent.ItemType<StarburstWand>()].Value,
            new NPCShop(-1, "Wand")
                .Add<CreateWand>(gold: 10)
                .Add<MagickWand>(gold: 10)
                .Add<PaintWand>(gold: 5)
                .Add<MoveChest>(gold: 15)
                .Add<WallPlace>(Condition.DownedKingSlime, gold: 8)
                .Add<SpaceWand>(Condition.DownedKingSlime, gold: 12, silver: 50)
                .Add<LiquidWand>(Condition.DownedEowOrBoc, gold: 20)
                .Add<StarburstWand>(Condition.Hardmode, gold: 50)
                .Add<ConstructWand>(Condition.Hardmode, gold: 30));
        mod.Call(3, ImproveGame.Instance, "Locator", TextureAssets.Item[ModContent.ItemType<AetherGlobe>()].Value,
            new NPCShop(-1, "Locator")
                .Add<FloatingIslandGlobe>(gold: 8)
                .Add<PyramidGlobe>(gold: 16)
                .Add<AetherGlobe>(gold: 30)
                .Add<DungeonGlobe>(gold: 1, silver: 80)
                .Add<EnchantedSwordGlobe>(gold: 8)
                .Add<PlanteraGlobe>(Condition.Hardmode, gold: 16)
                .Add<TempleGlobe>(Condition.DownedPlantera, gold: 20));
        mod.Call(3, ImproveGame.Instance, "Other", TextureAssets.Item[ModContent.ItemType<ExtremeStorage>()].Value,
            new NPCShop(-1, "Other")
                .Add<BannerChest>(gold: 2, silver: 50)
                .Add<ExtremeStorage>(gold: 12)
                .Add<Autofisher>(gold: 4)
                .Add<DetectorDrone>(gold: 2, silver: 20)
                .Add<StorageCommunicator>(Condition.Hardmode, gold: 50)
                .Add<BaitSupplier>(gold: 30)
                .Add<PotionBag>(gold: 2, silver: 50)
                .Add<Dummy>(silver: 50)
                .Add<WeatherBook>(Condition.DownedEowOrBoc, gold: 25, silver: 60));
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
                            List<int> potions = AsListOfInt(args[1]); // Potion IDs
                            ModdedInfBuffsIgnore.AddRange(potions);
                            return true;
                        }
                    case "AddPotion":
                        {
                            int itemType = Convert.ToInt32(args[1]); // Item ID
                            List<int> buffTypes = AsListOfInt(args[2]); // Buff IDs
                            ModdedPotionBuffs[itemType] = buffTypes;
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
                            Texture2D modIcon = (Texture2D)args[3];

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

                            if (PlayerStatsSystem.Instance.StatsCategories.TryGetValue(category, out BaseStatsCategory proCat))
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
                            bool isPotion = Convert.ToBoolean(args[2]); // Whether the item should meet the Infinite Potion requirement (stacked over 30 by default and can be changed via mod config)
                            bool isComebackItem = Convert.ToBoolean(args[3]); // Potion of Return like item

                            foreach (int item in items)
                            {
                                HomeTeleportingPlayer.HomeTeleportingItems.Add(
                                    new HomeTeleportingItem(item, isPotion, isComebackItem));
                            }
                            return false;
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