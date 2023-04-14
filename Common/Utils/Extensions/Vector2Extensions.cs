using System.Drawing;

namespace ImproveGame.Common.Utils.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Abs(this Vector2 _this)
        {
            return new Vector2(MathF.Abs(_this.X), MathF.Abs(_this.Y));
        }

        public static float MaxXY(this Vector2 _this)
        {
            return Math.Max(_this.X, _this.Y);
        }

        public static float MinXY(this Vector2 _this)
        {
            return Math.Min(_this.X, _this.Y);
        }
    }
}
