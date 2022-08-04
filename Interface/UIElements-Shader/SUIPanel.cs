using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class SUIPanel : UIElement
    {
        public float radius;
        public float border;
        public Color borderColor;
        public Color backgroundColor;
        public bool CalculateBorder;

        public SUIPanel(Color borderColor, Color backgroundColor, float radius = 12, float border = 3, bool CalculateBorder = true)
        {
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.radius = radius;
            this.border = border;
            this.CalculateBorder = CalculateBorder;
            SetPadding(16);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            Vector2 position = dimenstions.Position();
            Vector2 size = new(dimenstions.Width, dimenstions.Height);

            if (CalculateBorder)
            {
                position -= new Vector2(border);
                size += new Vector2(border * 2);
            }

            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, radius, border,
                borderColor, backgroundColor);
        }
    }
}
