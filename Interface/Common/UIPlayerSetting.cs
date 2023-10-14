using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.Common
{
    public class UIPlayerSetting : ModPlayer
    {
        public bool SuperVault_HeCheng;
        public bool SuperVault_SmartGrab;
        public bool SuperVault_OverflowGrab;
        public bool FuzzySearch;
        public bool SearchTooltip;

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(nameof(SuperVault_HeCheng), out SuperVault_HeCheng);
            tag.TryGet(nameof(SuperVault_SmartGrab), out SuperVault_SmartGrab);
            tag.TryGet(nameof(SuperVault_OverflowGrab), out SuperVault_OverflowGrab);
            tag.TryGet(nameof(FuzzySearch), out FuzzySearch);
            tag.TryGet(nameof(SearchTooltip), out SearchTooltip);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Set(nameof(SuperVault_HeCheng), SuperVault_HeCheng);
            tag.Set(nameof(SuperVault_SmartGrab), SuperVault_SmartGrab);
            tag.Set(nameof(SuperVault_OverflowGrab), SuperVault_OverflowGrab);
            tag.Set(nameof(FuzzySearch), FuzzySearch);
            tag.Set(nameof(SearchTooltip), SearchTooltip);
        }
    }
}
