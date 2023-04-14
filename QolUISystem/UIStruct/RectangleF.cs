namespace ImproveGame.QolUISystem.UIStruct;

public struct RectangleF
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left => X;

    public float Right => X + Width;

    public float Top => Y;

    public float Bottom => Y + Height;

    public Vector2 Position
    {
        set
        {
            X = value.X;
            Y = value.Y;
        }
        get => new Vector2(X, Y);
    }

    public Vector2 Size
    {
        set
        {
            Width = value.X;
            Height = value.Y;
        }
        get => new Vector2(Width, Height);
    }

    public RectangleF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 通过父类的 ContentRectangle 属性获得自己的 OuterRectangle 属性
    /// </summary>
    /// <param name="parentRectangle"></param>
    /// <param name="position"></param>
    /// <param name="size"></param>
    public RectangleF(RectangleF parentRectangle, UIPosition position, UISize size)
    {
        Size = size.Pixels + parentRectangle.Size * size.Parent;
        Position =
            parentRectangle.Position +
            position.Pixel +
            parentRectangle.Size * position.Parent +
            (parentRectangle.Size - Size) * position.Align;
    }

    public bool Contains(int x, int y)
    {
        if (X <= x && x < X + Width && Y <= y)
        {
            return y < Y + Height;
        }

        return false;
    }

    public bool Contains(Vector2 value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }

        return false;
    }

    public bool Contains(Point value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }

        return false;
    }

    public bool Contains(Rectangle value)
    {
        if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
        {
            return value.Y + value.Height <= Y + Height;
        }

        return false;
    }

    public bool Contains(RectangleF value)
    {
        if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
        {
            return value.Y + value.Height <= Y + Height;
        }

        return false;
    }

    public void Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }

    public void Offset(Vector2 offset)
    {
        X += offset.X;
        Y += offset.Y;
    }

    public void Offset(Point offset)
    {
        X += offset.X;
        Y += offset.Y;
    }

    public bool Equals(RectangleF other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is RectangleF f && this == f;
    }

    public override string ToString()
    {
        return $"X:{X} Y:{Y} Width:{Width} Height:{Height}";
    }

    public bool Intersects(RectangleF value)
    {
        if (value.Left < Right && Left < value.Right && value.Top < Bottom)
        {
            return Top < value.Bottom;
        }

        return false;
    }

    public RectangleF Union(RectangleF value2)
    {
        float num = Math.Min(X, value2.X);
        float num2 = Math.Min(Y, value2.Y);
        return new RectangleF(num, num2, Math.Max(Right, value2.Right) - num, Math.Max(Bottom, value2.Bottom) - num2);
    }

    public static RectangleF Union(RectangleF value1, RectangleF value2)
    {
        float num = Math.Min(value1.X, value2.X);
        float num2 = Math.Min(value1.Y, value2.Y);
        return new RectangleF(num, num2, Math.Max(value1.Right, value2.Right) - num, Math.Max(value1.Bottom, value2.Bottom) - num2);
    }

    public static bool operator ==(RectangleF a, RectangleF b)
    {
        if (a.X == b.X && a.Y == b.Y && a.Width == b.Width)
        {
            return a.Height == b.Height;
        }

        return false;
    }

    public static bool operator !=(RectangleF a, RectangleF b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
    }
}
