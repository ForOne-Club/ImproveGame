using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
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
            public bool LiquidAbsorption;
            public byte LiquidSelectedType;

            [DefaultValue(WandSystem.PaintMode.Tile)]
            public WandSystem.PaintMode PaintMode;
        }

        public const string FileName = "ImproveGame_AdditionalConfig.json";
        public static readonly string FullPath = Path.Combine(ConfigManager.ModConfigPath, FileName);

        public LifeformAnalyzerConfig LifeformAnalyzer = new();
        public WandModeConfig WandMode = new();
        public bool UseKeybindTranslation;
        public Vector2 HugeInventoryUIPosition;

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
                    case < NPCID.Count:
                        LifeformAnalyzer.VanillaBlacklist.Add(id);
                        break;
                    case >= NPCID.Count:
                        LifeformAnalyzer.ModdedBlacklist.Add(NPCLoader.GetNPC(id).FullName);
                        break;
                }
            }

            LifeformAnalyzer.VanillaBlacklist.Sort();
            LifeformAnalyzer.ModdedBlacklist.Sort();

            WandMode.BrustRangeFixed = WandSystem.FixedMode;
            WandMode.BrustDestroyTile = WandSystem.TileMode;
            WandMode.BrustDestroyWall = WandSystem.WallMode;
            WandMode.LiquidAbsorption = WandSystem.AbsorptionMode;
            WandMode.LiquidSelectedType = WandSystem.LiquidMode;
            WandMode.PaintMode = WandSystem.PaintWandMode;

            UseKeybindTranslation = KeybindSystem.UseKeybindTranslation;
            HugeInventoryUIPosition = UISystem.Instance.BigBagGUI.MainPanel is not null
                ? UISystem.Instance.BigBagGUI.MainPanel.GetDimensions().Position()
                : new(150, 340);
            UIPlayer.HugeInventoryUIPosition = HugeInventoryUIPosition; // 在这里也保存一下
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
                WandSystem.AbsorptionMode = WandMode.LiquidAbsorption;
                WandSystem.LiquidMode = WandMode.LiquidSelectedType;
                WandSystem.PaintWandMode = WandMode.PaintMode;
            }

            KeybindSystem.UseKeybindTranslation = UseKeybindTranslation;
            UIPlayer.HugeInventoryUIPosition = HugeInventoryUIPosition == Vector2.Zero
                ? new(150, 340)
                : HugeInventoryUIPosition;
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
                    $"Quality of Life mod additional config file located at {FullPath} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
                File.Delete(FullPath);
                JsonConvert.PopulateObject("{}", settings, ConfigManager.serializerSettings);
            }

            settings.Populate();
        }

        public static void Save()
        {
            if (Main.dedServ)
                return;
            Directory.CreateDirectory(ConfigManager.ModConfigPath);
            string json = JsonConvert.SerializeObject(new AdditionalConfig(true), ConfigManager.serializerSettings);
            File.WriteAllText(FullPath, json);
        }
    }
}