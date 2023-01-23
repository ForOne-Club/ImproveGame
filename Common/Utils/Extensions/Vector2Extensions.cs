namespace ImproveGame.Common.Utils.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 X0(this Vector2 _this)
        {
            return new Vector2(_this.X, 0);
        }

        public static Vector2 Y0(this Vector2 _this)
        {
            return new Vector2(0, _this.Y);
        }

        public static Vector2 ToVector2(this float _this)
        {
            return new Vector2(_this);
        }
    }
}
