using ImproveGame.Common.ConstructCore;
using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.UIElements
{
    internal class StructurePreviewPanel : UIElement
    {
        public string FilePath { get; private set; }

        internal float ViewScale;
        internal int StructureWidth;
        internal int StructureHeight;
        internal int OriginX;
        internal int OriginY;
        private bool _cacheUpdateResetHeight; // 不知道为啥当场设置没用，摆烂下一帧设置
        private bool _cacheSetOrigin;
        private bool _oldMouseLeft; // 经典点击判断

        public event UIElementAction OnResetHeight;

        public StructurePreviewPanel(string path)
        {
            FilePath = path;
            PreviewRenderer.UIPreviewPath = path;
            this.SetSize(580f, 10f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath) || PreviewRenderer.UIPreviewTarget is null || StructureWidth is 0 || StructureHeight is 0)
                return;

            var center = GetDimensions().Center();

            float scale = 1f;
            if (PreviewRenderer.UIPreviewTarget.Width >= 500f)
            {
                scale = 500f / PreviewRenderer.UIPreviewTarget.Width;
            }
            ViewScale = scale;

            spriteBatch.Draw(PreviewRenderer.UIPreviewTarget, center, null, Color.White, 0f, PreviewRenderer.UIPreviewTarget.Size() / 2f, scale, SpriteEffects.None, 0f);

            // 获取左上角位置
            var leftTop = center - PreviewRenderer.UIPreviewTarget.Size() / 2f * scale + new Vector2(2f);
            // 获取鼠标相对左上角的位置，也就是将坐标映射到RT2D坐标上
            var mouseInUI = Main.MouseScreen - leftTop;
            // 将鼠标的RT2D坐标转换为物块坐标
            var mouseInUITiles = (mouseInUI / 16f / scale).ToPoint();

            if (mouseInUITiles.X <= StructureWidth && mouseInUITiles.Y <= StructureHeight && mouseInUI.X >= 0 && mouseInUI.Y >= 0)
            {
                // 转换回屏幕坐标
                var mouseTiledInScreen = mouseInUITiles.ToVector2() * 16f * scale + leftTop;
                DrawBorder(mouseTiledInScreen, 16f * scale, 16f * scale, Color.SkyBlue * 0.35f, Color.SkyBlue);

                if (!_oldMouseLeft && Main.mouseLeft) // 经典点击
                {
                    OriginX = mouseInUITiles.X;
                    OriginY = mouseInUITiles.Y;
                    _cacheSetOrigin = true;
                }
            }

            // 绘制Tag存储的OriginX, Y
            var originInScreen = new Vector2(OriginX, OriginY) * 16f * scale + leftTop;
            DrawBorder(originInScreen, 16f * scale, 16f * scale, Color.Yellow * 0.35f, Color.Yellow);

            _oldMouseLeft = Main.mouseLeft; // 经典
        }

        public override void Update(GameTime gameTime)
        {
            if (_cacheUpdateResetHeight)
            {
                OnResetHeight?.Invoke(this);
                _cacheUpdateResetHeight = false;
            }

            var tag = FileOperator.GetTagFromFile(FilePath);

            if (tag is null)
                return;

            if (_cacheSetOrigin)
            {
                tag.Set("OriginX", OriginX, true);
                tag.Set("OriginY", OriginY, true);
                TagIO.ToFile(tag, FilePath);
                FileOperator.CachedStructureDatas.Remove(FilePath);
                tag = FileOperator.GetTagFromFile(FilePath);
                _cacheSetOrigin = false;
            }

            StructureWidth = tag.GetInt("Width");
            StructureHeight = tag.GetInt("Height");
            OriginX = tag.GetInt("OriginX");
            OriginY = tag.GetInt("OriginY");

            float uiHeight = StructureHeight * 16f + 50f;
            if (Height.Pixels != uiHeight * ViewScale)
            {
                Height.Pixels = uiHeight * ViewScale;
                Recalculate();
                _cacheUpdateResetHeight = true;
                PreviewRenderer.UIPreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, StructureWidth * 16 + 20, StructureHeight * 16 + 20, false, default, default, default, RenderTargetUsage.PreserveContents);
                PreviewRenderer.ResetUIPreviewTarget = true;
            }

            base.Update(gameTime);
        }
    }
}