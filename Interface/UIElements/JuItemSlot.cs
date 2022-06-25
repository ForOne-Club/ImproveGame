using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    partial class JuItemSlot : UIElement
    {
        public JuItemSlot(Item[] SuperVault, int index) {
            this.SuperVault = SuperVault;
            this.index = index;
            Width.Set(Back.Width, 0f);
            Height.Set(Back.Height, 0f);

            text = new UIText("") {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            Append(text);

            UIText text2 = new UIText((index + 1).ToString(), 0.75f) {
                VAlign = 0.15f
            };
            text2.Left.Set(0, 0.15f);
            Append(text2);
        }

        public override void MouseDown(UIMouseEvent evt) {
            SetCursorOverride();
            LeftClickItem();
        }

        public override void RightMouseDown(UIMouseEvent evt) {
            RightMouseTimer = 0;
            TakeSlotItemToMouseItem();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (Main.mouseRight && IsMouseHovering) {
                if (RightMouseTimer >= 60) {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 30 && RightMouseTimer % 3 == 0) {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 15 && RightMouseTimer % 6 == 0) {
                    TakeSlotItemToMouseItem();
                }
                RightMouseTimer++;
            }
        }

        protected override void DrawSelf(SpriteBatch sb) {
            // 按下 Ctrl 改变鼠标指针外观
            if (IsMouseHovering && !Item.IsAir) {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item.Clone();
                SetCursorOverride();
            }
            // 绘制背景框
            CalculatedStyle dimensions = GetDimensions();
            sb.Draw(backgroundTexture2D, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f, 0, 0f);

            DrawItem(sb, Item, dimensions);

            text.SetText(Item.IsAir || Item.stack <= 1 ? "" : Item.stack.ToString(), 0.8f, false);
            text.Recalculate();
        }
    }
}
