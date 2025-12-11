using ImproveGame.Helpers.Extensions;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.UIFramework.SUIElements
{
    public class SUIPanel : View
    {
        /// <summary>
        /// 显示窗口阴影
        /// </summary>
        internal bool Shaded;

        internal float ShadowThickness;
        internal Color ShadowColor;

        /// <summary>
        /// 可拖动
        /// </summary>
        internal bool Draggable;
        internal bool Dragging;
        internal Vector2 Offset;

        /// <summary>
        /// 可调节大小
        /// </summary>
        internal bool Resizeable;
        internal bool Resizing;
        internal int MinResizeWidth = 200;
        internal int MinResizeHeight = 200;
        /// <summary>
        /// 鼠标放到可以调节大小的范围时，会显示一个物品图标，设置为-1则不显示
        /// </summary>
        internal int ItemIdForResizeIcon = ItemID.TitanGlove;

        public Rectangle ResizeRectangle
        {
            get
            {
                CalculatedStyle innerDimensions = GetInnerDimensions();
                return new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 12), (int)(innerDimensions.Y + innerDimensions.Height - 12), 12 + (int)PaddingRight, 12 + (int)PaddingBottom);
            }
        }

        public SUIPanel(Color borderColor, Color backgroundColor, float rounded = 12, float border = 2,
            bool draggable = false)
        {
            SetPadding(10f);
            Draggable = draggable;
            DragIgnore = true;

            ShadowThickness = UIStyle.ShadowThickness;
            ShadowColor = borderColor * 0.35f;

            Border = border;
            BorderColor = borderColor;
            BgColor = backgroundColor;
            Rounded = new Vector4(rounded);
        }

        public SUIPanel(Color backgroundColor, Color borderColor, Vector4 rounded, float border, bool draggable = false)
        {
            SetPadding(10f);
            DragIgnore = true;
            ShadowThickness = UIStyle.ShadowThickness;
            ShadowColor = borderColor * 0.35f;
            Draggable = draggable;

            Border = border;
            BorderColor = borderColor;
            BgColor = backgroundColor;
            Rounded = rounded;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (Resizeable && ResizeRectangle.Contains(evt.MousePosition.ToPoint()))
            {
                Offset = new Vector2(evt.MousePosition.X - PositionPixels.X - innerDimensions.Width, evt.MousePosition.Y - PositionPixels.Y - innerDimensions.Height);
                Resizing = true;
            }
            // 当点击的是子元素不进行移动
            else if (Draggable &&
                (evt.Target == this || (evt.Target is View view && view.DragIgnore) ||
                 evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
            {
                Offset = evt.MousePosition - PositionPixels;
                Dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            Dragging = false;
            Resizing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public Vector2 DragIncrement = Vector2.One;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Dragging)
            {
                var left = Main.mouseX - Offset.X;
                var top = Main.mouseY - Offset.Y;
                if (DragIncrement.X > 0)
                    left -= left % DragIncrement.X;
                if (DragIncrement.Y > 0)
                    top -= top % DragIncrement.Y;
                SetPosPixels(left, top).Recalculate();
            }

            if (Resizing)
            {
                CalculatedStyle dimensions = GetOuterDimensions();
                Width.Pixels = Math.Max(Main.MouseScreen.X - dimensions.X - Offset.X, MinResizeWidth);
                Height.Pixels = Math.Max(Main.MouseScreen.Y - dimensions.Y - Offset.Y, MinResizeHeight);
                Recalculate();
            }

            if (Resizeable && ResizeRectangle.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.instance.MouseText($"[i:{ItemIdForResizeIcon}]", 0, 0, Main.mouseX + 16, Main.mouseY - 10);
            }

            base.Draw(spriteBatch);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Vector2 shadowThickness = new Vector2(ShadowThickness);
            Vector2 shadowPos = pos - shadowThickness;
            Vector2 shadowSize = size + shadowThickness * 2;

            // 默认开启改的代码就少了
            // 然后放到 View 就不能默认开启了，要改太多
            // 复古模式自动没
            if (EnableBlur && BlurMakeSystem.BlurAvailable)
            {
                if (BlurMakeSystem.SingleBlur)
                {
                    var device = Main.graphics.GraphicsDevice;
                    var scissorRectangle = device.ScissorRectangle;
                    var batch = Main.spriteBatch;
                    batch.End();
                    BlurMakeSystem.MakeKawaseBlur();
                    device.ScissorRectangle = scissorRectangle;
                    batch.Begin(SpriteSortMode.Deferred, null, null, null, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
                }

                var scale = Main.UIScale;
                SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                    _dimensions.Position() * scale, _dimensions.Size() * scale, Rounded * scale, Matrix.Identity);
            }

            if (Shaded)
            {
                SDFRectangle.Shadow(shadowPos, shadowSize, Rounded + new Vector4(ShadowThickness), ShadowColor, ShadowThickness, Main.UIScaleMatrix);
            }
            base.DrawSelf(spriteBatch);
        }

        public bool EnableBlur { get; set; } = true;

        public override void DrawSDFRectangle()
        {
            // 写这里就会闪一下
            base.DrawSDFRectangle();
        }
    }
}