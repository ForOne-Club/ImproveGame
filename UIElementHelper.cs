using Terraria.UI;

namespace ImproveGame
{
    public static class UIElementHelper
    {
        public static void SetPos(this UIElement uie, float x, float y, float precentX = 0, float precentY = 0) {
            uie.Left.Set(x, precentX);
            uie.Top.Set(y, precentY);
        }

        public static void SetSize(this UIElement uie, float width, float height, float precentWidth = 0, float precentHeight = 0) {
            uie.Width.Set(width, precentWidth);
            uie.Height.Set(height, precentHeight);
        }

        public static float Left(this UIElement uie) => uie.Left.Pixels;

        public static float Top(this UIElement uie) => uie.Top.Pixels;

        public static float Width(this UIElement uie) => uie.Width.Pixels;

        public static float Height(this UIElement uie) => uie.Height.Pixels;

        public static float WidthInside(this UIElement uie) => uie.Width.Pixels - uie.PaddingLeft - uie.PaddingRight;

        public static float HeightInside(this UIElement uie) => uie.Height.Pixels - uie.PaddingTop - uie.PaddingBottom;

        public static float HPadding(this UIElement uie) => uie.PaddingLeft + uie.PaddingRight;

        public static float VPadding(this UIElement uie) => uie.PaddingTop + uie.PaddingBottom;
    }
}
