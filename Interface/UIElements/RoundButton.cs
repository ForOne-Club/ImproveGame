namespace ImproveGame.Interface.UIElements
{
    public class RoundButton : UIElement
    {
        public Asset<Texture2D> background;
        public Asset<Texture2D> hover;
        public Asset<Texture2D> mainImage;
        public Func<Color> OnGetColor;

        public RoundButton(Asset<Texture2D> mainImage)
        {
            background = GetTexture("UI/Brust/Background");
            hover = GetTexture("UI/Brust/Hover");
            this.SetSize(background.Size());

            this.mainImage = mainImage;
        }

        public Color GetColor()
        {
            return OnGetColor();
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + this.GetSize() / 2f;

            Color color = OnGetColor is null ? Color.White : GetColor();
            sb.Draw(background.Value, position, null, color, 0, this.GetSize() / 2f, 1f, 0, 0f);
            if (IsMouseHovering)
            {
                // color * 1.4f => 高光边框贴图应该亮一点
                sb.Draw(hover.Value, position, null, color * 1.4f, 0, this.GetSize() / 2f, 1f, 0, 0f);
            }

            sb.Draw(mainImage.Value, position, null, color, 0, mainImage.Size() / 2f, 0.8f, 0, 0f);
        }
    }
}
