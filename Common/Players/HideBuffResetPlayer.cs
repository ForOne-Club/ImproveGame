using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using Terraria.ModLoader;

namespace ImproveGame.Common.Players
{
    public class HideBuffResetPlayer : ModPlayer
    {
        // 专门用来清除ApplyBuffItem.BuffTypesShouldHide的
        public override void ResetEffects() {
            ApplyBuffItem.BuffTypesShouldHide.Clear();
            HideGlobalBuff.HidedBuffCountThisFrame = 0;
        }
    }
}
