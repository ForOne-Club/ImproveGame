using ImproveGame.Common.Configs.FavoritedSystem;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions;
using ImproveGame.UI;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UI.ItemSearcher;
using ImproveGame.UI.MasterControl;
using ImproveGame.UI.OpenBag;
using ImproveGame.UI.PlayerStats;
using ImproveGame.UI.WorldFeature;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.Common;
using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    /// <summary>
    /// 额外配置，用于不写进配置文件的设置选项
    /// </summary>
    public class AdditionalConfigCore : ModSystem
    {
        public override void PostSetupContent()
        {
            if (Main.dedServ)
                return;
            AdditionalConfig.Load();
        }

        public override void PreSaveAndQuit() => AdditionalConfig.Save();
    }

    /// <summary>
    /// 用于序列化json和读取json的dummy类
    /// </summary>
    [Serializable]
    public class AdditionalConfig
    {
        [Serializable]
        public class LifeformAnalyzerConfig
        {
            public List<int> VanillaBlacklist = new();
            public List<string> ModdedBlacklist = new();
        }

        [Serializable]
        public class WandModeConfig
        {
            [DefaultValue(true)] public bool BrustRangeFixed;
            [DefaultValue(true)] public bool BrustDestroyTile;
            [DefaultValue(true)] public bool BrustDestroyWall;
            [DefaultValue(false)] public bool BrustDestroyChest;
            public bool LiquidAbsorption;
            public short LiquidSelectedType;

            [DefaultValue(WandSystem.PaintMode.Tile)]
            public WandSystem.PaintMode PaintMode;
        }

        public HashSet<string> FavoritedModernConfigOptions;

        public const string FileName = "ImproveGame_AdditionalConfig.json";
        public static readonly string FullPath = Path.Combine(ConfigManager.ModConfigPath, FileName);

        public LifeformAnalyzerConfig LifeformAnalyzer = new();
        public WandModeConfig WandMode = new();
        public bool UseKeybindTranslation;
        public Vector2 HugeInventoryUIPosition;
        public Vector2 BuffTrackerPosition;
        public Vector2 WorldFeaturePosition;
        public Vector2 ItemSearcherPosition;
        public Vector2 OpenBagPosition;
        public Vector2 PlayerInfoTogglePosition;
        public bool MasterControlPinned;
        public bool ExtremeStorageGUIDisplayCrafting;

        /// <summary>
        /// (根据模组内容)获取 Config
        /// </summary>
        /// <param name="serializing">是否正在序列化(保存状态)</param>
        public AdditionalConfig(bool serializing)
        {
            if (!serializing)
                return;

            foreach ((int id, bool blacklisted) in LifeAnalyzeCore.Blacklist)
            {
                if (!blacklisted)
                    continue;
                switch (id)
                {
                    case < 688:
                        LifeformAnalyzer.VanillaBlacklist.Add(id);
                        break;
                    case >= 688:
                        LifeformAnalyzer.ModdedBlacklist.Add(NPCLoader.GetNPC(id).FullName);
                        break;
                }
            }

            LifeformAnalyzer.VanillaBlacklist.Sort();
            LifeformAnalyzer.ModdedBlacklist.Sort();

            WandMode.BrustRangeFixed = WandSystem.FixedMode;
            WandMode.BrustDestroyTile = WandSystem.TileMode;
            WandMode.BrustDestroyWall = WandSystem.WallMode;
            WandMode.BrustDestroyChest = WandSystem.ChestMode;
            WandMode.LiquidAbsorption = WandSystem.AbsorptionMode;
            WandMode.LiquidSelectedType = WandSystem.LiquidMode;
            WandMode.PaintMode = WandSystem.PaintWandMode;

            FavoritedModernConfigOptions = FavoritedOptionDatabase.FavoritedOptions;

            UseKeybindTranslation = KeybindSystem.UseKeybindTranslation;

            var uiSystem = UISystem.Instance;

            HugeInventoryUIPosition = BigBagGUI.Instance?.MainPanel?.GetDimensions().Position() ??
                                      new Vector2(150, 340);
            UIPlayer.HugeInventoryUIPosition = HugeInventoryUIPosition; // 在这里也保存一下

            BuffTrackerPosition = uiSystem.BuffTrackerGUI?.MainPanel?.GetDimensions().Position() ??
                                  new Vector2(630, 160);
            UIPlayer.BuffTrackerPosition = BuffTrackerPosition; // 在这里也保存一下

            WorldFeaturePosition = WorldFeatureGUI.Instance?.MainPanel?.GetDimensions().Position() ??
                                   new Vector2(250, 280);
            UIPlayer.WorldFeaturePosition = WorldFeaturePosition; // 在这里也保存一下

            ItemSearcherPosition = ItemSearcherGUI.Instance?.MainPanel?.GetDimensions().Position() ??
                                   new Vector2(620, 400);
            UIPlayer.ItemSearcherPosition = ItemSearcherPosition; // 在这里也保存一下

            OpenBagPosition = OpenBagGUI.Instance?.MainPanel?.GetDimensions().Position() ??
                              new Vector2(410, 360);
            UIPlayer.OpenBagPosition = OpenBagPosition; // 在这里也保存一下

            PlayerInfoTogglePosition = PlayerStatsGUI.Instance?.ControllerSwitch?.GetDimensions().Position() ??
                                       UIPlayer.PlayerInfoToggleDefPosition;
            UIPlayer.PlayerInfoTogglePosition = PlayerInfoTogglePosition; // 在这里也保存一下

            MasterControlPinned = MasterControlGUI.Pinned;

            ExtremeStorageGUIDisplayCrafting = ExtremeStorageGUI.DisplayCrafting;
        }

        /// <summary>
        /// 应用 Config 内容
        /// </summary>
        public void Populate()
        {
            LifeformAnalyzer?.VanillaBlacklist?.ForEach(i => LifeAnalyzeCore.Blacklist[i] = true);
            LifeformAnalyzer?.ModdedBlacklist?.ForEach(s =>
            {
                if (ModContent.TryFind<ModNPC>(s, out var modNpc))
                    LifeAnalyzeCore.Blacklist[modNpc.Type] = true;
            });

            // 旧版兼容
            if (WandMode is not null)
            {
                WandSystem.FixedMode = WandMode.BrustRangeFixed;
                WandSystem.TileMode = WandMode.BrustDestroyTile;
                WandSystem.WallMode = WandMode.BrustDestroyWall;
                WandSystem.ChestMode = WandMode.BrustDestroyChest;
                WandSystem.AbsorptionMode = WandMode.LiquidAbsorption;
                WandSystem.LiquidMode = WandMode.LiquidSelectedType;
                WandSystem.PaintWandMode = WandMode.PaintMode;
            }

            if (FavoritedModernConfigOptions is null || FavoritedModernConfigOptions.Count is 0)
                FavoritedOptionDatabase.SetDefaultFavoritedOptions();
            else
                FavoritedOptionDatabase.FavoritedOptions = FavoritedModernConfigOptions;

            KeybindSystem.UseKeybindTranslation = UseKeybindTranslation;
            UIPlayer.HugeInventoryUIPosition = HugeInventoryUIPosition == Vector2.Zero
                ? new Vector2(150, 340)
                : HugeInventoryUIPosition;
            UIPlayer.BuffTrackerPosition = BuffTrackerPosition == Vector2.Zero
                ? new Vector2(630, 160)
                : BuffTrackerPosition;
            UIPlayer.WorldFeaturePosition = WorldFeaturePosition == Vector2.Zero
                ? new Vector2(250, 280)
                : WorldFeaturePosition;
            UIPlayer.ItemSearcherPosition = ItemSearcherPosition == Vector2.Zero
                ? new Vector2(620, 400)
                : ItemSearcherPosition;
            UIPlayer.OpenBagPosition = OpenBagPosition == Vector2.Zero
                ? new Vector2(410, 360)
                : OpenBagPosition;
            UIPlayer.PlayerInfoTogglePosition = PlayerInfoTogglePosition == Vector2.Zero
                ? UIPlayer.PlayerInfoToggleDefPosition
                : PlayerInfoTogglePosition;

            MasterControlGUI.Pinned = MasterControlPinned;

            ExtremeStorageGUI.DisplayCrafting = ExtremeStorageGUIDisplayCrafting;
        }

        public static void Load()
        {
            var settings = new AdditionalConfig(false);

            bool jsonFileExists = File.Exists(FullPath);
            string json = jsonFileExists ? File.ReadAllText(FullPath) : "{}";

            try
            {
                JsonConvert.PopulateObject(json, settings, ConfigManager.serializerSettings);
            }
            catch (Exception e) when (jsonFileExists && e is JsonReaderException or JsonSerializationException)
            {
                ImproveGame.Instance.Logger.Warn(
                    $"Quality of Terraria mod additional config file located at {FullPath} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
                File.Delete(FullPath);
                JsonConvert.PopulateObject("{}", settings, ConfigManager.serializerSettings);
            }

            settings.Populate();
        }

        public static void Save()
        {
            if (Main.dedServ)
                return;

            try
            {
                Directory.CreateDirectory(ConfigManager.ModConfigPath);
                string json = JsonConvert.SerializeObject(new AdditionalConfig(true), ConfigManager.serializerSettings);
                File.WriteAllText(FullPath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while saving Quality of Terraria Additional Config.");
                Console.WriteLine(e);
            }
        }
    }
}