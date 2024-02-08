namespace ImproveGame.UIFramework.UIStructs;

public struct RectangleFloat
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left => X;

    public float Right => X + Width;

    public float Top => Y;

    public float Bottom => Y + Height;

    public Vector2 RightBottom => new Vector2(X + Width, Y + Height);

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

    public Vector2 Center => new Vector2(X + Width / 2f, Y + Height / 2f);

    public RectangleFloat(float position, float size)
    {
        X = Y = position;
        Width = Height = size;
    }

    public RectangleFloat(float position, Vector2 size)
    {
        X = Y = position;
        Width = size.X;
        Height = size.Y;
    }

    public RectangleFloat(Vector2 position, float size)
    {
        X = position.X;
        Y = position.Y;
        Width = size;
        Height = size;
    }

    public RectangleFloat(Vector2 position, Vector2 size)
    {
        X = position.X;
        Y = position.Y;
        Width = size.X;
        Height = size.Y;
    }

    public RectangleFloat(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public RectangleFloat(float x, float y, Vector2 size)
    {
        X = x;
        Y = y;
        Width = size.X;
        Height = size.Y;
    }

    public RectangleFloat(float x, float y, float size)
    {
        X = x;
        Y = y;
        Width = Height = size;
    }

    public RectangleFloat CeilingSize()
    {
        Width = MathF.Ceiling(Width);
        Height = MathF.Ceiling(Height);
        return this;
    }

    public readonly Rectangle ToRectangle()
    {
        return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
    }

    public static RectangleFloat Transform(RectangleFloat rectangle, Matrix matrix)
    {
        Vector2 leftTop = Vector2.Transform(rectangle.Position, matrix);
        Vector2 rightBottom = Vector2.Transform(rectangle.RightBottom, matrix);

        return new RectangleFloat(leftTop.X, leftTop.Y, rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);
    }

    public readonly bool Contains(int x, int y)
    {
        if (X <= x && x < X + Width && Y <= y)
        {
            return y < Y + Height;
        }

        return false;
    }

    public readonly bool Contains(Vector2 value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }

        return false;
    }

    public readonly bool Contains(Point value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }

        return false;
    }

    public readonly bool Contains(Rectangle value)
    {
        if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
        {
            return value.Y + value.Height <= Y + Height;
        }

        return false;
    }

    public readonly bool Contains(RectangleFloat value)
    {
        if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
        {
            return value.Y + value.Height <= Y + Height;
        }

        return false;
    }

    public RectangleFloat Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
        return this;
    }

    public RectangleFloat Offset(Vector2 offset)
    {
        X += offset.X;
        Y += offset.Y;
        return this;
    }

    public RectangleFloat Offset(Point offset)
    {
        X += offset.X;
        Y += offset.Y;
        return this;
    }

    public RectangleFloat Increase(int width, int height)
    {
        Width += width;
        Height += height;
        return this;
    }

    public RectangleFloat Increase(Vector2 size)
    {
        Width += size.X;
        Height += size.Y;
        return this;
    }

    public RectangleFloat Increase(Point size)
    {
        Width += size.X;
        Height += size.Y;
        return this;
    }

    public RectangleFloat Increase(float x, float y, float width, float height)
    {
        X += x;
        Y += y;
        Width += width;
        Height += height;
        return this;
    }

    public RectangleFloat Increase(RectangleFloat rectangle)
    {
        X += rectangle.X;
        Y += rectangle.Y;
        Width += rectangle.Width;
        Height += rectangle.Height;
        return this;
    }

    public RectangleFloat Increase(Vector2 position, Vector2 size)
    {
        X += position.X;
        Y += position.Y;
        Width += size.X;
        Height += size.Y;
        return this;
    }

    public RectangleFloat Increase(Rectangle rectangle)
    {
        X += rectangle.X;
        Y += rectangle.Y;
        Width += rectangle.Width;
        Height += rectangle.Height;
        return this;
    }

    public bool Equals(RectangleFloat other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is RectangleFloat rectangle && this == rectangle;
    }

    public override string ToString()
    {
        return $"X:{X} Y:{Y} Width:{Width} Height:{Height}";
    }

    public static bool operator ==(RectangleFloat a, RectangleFloat b)
    {
        return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator !=(RectangleFloat a, RectangleFloat b)
    {
        return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
    }

    public static RectangleFloat operator +(RectangleFloat value1, RectangleFloat value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        value1.Width += value2.Width;
        value1.Height += value2.Height;
        return value1;
    }

    public static RectangleFloat operator -(RectangleFloat value1, RectangleFloat value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        value1.Width -= value2.Width;
        value1.Height -= value2.Height;
        return value1;
    }

    public static RectangleFloat operator *(RectangleFloat value1, RectangleFloat value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        value1.Width *= value2.Width;
        value1.Height *= value2.Height;
        return value1;
    }

    public static RectangleFloat operator /(RectangleFloat value1, RectangleFloat value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        value1.Width /= value2.Width;
        value1.Height /= value2.Height;
        return value1;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
    }
}
