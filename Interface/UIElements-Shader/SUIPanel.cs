using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class SUIPanel : UIElement
    {
        public Color borderColor;
        public Color backgroundColor;

        public SUIPanel(Color borderColor, Color backgroundColor)
        {
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            SetPadding(16);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            PixelShader.DrawBox(Main.UIScaleMatrix, GetDimensions().Position(), this.GetSize(),
                12, 3, borderColor, backgroundColor);
        }
    }
}
