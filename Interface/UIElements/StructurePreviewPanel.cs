using ImproveGame.Common.ConstructCore;

namespace ImproveGame.Interface.UIElements
{
    internal class StructurePreviewPanel : UIElement
    {
        public string FilePath { get; private set; }

        internal int StructureWidth;
        internal int StructureHeight;
        private bool _cacheUpdateResetHeight; // 不知道为啥当场设置没用，摆烂下一帧设置

        public event UIElementAction OnResetHeight;

        public StructurePreviewPanel(string path)
        {
            FilePath = path;
            PreviewRenderer.UIPreviewPath = path;
            this.SetSize(540f, 10f);
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

            spriteBatch.Draw(PreviewRenderer.UIPreviewTarget, center, null, Color.White, 0f, PreviewRenderer.UIPreviewTarget.Size() / 2f, scale, SpriteEffects.None, 0f); ;
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

            StructureWidth = tag.GetInt("Width");
            StructureHeight = tag.GetInt("Height");

            float uiHeight = StructureHeight * 16f + 50f;
            if (Height.Pixels != uiHeight)
            {
                Height.Pixels = uiHeight;
                Recalculate();
                _cacheUpdateResetHeight = true;
                PreviewRenderer.UIPreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)StructureWidth * 16 + 20, (int)StructureHeight * 16 + 20, false, default, default, default, RenderTargetUsage.PreserveContents);
            }

            base.Update(gameTime);
        }
    }
}