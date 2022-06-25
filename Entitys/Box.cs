using ImproveGame.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ImproveGame.Entitys
{
    public class Box
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Color borderColor;
        public Color backgroundColor;
        public bool ShowWidth;
        public bool ShowHeight;
        public Texture2D PreView;

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public static int NewBox(Rectangle rect, Color backgroundColor, Color borderColor)
        {
            Box box = new Box(rect, backgroundColor, borderColor);
            for (int i = 0; i < DrawSystem.boxs.Length; i++)
            {
                if (DrawSystem.boxs[i] == null)
                {
                    DrawSystem.boxs[i] = box;
                    return i;
                }
            }
            return -1;
        }

        public static int NewBox(Point start, Point end, Color backgroundColor, Color borderColor)
        {
            return NewBox((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y),
                 (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1,
                 backgroundColor, borderColor);
        }

        public static int NewBox(int x, int y, int width, int height, Color backgroundColor, Color borderColor)
        {
            Box box = new Box(x, y, width, height, backgroundColor, borderColor);
            for (int i = 0; i < DrawSystem.boxs.Length; i++)
            {
                if (DrawSystem.boxs[i] == null)
                {
                    DrawSystem.boxs[i] = box;
                    return i;
                }
            }
            return -1;
        }

        public Box(Rectangle rect, Color backgroundColor, Color borderColor)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
        }

        public Box(int x, int y, int width, int height, Color backgroundColor, Color borderColor)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
        }

        public void Draw()
        {
            MyUtils.DrawBorderRect(Rect, backgroundColor, borderColor);
        }
    }
}
