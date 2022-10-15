using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Systems
{
    public class LifeAnalyzeCore : ModSystem
    {
        private bool _mouseLeftPrev;
        public const string FileName = "ImproveGame_LifeformAnalyzerBlacklist.json";
        public static readonly string FullPath = Path.Combine(ConfigManager.ModConfigPath, FileName);

        internal static List<NPC> RaritiedNpcs;
        internal static Dictionary<int,bool> Blacklist = new();

        public static void Save()
        {
            Directory.CreateDirectory(ConfigManager.ModConfigPath);
            string json = JsonConvert.SerializeObject(new LifeAnalyzeSettingsDummy(true), ConfigManager.serializerSettings);
            File.WriteAllText(FullPath, json);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Info Accessories Bar");
            if (index != -1)
            {
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: Modify Info Acc Display",
                    () =>
                    {
                        // private MouseTextCache _mouseTextCache
                        var mouseTextCache = Main.instance.GetType().GetField("_mouseTextCache", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(Main.instance);
                        if (mouseTextCache is null)
                        {
                            return true;
                        }

                        var cursorTextField = mouseTextCache.GetType().GetField("cursorText", BindingFlags.Public | BindingFlags.Instance);
                        var cursorText = cursorTextField?.GetValue(mouseTextCache) as string;
                
                        if (Main.mouseText && cursorTextField is not null && cursorText is not null && cursorText == Lang.inter[105].Value + "\n" + GetText("LifeAnalyzer.Tip"))
                        {
                            if (Main.mouseLeft && !_mouseLeftPrev)
                            {
                                switch (LifeformAnalyzerGUI.Visible)
                                {
                                    case false:
                                        UISystem.Instance.LifeformAnalyzerGUI.Open();
                                        break;
                                    case true:
                                        UISystem.Instance.LifeformAnalyzerGUI.Close();
                                        break;
                                }
                            }

                            _mouseLeftPrev = Main.mouseLeft;
                        }
                        return true;
                    }, InterfaceScaleType.UI));

            }
        }

        public override void Load()
        {
            RaritiedNpcs = new();
            foreach ((_, NPC npc) in ContentSamples.NpcsByNetId)
            {
                if (npc?.rarity > 0)
                {
                    RaritiedNpcs.Add(npc);
                }
            }
            
            var settings = new LifeAnalyzeSettingsDummy(false);

            bool jsonFileExists = File.Exists(FullPath);
            string json = jsonFileExists ? File.ReadAllText(FullPath) : "{}";

            try {
                JsonConvert.PopulateObject(json, settings, ConfigManager.serializerSettings);
            }
            catch (Exception e) when (jsonFileExists && e is JsonReaderException or JsonSerializationException) {
                ImproveGame.Instance.Logger.Warn($"Quality of Life life analyzer blacklist file located at {FullPath} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
                File.Delete(FullPath);
                JsonConvert.PopulateObject("{}", settings, ConfigManager.serializerSettings);
            }

            settings.PopulateToBlacklistArray();
        }

        public override void PreSaveAndQuit() => Save();
    }

    /// <summary>
    /// 用于序列化json
    /// </summary>
    public class LifeAnalyzeSettingsDummy
    {
        public List<int> VanillaBlacklist = new();
        public List<string> ModdedBlacklist = new();

        public LifeAnalyzeSettingsDummy(bool serlizing)
        {
            if (!serlizing)
                return;

            foreach ((int id, bool blacklisted) in LifeAnalyzeCore.Blacklist)
            {
                if (!blacklisted)
                    continue;
                switch (id)
                {
                    case < NPCID.Count:
                        VanillaBlacklist.Add(id);
                        break;
                    case >= NPCID.Count:
                        ModdedBlacklist.Add(NPCLoader.GetNPC(id).FullName);
                        break;
                }
            }

            VanillaBlacklist.Sort();
            ModdedBlacklist.Sort();
        }

        public void PopulateToBlacklistArray()
        {
            VanillaBlacklist?.ForEach(i => LifeAnalyzeCore.Blacklist[i] = true);
            ModdedBlacklist?.ForEach(s =>
            {
                if (ModContent.TryFind<ModNPC>(s, out var modNpc))
                    LifeAnalyzeCore.Blacklist[modNpc.Type] = true;
            });
        }
    }

    public class LifeformAnaliyzerModify : GlobalInfoDisplay
    {
        public static int counter;
        public static int number;

        public override void ModifyDisplayName(InfoDisplay currentDisplay, ref string displayName)
        {
            if (currentDisplay == InfoDisplay.LifeformAnalyzer)
            {
                displayName += "\n" + GetText("LifeAnalyzer.Tip");
            }
        }

        public override void ModifyDisplayValue(InfoDisplay currentDisplay, ref string displayValue)
        {
            if (currentDisplay != InfoDisplay.LifeformAnalyzer)
                return;
                
            const int maxDistance = 1300;
            int maxRarity = 0;
            int npcIndex = -1;
            if (counter <= 0) {
                counter = 15;
                for (int k = 0; k < 200; k++)
                {
                    var n = Main.npc[k];
                    if (!LifeAnalyzeCore.Blacklist.GetValueOrDefault(n.netID) && n.active && n.rarity > maxRarity && (n.Center - Main.LocalPlayer.Center).Length() < maxDistance) {
                        npcIndex = k;
                        maxRarity = n.rarity;
                    }
                }

                number = (byte)npcIndex;
            }
            else {
                counter--;
                npcIndex = number;
            }

            displayValue = (npcIndex is < 0 or >= 200 || !Main.npc[npcIndex].active || Main.npc[npcIndex].rarity <= 0) ? Language.GetTextValue("GameUI.NoRareCreatures") : Main.npc[npcIndex].GivenOrTypeName;
        }
    }
}
