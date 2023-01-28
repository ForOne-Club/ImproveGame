using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using System.Reflection;

namespace ImproveGame.Common.Systems
{
    public class LifeAnalyzeCore : ModSystem
    {
        private bool _mouseLeftPrev;
        internal static List<NPC> RaritiedNpcs;
        internal static Dictionary<int,bool> Blacklist = new();

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Info Accessories Bar");
            if (index != -1)
            {
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: Modify Info Acc Display",
                    () =>
                    {
                        var cursorText = Main.instance._mouseTextCache.cursorText;
                
                        if (Main.mouseText && !string.IsNullOrEmpty(cursorText) && cursorText == Lang.inter[105].Value + "\n" + GetText("LifeAnalyzer.Tip"))
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
