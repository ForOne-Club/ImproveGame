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

            HoverTimer = new(2);
            SelectedTimer = new(2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
            SelectedTimer.Update();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            HoverTimer.Open();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
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

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + this.GetSize() / 2f;
            Color color = GetColor() * Opacity;

            sb.Draw(background.Value, position, null, color, 0, this.GetSize() / 2f, 1f, 0, 0f);

            // color * 1.4f => 高光边框贴图应该亮一点
            Color borderColor = color * 1.4f * HoverTimer.Schedule;
            sb.Draw(hover.Value, position, null, borderColor, 0, this.GetSize() / 2f, 1f, 0, 0f);

            sb.Draw(mainImage.Value, position, null, color, 0, mainImage.Size() / 2f, 0.8f, 0, 0f);
        }
    }
}
