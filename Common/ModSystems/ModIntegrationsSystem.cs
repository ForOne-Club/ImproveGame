using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.Packets;
using ImproveGame.Content.Patches;
using ImproveGame.Interface.GUI;
using MonoMod.RuntimeDetour.HookGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.ModSystems;

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

    internal static bool NoLakeSizePenaltyLoaded = false;
    internal static bool WMITFLoaded = false;
    internal static bool DialogueTweakLoaded = false;

    internal static int MultipleLuresAmount;
    internal static int UnloadedItemType;

    public override void PostSetupContent()
    {
        DoCalamityModIntegration();
        DoFargowiltasIntegration();
        DoRecipeBrowserIntegration();
        DoDialogueTweakIntegration();
        DoModLoaderIntegration();
        DoMultipleLuresIntegration();
        NoLakeSizePenaltyLoaded = ModLoader.HasMod("NoLakeSizePenalty");
        WMITFLoaded = ModLoader.HasMod("WMITF");

        // Config加载的钩子
        MonoModHooks.Add(typeof(ConfigManager).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static),
            (Action<ModConfig> orig, ModConfig config) =>
            {
                orig.Invoke(config);
                DoMultipleLuresIntegration();
            });
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

    private static void DoMultipleLuresIntegration()
    {
        if (!ModLoader.HasMod("MultipleLures"))
            return;

        string luresConfigPath = Path.Combine(ConfigManager.ModConfigPath, "MultipleLures_Configuration.json");
        if (!File.Exists(luresConfigPath))
            return;

        string jsonText = File.ReadAllText(luresConfigPath);
        JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
        string amountText = jo?["LuresAmount"]?.ToString() ?? "0";
        if (!int.TryParse(amountText, out MultipleLuresAmount))
            MultipleLuresAmount = 0;
    }

    private static void AddCraftStationIntegration(Mod mod, string itemName, List<int> tileIDs)
    {
        PortableStations[mod.Find<ModItem>(itemName).Type] = tileIDs;
    }

    private static void AddBuffIntegration(Mod mod, string itemName, string buffName, bool isPlaceable)
    {
        if (isPlaceable)
            ModdedPlaceableItemBuffs[mod.Find<ModItem>(itemName).Type] = new List<int>(mod.Find<ModBuff>(buffName).Type);
        else
            ModdedPotionBuffs[mod.Find<ModItem>(itemName).Type] = new List<int>(mod.Find<ModBuff>(buffName).Type);
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
                            var potions = AsListOfInt(args[1]); // Potion IDs
                            ModdedInfBuffsIgnore.AddRange(potions);
                            return true;
                        }
                    case "AddPotion":
                        {
                            var itemType = Convert.ToInt32(args[1]); // Item ID
                            var buffTypes = AsListOfInt(args[2]); // Buff IDs
                            ModdedPotionBuffs[itemType] = buffTypes;
                            return true;
                        }
                    case "AddStation":
                        {
                            var itemType = Convert.ToInt32(args[1]); // Item ID
                            var buffTypes = AsListOfInt(args[2]); // Buff IDs
                            ModdedPlaceableItemBuffs[itemType] = buffTypes;
                            return true;
                        }
                    case "AddPortableCraftingStation":
                        {
                            var itemType = Convert.ToInt32(args[1]); // Item ID
                            var tileIDs = AsListOfInt(args[2]); // Tile IDs
                            PortableStations[itemType] = tileIDs;
                            return true;
                        }
                        case "AddAutofisherAccessory":
                        {
                            var itemType = Convert.ToInt32(args[1]); // Item ID
                            var fishingAddition = Convert.ToSingle(args[2]); // Fishing Addition
                            var fishingPower = Convert.ToInt32(args[3]); // Fishing Power
                            var tackleBox = Convert.ToBoolean(args[4]); // Tackle Box
                            var lavaFishing = Convert.ToBoolean(args[5]); // Lava Fishing
                            AccessoryAttribute.FishingAddition[itemType] = fishingAddition;
                            AccessoryAttribute.FishingPower[itemType] = fishingPower;
                            AccessoryAttribute.TackleBox[itemType] = tackleBox;
                            AccessoryAttribute.LavaFishing[itemType] = lavaFishing;
                            return true;
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
            data is List<int> ? data as List<int> : new List<int>() {Convert.ToInt32(data)};

        return false;
    }
}