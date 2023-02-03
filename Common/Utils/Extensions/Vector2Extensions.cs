using System.Drawing;

namespace ImproveGame.Common.Utils.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Abs(this Vector2 _this)
        {
            return new Vector2(MathF.Abs(_this.X), MathF.Abs(_this.Y));
        }

        public static Vector2 ToX(this Vector2 _this)
        {
            return new Vector2(_this.X, 0);
        }

        public static Vector2 ToY(this Vector2 _this)
        {
            return new Vector2(0, _this.Y);
        }

        public static Vector2 ToX(this float _this)
        {
            return new Vector2(_this, 0);
        }

        public static Vector2 ToY(this float _this)
        {
            return new Vector2(0, _this);
        }

        public static Vector2 ToXY(this float _this)
        {
            return new Vector2(_this);
        }
    }
}
