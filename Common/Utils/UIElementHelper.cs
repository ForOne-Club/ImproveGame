namespace ImproveGame.Common.Utils
{
    public static class UIElementHelper
    {
        public static void AppendS(this UIElement parent, params UIElement[] uies)
        {
            foreach (var uie in uies)
            {
                parent.Append(uie);
            }
        }

        public static void SetHPadding(this UIElement uie, float padding)
        {
            uie.PaddingLeft = padding;
            uie.PaddingRight = padding;
        }

        public static void SetVPadding(this UIElement uie, float padding)
        {
            uie.PaddingTop = padding;
            uie.PaddingBottom = padding;
        }

        public static Vector2 GetPPos(this UIElement uie)
        {
            return new(uie.Left.Pixels, uie.Top.Pixels);
        }

        public static void SetPPos(this UIElement uie, Vector2 position, float precentX = 0, float precentY = 0)
        {
            uie.SetPos(position.X, position.Y, precentX, precentY);
        }

        public static void SetPos(this UIElement uie, float x, float y, float precentX = 0, float precentY = 0)
        {
            uie.Left.Set(x, precentX);
            uie.Top.Set(y, precentY);
        }

        public static void SetSize(this UIElement uie, Vector2 size, float precentWidth = 0, float precentHeight = 0)
        {
            uie.SetSize(size.X, size.Y, precentWidth, precentHeight);
        }

        public static void SetSize(this UIElement uie, float width, float height, float precentWidth = 0, float precentHeight = 0)
        {
            uie.Width.Set(width, precentWidth);
            uie.Height.Set(height, precentHeight);
        }

        public static float Left(this UIElement uie) => uie.Left.Pixels;

        public static float Top(this UIElement uie) => uie.Top.Pixels;

        public static float Right(this UIElement uie) => uie.Left.Pixels + uie.Width.Pixels;

        public static float Bottom(this UIElement uie) => uie.Top.Pixels + uie.Height.Pixels;

        public static float Width(this UIElement uie) => uie.Width.Pixels;

        public static float Height(this UIElement uie) => uie.Height.Pixels;

        public static float WidthInside(this UIElement uie) => uie.Width.Pixels - uie.PaddingLeft - uie.PaddingRight;

        public static float HeightInside(this UIElement uie) => uie.Height.Pixels - uie.PaddingTop - uie.PaddingBottom;

        public static float HPadding(this UIElement uie) => uie.PaddingLeft + uie.PaddingRight;

        public static float VPadding(this UIElement uie) => uie.PaddingTop + uie.PaddingBottom;
    }
}
