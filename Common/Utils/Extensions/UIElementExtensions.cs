namespace ImproveGame.Common.Utils.Extensions
{
    public static class CalculatedStyleExtensions
    {
        public static Vector2 Size(this CalculatedStyle calculatedStyle)
        {
            return new Vector2(calculatedStyle.Width, calculatedStyle.Height);
        }
    }

    public static class UIElementExtensions
    {
        public static UIElement SetCenterPixels(this UIElement uie, Vector2 center)
        {
            uie.Left.Pixels = center.X - uie.Width.Pixels / 2f;
            uie.Top.Pixels = center.Y - uie.Height.Pixels / 2f;
            return uie;
        }

        public static UIElement SetCenterPixels(this UIElement uie, float x, float y)
        {
            uie.Left.Pixels = x - uie.Width.Pixels / 2f;
            uie.Top.Pixels = y - uie.Height.Pixels / 2f;
            return uie;
        }

        public static Vector2 GetSize(this UIElement uie)
        {
            return new(uie.Width.Pixels, uie.Height.Pixels);
        }

        public static Vector2 GetInnerSizePixels(this UIElement uie)
        {
            return new(uie.Width.Pixels - uie.HPadding(), uie.Height.Pixels - uie.VPadding());
        }

        public static UIElement SetPos(this UIElement uie, Vector2 position, float precentX = 0, float precentY = 0)
        {
            uie.SetPos(position.X, position.Y, precentX, precentY);
            return uie;
        }

        public static UIElement SetPos(this UIElement uie, float x, float y, float precentX = 0, float precentY = 0)
        {
            uie.Left.Set(x, precentX);
            uie.Top.Set(y, precentY);
            return uie;
        }

        public static UIElement SetAlign(this UIElement uie, float horizontalAlign = -1f, float verticalAlign = -1f)
        {
            if (horizontalAlign is not -1f)
                uie.HAlign = horizontalAlign;
            if (verticalAlign is not -1f)
                uie.VAlign = verticalAlign;
            return uie;
        }

        public static UIElement SetSize(this UIElement uie, Vector2 size, float precentWidth = 0,
            float precentHeight = 0)
        {
            uie.Width.Set(size.X, precentWidth);
            uie.Height.Set(size.Y, precentHeight);
            return uie;
        }

        public static UIElement SetSize(this UIElement uie, float width, float height, float precentWidth = 0,
            float precentHeight = 0)
        {
            uie.Width.Set(width, precentWidth);
            uie.Height.Set(height, precentHeight);
            return uie;
        }

        public static float Left(this UIElement uie) => uie.Left.Pixels;

        public static float Top(this UIElement uie) => uie.Top.Pixels;

        public static float Right(this UIElement uie) => uie.Left.Pixels + uie.Width.Pixels;

        public static float Bottom(this UIElement uie) => uie.Top.Pixels + uie.Height.Pixels;

        public static float Width(this UIElement uie) => uie.Width.Pixels;

        public static float Height(this UIElement uie) => uie.Height.Pixels;

        public static float HPadding(this UIElement uie) => uie.PaddingLeft + uie.PaddingRight;

        public static float VPadding(this UIElement uie) => uie.PaddingTop + uie.PaddingBottom;
    }
}