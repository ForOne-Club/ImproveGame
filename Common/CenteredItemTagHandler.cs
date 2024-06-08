using ReLogic.Graphics;
using Terraria.GameContent.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Common;

/// <summary>
/// 仅仅是为了图鉴里显示居中
/// </summary>
public class CenteredItemTagHandler : ITagHandler
{
    private class CenteredItemSnippet : TextSnippet
    {
        internal int HeightOffset;
        private Item _item;

        public CenteredItemSnippet(Item item)
        {
            _item = item;
            Color = ItemRarity.GetColor(item.rare);
        }

        public override void OnHover()
        {
            Main.HoverItem = _item.Clone();
            Main.instance.MouseText(_item.Name, _item.rare, 0);
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
            Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                if (ModernConfigDrawing)
                {
                    position.X += 10f;
                    position.Y -= 6f;
                }
                else
                {
                    position.X += 8f;
                    position.Y += 8f;
                }
                _item.DrawIcon(spriteBatch, Color.White, position, 24f, scale);
                if (_item.stack > 1)
                {
                    var text = _item.stack.ToString();
                    Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(text) * 0.75f;
                    Vector2 textPos = position + new Vector2(-4f, 9f) - textSize / 2f;
                    spriteBatch.DrawItemStackString(text, textPos, scale * 0.8f);
                }
            }

            size = new Vector2(24) * scale;
            return true;
        }

        public override float GetStringLength(DynamicSpriteFont font) => 28 * Scale;
    }

    TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
    {
        Item item = new();
        if (int.TryParse(text, out int result) && result < ItemLoader.ItemCount)
            item.netDefaults(result);

        // Add support for [i:ModItem.FullName] ([i:ExampleMod/ExampleItem]). Coincidentally support [i:ItemID.FieldName] ([i:GoldBar])
        if (ItemID.Search.TryGetId(text, out result))
            item.netDefaults(result);

        if (item.type <= ItemID.None)
            return new TextSnippet(text);

        int heightOffset = 0;
        item.stack = 1;
        if (options != null)
        {
            string[] array = options.Split(',');
            foreach (var arg in array)
            {
                if (arg.Length == 0)
                    continue;

                switch (arg[0])
                {
                    case 's':
                        {
                            if (int.TryParse(arg[1..], out int stack))
                                item.stack = stack;

                            break;
                        }
                    case 'h':
                        {
                            if (int.TryParse(arg[1..], out int height))
                                heightOffset = height;

                            break;
                        }
                }
            }
        }

        string str = "";
        if (item.stack > 1)
            str = " (" + item.stack + ")";

        return new CenteredItemSnippet(item)
        {
            Text = "[" + item.AffixName() + str + "]",
            CheckForHover = true,
            DeleteWhole = true,
            HeightOffset = heightOffset
        };
    }

    public static string GenerateTag(Item I)
    {
        string str = "[centeritem";
        if (I.stack != 1)
            str = $"{str}/s{I.stack}";
        return $"{str}:{I.netID}]";
    }

    /// <summary>
    /// 如果是用于ModernConfig的文字绘制，图标会有所调整以适应
    /// </summary>
    public static bool ModernConfigDrawing;
}