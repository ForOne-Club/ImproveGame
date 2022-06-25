using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class LiquidWand : SelectorItem
    {
        // 还没做好，先不发
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override bool ModifySelectedTiles(Player player, int i, int j) {
            if (WandSystem.AbsorptionMode) {
                
            }
            return true;
        }

        public override bool AltFunctionUse(Player player) => true;

        protected Point ExtraRange;
        protected Point KillSize;

        public override void SetItemDefaults() {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(20, 10);
            KillSize = new(5, 3);
            ExtraRange = new(5, 3);
        }

        public override bool StartUseItem(Player player) {
            if (player.altFunctionUse == 0) {
                MyUtils.ItemRotation(player);
            }
            else if (player.altFunctionUse == 2) {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override void HoldItem(Player player) {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI) {
                // 还在用物品的时候不能打开UI
                if (player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1) {
                    return;
                }
                if (!BrustGUI.Visible) {
                    BrustGUI.Open();
                }
                else {
                    BrustGUI.Close();
                }
            }
        }

        //public override void AddRecipes() {
        //    CreateRecipe()
        //        .AddTile(TileID.WorkBenches).Register();
        //}
    }
}
