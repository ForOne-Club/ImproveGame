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
        /// <summary>
        /// A list of the items that don't apply infinite buffs
        /// </summary>
        internal static List<int> ModdedInfBuffsIgnore = new();

        internal static bool NoLakeSizePenaltyLoaded = false;

        public override void PostSetupContent()
        {
            DoCalamityModIntegration();
            DoFargowiltasIntegration();
            NoLakeSizePenaltyLoaded = ModLoader.HasMod("NoLakeSizePenalty");
        }

        private static void DoCalamityModIntegration()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
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

        private static void DoFargowiltasIntegration()
        {
            if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargowiltas))
            {
                return;
            }
            AddBuffIntegration(fargowiltas, "Omnistation", "Omnistation", true);
            AddBuffIntegration(fargowiltas, "Omnistation2", "Omnistation", true);
        }

        public static void AddBuffIntegration(Mod mod, string itemName, string buffName, bool isPlaceable)
        {
            if (isPlaceable)
                ModdedPlaceableItemBuffs[mod.Find<ModItem>(itemName).Type] = mod.Find<ModBuff>(buffName).Type;
            else
                ModdedPotionBuffs[mod.Find<ModItem>(itemName).Type] = mod.Find<ModBuff>(buffName).Type;
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
                                var buffType = Convert.ToInt32(args[2]); // Buff ID
                                ModdedPotionBuffs[itemType] = buffType;
                                return true;
                            }
                        case "AddStation":
                            {
                                var itemType = Convert.ToInt32(args[1]); // Item ID
                                var buffType = Convert.ToInt32(args[2]); // Buff ID
                                ModdedPlaceableItemBuffs[itemType] = buffType;
                                return true;
                            }
                        default:
                            ImproveGame.Instance.Logger.Error($"Replacement type \"{msg}\" not found.");
                            return false;
                    }
                }
            } catch (Exception e)
            {
                ImproveGame.Instance.Logger.Error($"{e.StackTrace} {e.Message}");
            }

            static List<int> AsListOfInt(object data) => data is List<int> ? data as List<int> : new List<int>() { Convert.ToInt32(data) };

            return false;
        }
    }
}
