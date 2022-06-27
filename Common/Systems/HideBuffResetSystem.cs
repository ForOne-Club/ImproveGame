using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    public class HideBuffResetSystem : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            // 专门用来清除ApplyBuffItem.BuffTypesShouldHide的
            ApplyBuffItem.BuffTypesShouldHide.Clear();
            HideGlobalBuff.HidedBuffCountThisFrame = 0;
        }
    }
}
