using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    public class HideBuffResetSystem : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            // 专门用来清除ApplyBuffItem.BuffTypesShouldHide的
            ApplyBuffItem.BuffTypesShouldHide.Clear();
            HideGlobalBuff.HidedBuffCountThisFrame = 0;

            var items = MyUtils.GetAllInventoryItemsList(Main.LocalPlayer, false);
            foreach (var item in items) {
                item.GetGlobalItem<ApplyBuffItem>().UpdateInventoryGlow(item);
            }
        }
    }
}
