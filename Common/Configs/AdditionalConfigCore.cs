using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
using Newtonsoft.Json;
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
    }

    /// <summary>
    /// 用于序列化json和读取json的dummy类
    /// </summary>
    [Serializable]
    public class AdditionalConfig
    {
        public const string FileName = "ImproveGame_AdditionalConfig.json";
        public static readonly string FullPath = Path.Combine(ConfigManager.ModConfigPath, FileName);
        
        public LifeformAnalyzerConfig LifeformAnalyzer;
        [Serializable]
        public class LifeformAnalyzerConfig
        {
            public List<int> VanillaBlacklist = new();
            public List<string> ModdedBlacklist = new();
        }
        public bool UseKeybindTranslation;
        public Vector2 HugeInventoryUIPosition;

        public AdditionalConfig(bool serlizing)
        {
            LifeformAnalyzer = new();

            if (!serlizing)
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
            UseKeybindTranslation = KeybindSystem.UseKeybindTranslation;
            HugeInventoryUIPosition = UISystem.Instance.BigBagGUI.MainPanel is not null
                ? UISystem.Instance.BigBagGUI.MainPanel.GetDimensions().Position()
                : new Vector2(500f);
        }

        public void Populate()
        {
            LifeformAnalyzer?.VanillaBlacklist?.ForEach(i => LifeAnalyzeCore.Blacklist[i] = true);
            LifeformAnalyzer?.ModdedBlacklist?.ForEach(s =>
            {
                if (ModContent.TryFind<ModNPC>(s, out var modNpc))
                    LifeAnalyzeCore.Blacklist[modNpc.Type] = true;
            });
            KeybindSystem.UseKeybindTranslation = UseKeybindTranslation;
            UIPlayer.HugeInventoryUIPosition = HugeInventoryUIPosition == Vector2.Zero ? new(150, 340) : HugeInventoryUIPosition;
        }

        public static void Load()
        {
            var settings = new AdditionalConfig(false);

            bool jsonFileExists = File.Exists(FullPath);
            string json = jsonFileExists ? File.ReadAllText(FullPath) : "{}";

            try {
                JsonConvert.PopulateObject(json, settings, ConfigManager.serializerSettings);
            }
            catch (Exception e) when (jsonFileExists && e is JsonReaderException or JsonSerializationException) {
                ImproveGame.Instance.Logger.Warn($"Quality of Life mod additional config file located at {FullPath} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
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
