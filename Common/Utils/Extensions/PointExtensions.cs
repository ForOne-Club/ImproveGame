using Terraria.DataStructures;

namespace ImproveGame.Common.Utils.Extensions;

public static class PointExtensions
{
    public static float DistanceSQ(this Point16 point1, Point16 point2)
    {
        return (MathF.Pow(point1.X - point2.X, 2)) + MathF.Pow(point1.Y - point2.Y, 2);
    }
    public static float DistanceSQ(this Point point1, Point point2)
    {
        return (MathF.Pow(point1.X - point2.X, 2)) + MathF.Pow(point1.Y - point2.Y, 2);
    }
    public static float Distance(this Point point1, Point point2)
    {
        return MathF.Sqrt(DistanceSQ(point1, point2));
    }

    public static Point Min(Point point1, Point point2)
    {
        return new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
    }

    public static Point Abs(this Point _this)
    {
        return new Point(Math.Abs(_this.X), Math.Abs(_this.Y));
    }

    public static float MaxXY(this Point _this)
    {
        return Math.Max(_this.X, _this.Y);
    }

    public static float MinXY(this Point _this)
    {
        return Math.Min(_this.X, _this.Y);
    }
}