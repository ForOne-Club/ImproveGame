namespace ImproveGame.Common.Utils
{
    public static class RectangleHelper
    {
        public static bool Contains(this CalculatedStyle calculatedStyle, Vector2 position)
        {
            if (calculatedStyle.X <= position.X && position.X < calculatedStyle.X + calculatedStyle.Width &&
                calculatedStyle.Y <= position.Y)
            {
                return position.Y < calculatedStyle.Y + calculatedStyle.Height;
            }

            return false;
        }

        public static bool Contains(this CalculatedStyle calculatedStyle, Point position)
        {
            if (calculatedStyle.X <= position.X && position.X < calculatedStyle.X + calculatedStyle.Width &&
                calculatedStyle.Y <= position.Y)
            {
                return position.Y < calculatedStyle.Y + calculatedStyle.Height;
            }

            return false;
        }

        public static Vector2 Size(this CalculatedStyle calculatedStyle)
        {
            return new Vector2(calculatedStyle.Width, calculatedStyle.Height);
        }
    }
}
