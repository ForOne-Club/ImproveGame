using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    partial class JuItemSlot : UIElement
    {
        public JuItemSlot(Item[] items, int index) {
            this.items = items;
            this.index = index;
            Width.Set(TextureAssets.InventoryBack.Value.Width, 0f);
            Height.Set(TextureAssets.InventoryBack.Value.Height, 0f);

            UIText text = new UIText((index + 1).ToString(), 0.75f) {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            text.OnUpdate += (uie) => {
                (uie as UIText).SetText(Item.IsAir || Item.stack <= 1 ? string.Empty : Item.stack.ToString());
                (uie as UIText).Recalculate();
            };
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
            sb.Draw(Background, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f, 0, 0f);

            DrawItem(sb, Item, dimensions);

            /*Vector2 position = dimensions.Position();
            string text = Item.IsAir || Item.stack <= 1 ? "" : Item.stack.ToString();
            Vector2 textSize = MyUtils.GetStringSize(text) * 0.8f;
            position.Y += this.Height() * 0.8f;
            position.X += this.Width() * 0.2f;
            MyUtils.DrawString(position, text, Color.White, Color.Black, 0.8f);*/
        }
    }
}
