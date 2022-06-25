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
            Width.Set(Back9.Width, 0f);
            Height.Set(Back9.Height, 0f);

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

        // 左键点击事件
        public override void MouseDown(UIMouseEvent evt) {
            SetCursorOverride();
            LeftClickItem();
        }
        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseDown(UIMouseEvent evt) {
            RightMouseTimer = 0;
            RightMouseKeepPressItem();
        }

        /// <summary>
        /// 鼠标右键放开
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseUp(UIMouseEvent evt) {
            RightMouseTimer = -1;
        }

        /// <summary>
        /// Update 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (RightMouseTimer >= 60) {
                RightMouseKeepPressItem();
            }
            else if (RightMouseTimer >= 30 && RightMouseTimer % 3 == 0) {
                RightMouseKeepPressItem();
            }
            else if (RightMouseTimer >= 15 && RightMouseTimer % 6 == 0) {
                RightMouseKeepPressItem();
            }
            if (RightMouseTimer != -1) {
                RightMouseTimer++;
            }
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="sb"></param>
        protected override void DrawSelf(SpriteBatch sb) {
            // 按下 Ctrl 改变鼠标指针外观
            if (IsMouseHovering && !Item.IsAir) {
                SetCursorOverride();
            }
            // 绘制背景框
            CalculatedStyle dimensions = GetDimensions();
            if (Item.favorited) {
                sb.Draw(Back10, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);
            }
            else {
                sb.Draw(Back9, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);
            }

            // 绘制物品
            if (!Item.IsAir) {
                if (Item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    RasterizerState rasterizerState = sb.GraphicsDevice.RasterizerState;
                    Rectangle rectangle1 = sb.GraphicsDevice.ScissorRectangle;
                    sb.End();
                    sb.GraphicsDevice.RasterizerState = rasterizerState;
                    sb.GraphicsDevice.ScissorRectangle = rectangle1;
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                        DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
                    Color lerpColor;
                    float time = ImprovePlayer.G(Main.LocalPlayer).PlayerTimer;
                    if (time % 60f < 30) {
                        lerpColor = Color.Lerp(Color.White * 0.25f, Color.Transparent, (float)(time % 60f % 30 / 29));
                    }
                    else {
                        lerpColor = Color.Lerp(Color.Transparent, Color.White * 0.25f, (float)(time % 60f % 30 / 29));
                    }
                    MyAssets.ItemEffect.Parameters["uColor"].SetValue(lerpColor.ToVector4());
                    MyAssets.ItemEffect.CurrentTechnique.Passes["Test"].Apply();
                }
                Rectangle rectangle;
                if (Main.itemAnimations[Item.type] == null) {
                    rectangle = ItemTexture2D.Frame(1, 1, 0, 0);
                }
                else {
                    rectangle = Main.itemAnimations[Item.type].GetFrame(ItemTexture2D.Value);
                }
                float textureSize = 30f;
                float size = rectangle.Width > textureSize || rectangle.Height > textureSize ?
                    rectangle.Width > rectangle.Height ? textureSize / rectangle.Width : textureSize / rectangle.Height :
                    1f;
                sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), Item.GetAlpha(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
                sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), Item.GetColor(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);

                if (Item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    Item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
                    RasterizerState rasterizerState = sb.GraphicsDevice.RasterizerState;
                    Rectangle rectangle1 = sb.GraphicsDevice.ScissorRectangle;
                    sb.End();
                    sb.GraphicsDevice.RasterizerState = rasterizerState;
                    sb.GraphicsDevice.ScissorRectangle = rectangle1;
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                        DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
                }
            }
            // 物品信息
            if (IsMouseHovering) {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item.Clone();
            }
            text.SetText(Item.IsAir || Item.stack <= 1 ? "" : Item.stack.ToString(), 0.8f, false);
            text.Recalculate();
        }
    }
}
