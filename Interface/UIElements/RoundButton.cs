using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements
{
    public class RoundButton : UIElement
    {
        public Asset<Texture2D> background;
        public Asset<Texture2D> hover;
        public Asset<Texture2D> mainImage;

        public float Opacity;

        public Func<bool> Selected;
        public Func<string> text;

        // 针不错, 全都加上渐变真不错!
        public AnimationTimer HoverTimer;
        public AnimationTimer SelectedTimer;

        public string Text => text?.Invoke();

        public RoundButton(Asset<Texture2D> mainImage)
        {
            background = GetTexture("UI/Brust/Background");
            hover = GetTexture("UI/Brust/Hover");

            this.SetSize(background.Size());
            this.mainImage = mainImage;

            HoverTimer = new(3);
            SelectedTimer = new(3);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
            SelectedTimer.Update();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            Main.NewText($"{typeof(RoundButton)}.MouseOver");
            base.MouseOver(evt);
            HoverTimer.Open();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            Main.NewText($"{typeof(RoundButton)}.MouseOut");
            base.MouseOut(evt);
            HoverTimer.Close();
        }

        public Color GetColor()
        {
            if (Selected())
            {
                if (!SelectedTimer.AnyOpen)
                    SelectedTimer.Open();
            }
            else
            {
                if (!SelectedTimer.AnyClose)
                    SelectedTimer.Close();
            }
            return Color.Lerp(Color.Gray, Color.White, SelectedTimer.Schedule);
        }

        /*private readonly Color borderColor = new(233, 176, 0);
        private readonly Color light1 = new(192, 130, 255);
        private readonly Color drak1 = new(96, 65, 127);
        private readonly Color light2 = new(56, 156, 255);
        private readonly Color drak2 = new(28, 78, 127);*/

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + this.GetSize() / 2f;
            Color color = GetColor() * Opacity;

            sb.Draw(background.Value, position, null, color, 0, this.GetSize() / 2f, 1f, 0, 0f);

            // color * 1.4f => 高光边框贴图应该亮一点
            Color borderColor = color * 1.4f * HoverTimer.Schedule;
            sb.Draw(hover.Value, position, null, borderColor, 0, this.GetSize() / 2f, 1f, 0, 0f);

            /*Color borderColor = Color.Lerp(Color.White, this.borderColor, HoverTimer.Schedule);
            Color background1 = Color.Lerp(drak1, light1, SelectedTimer.Schedule);
            Color background2 = Color.Lerp(drak2, light2, SelectedTimer.Schedule);

            PixelShader.DrawBox(Main.UIScaleMatrix, GetDimensions().Position(),
                this.GetSize(), Width.Pixels / 2, 3, borderColor * Opacity, borderColor * Opacity,
                background1 * Opacity, background2 * Opacity);*/

            sb.Draw(mainImage.Value, position, null, color, 0, mainImage.Size() / 2f, 0.8f, 0, 0f);
        }
    }
}
