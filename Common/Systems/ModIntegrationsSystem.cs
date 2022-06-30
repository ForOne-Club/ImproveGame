using System.Collections.Generic;
using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    public class ModIntegrationsSystem : ModSystem
    {
        /// <summary>
        /// 储存独立添加支持的模组放置类Buff（也就是只需要1堆叠即可生效），由于这个是加载后执行的，直接存ID即可
        /// <br>Key为物品ID，Value为BuffID</br>
        /// </summary>
        internal static Dictionary<int, int> ModdedPlaceableItemBuffs = new();
        /// <summary>
        /// 储存独立添加支持的模组药水类Buff（也就是需要30堆叠生效），由于这个是加载后执行的，直接存ID即可
        /// <br>Key为物品ID，Value为BuffID</br>
        /// </summary>
        internal static Dictionary<int, int> ModdedPotionBuffs = new();

        public override void PostSetupContent() {
            DoCalamityModIntegration();
        }

        private static void DoCalamityModIntegration() {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod)) {
                return;
            }
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

        public static void AddBuffIntegration(Mod mod, string itemName, string buffName, bool isPlaceable) {
            if (isPlaceable)
                ModdedPlaceableItemBuffs[mod.Find<ModItem>(itemName).Type] = mod.Find<ModBuff>(buffName).Type;
            else
                ModdedPotionBuffs[mod.Find<ModItem>(itemName).Type] = mod.Find<ModBuff>(buffName).Type;
        }

        public override void Unload() {
            ModdedPlaceableItemBuffs = null;
        }
    }
}
