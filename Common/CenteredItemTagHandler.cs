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

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            float num = 1f;
            float num2 = 1f;
            if (Main.netMode is not NetmodeID.Server && !Main.dedServ)
            {
                Main.instance.LoadItem(_item.type);
                Texture2D value = TextureAssets.Item[_item.type].Value;
                if (Main.itemAnimations[_item.type] != null)
                    Main.itemAnimations[_item.type].GetFrame(value);
                else
                    value.Frame();
            }

            num2 *= scale;
            num *= num2;
            if (num > 0.75f)
                num = 0.75f;

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                float inventoryScale = Main.inventoryScale;
                Main.inventoryScale = scale * num;
                var offset = new Vector2(12, 16 - HeightOffset);
                ItemSlot.Draw(spriteBatch, ref _item, 14, position - offset * scale * num, Color.White);
                Main.inventoryScale = inventoryScale;
            }

            size = new Vector2(28) * scale;
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
}
