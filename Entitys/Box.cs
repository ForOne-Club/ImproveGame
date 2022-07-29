using ImproveGame.Common.Systems;

namespace ImproveGame.Entitys
{
    public class Box
    {
        public int whoAmI;
        public Rectangle Rectangle; // √
        public Color borderColor; // √
        public Color backgroundColor; // √
        public TextDisplayMode textDisplayMode; // √
        public Texture2D PreView; // x
        /// <summary>
        /// 杀死 Box 的条件, 达到之后会从 DrawSystem.boxs 删除
        /// </summary>
        public Func<bool> NeedKill; // √
        public ModItem Parent; // √

        public Box(ModItem Parent, Func<bool> NeedKill, Rectangle Rectangle, Color backgroundColor, Color borderColor, TextDisplayMode textDisplayMode)
        {
            this.Parent = Parent;
            this.NeedKill = NeedKill;
            this.Rectangle = Rectangle;
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.textDisplayMode = textDisplayMode;
        }

        /// <summary>
        /// 创建一个Box, 如果你创建的Box已经存在 (通过 ModItem 来判断是否存在), 则直接修改已经存在的Box <para/>
        /// 第一个参数是 Box 对应的 ModItem, 每一个 Box 都必须绑定一个 ModItem <para/>
        /// 第二个参数是用来判断是否绘制的, 如果返回值为 <see langword="false"/> 会直接删除 box 对象再 <see cref="BoxSystem.boxs"/> 中的引用 <para/>
        /// </summary>
        /// <param name="Rectangle"></param>
        /// <returns>它在 <see cref="BoxSystem.boxs"/> 的下标. 返回 -1 代表没有位置了, 那样子就不会生成了</returns>
        public static int NewBox(ModItem Parent, Func<bool> NeedKill, Rectangle Rectangle, Color backgroundColor, Color borderColor, TextDisplayMode textDisplayMode = TextDisplayMode.None)
        {
            Box[] boxs = BoxSystem.boxs;
            int index = HasBox(Parent);
            if (boxs.IndexInRange(index))
            {
                Box box = boxs[index];
                box.NeedKill = NeedKill;
                box.Rectangle = Rectangle;
                box.backgroundColor = backgroundColor;
                box.borderColor = borderColor;
                box.textDisplayMode = textDisplayMode;
                return index;
            }
            else
            {
                Box box = new(Parent, NeedKill, Rectangle, backgroundColor, borderColor, textDisplayMode);
                for (int i = 0; i < BoxSystem.boxs.Length; i++)
                {
                    if (BoxSystem.boxs[i] == null)
                    {
                        BoxSystem.boxs[i] = box;
                        box.whoAmI = i;
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int NewBox(ModItem Parent, Func<bool> NeedKill, Point start, Point end, Color backgroundColor, Color borderColor, TextDisplayMode textDisplayMode = TextDisplayMode.None)
        {
            return NewBox(Parent, NeedKill, new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y), (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1), backgroundColor, borderColor, textDisplayMode);
        }

        public static int HasBox(ModItem item)
        {
            Box[] boxs = BoxSystem.boxs;
            for (int i = 0; i < boxs.Length; i++)
            {
                Box box = boxs[i];
                if (box is not null && box.Parent.Type == item.Type)
                    return i;
            }
            return -1;
        }

        public void Update()
        {
            Box[] boxs = BoxSystem.boxs;
            if ((NeedKill?.Invoke() ?? true) || Parent.Type != Main.LocalPlayer.HeldItem.type || Main.LocalPlayer.mouseInterface)
            {
                boxs[whoAmI] = null;
            }
        }

        public void Draw()
        {
            MyUtils.DrawBorderRect(Rectangle, backgroundColor, borderColor);
        }

        public void DrawPreView()
        {
            if (PreView is not null)
            {
                Main.spriteBatch.Draw(PreView, new Vector2(Rectangle.X, Rectangle.Y) * 16f - Main.screenPosition, null, Color.White * 0.5f, 0, Vector2.Zero, 1f, 0, 0);
            }
        }

        public void DrawString()
        {
            if (textDisplayMode is not TextDisplayMode.None)
            {
                string text = string.Empty;
                if (textDisplayMode == TextDisplayMode.All)
                {
                    text = $"{Rectangle.Width}×{Rectangle.Height}";
                }
                else if (textDisplayMode == TextDisplayMode.Width)
                {
                    text = Rectangle.Width.ToString();
                }
                else if (textDisplayMode == TextDisplayMode.Height)
                {
                    text = Rectangle.Height.ToString();
                }
                Vector2 size = FontAssets.MouseText.Value.MeasureString(Rectangle.Width.ToString()) * 1.2f;
                Vector2 position = Main.MouseScreen + new Vector2(16, -size.Y + 6);
                Utils.DrawBorderString(Main.spriteBatch, text, position, borderColor, 1.2f);
            }
        }
    }

    public enum TextDisplayMode
    {
        None,
        Width,
        Height,
        All
    }
}
