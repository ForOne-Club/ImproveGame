using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace ImproveGame.Content.UI
{
    public class MyItemSlot : UIElement
    {
        public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
        public Asset<Texture2D> texture
        {
            get
            {
                Main.instance.LoadItem(item.type);
                return TextureAssets.Item[item.type];
            }
        }
        public Item[] SuperVault;
        public int index;
        public Item item
        {
            get
            {
                return SuperVault[index];
            }
            set
            {
                SuperVault[index] = value;
            }
        }

        public UIText text;

        public MyItemSlot(Item[] SuperVault, int index)
        {
            this.SuperVault = SuperVault;
            this.index = index;
            Width.Set(backgroundTexture.Width, 0f);
            Height.Set(backgroundTexture.Height, 0f);

            text = new UIText("")
            {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            Append(text);

            UIText text2 = new UIText((index + 1).ToString(), 0.75f)
            {
                VAlign = 0.15f
            };
            text2.Left.Set(0, 0.15f);
            Append(text2);
        }

        // 左键点击事件
        public override void MouseDown(UIMouseEvent evt)
        {
            Player player = Main.LocalPlayer;
            if (NotItem(Main.mouseItem) && !NotItem(item))
            {
                // Ctrl 放入垃圾桶
                if (Main.keyState.IsKeyDown(Keys.LeftControl) || PlayerInput.GetPressedKeys().Contains(Keys.RightControl))
                {
                    player.trashItem = item;
                    item = new Item();
                }
                else
                {
                    Main.mouseItem = item;
                    item = new Item();
                }
            }
            else if (NotItem(item) && !NotItem(Main.mouseItem))
            {
                item = Main.mouseItem;
                Main.mouseItem = new Item();
            }
            else if (!NotItem(Main.mouseItem) && !NotItem(item))
            {
                if (Main.mouseItem.type == item.type && item.stack < item.maxStack)
                {
                    if (item.stack + Main.mouseItem.stack < Main.mouseItem.maxStack)
                    {
                        item.stack += Main.mouseItem.stack;
                        Main.mouseItem = new Item();
                    }
                    else
                    {
                        Main.mouseItem.stack -= item.maxStack - item.stack;
                        item.stack = item.maxStack;
                    }
                }
                else
                {
                    Item mouseItem = Main.mouseItem;
                    Main.mouseItem = item;
                    item = mouseItem;
                }
            }
            item.favorited = false;
        }

        /// <summary>
        /// 右键持续按住物品，向鼠标物品堆叠数量
        /// </summary>
        public void RightMouseKeepPressItem()
        {
            if (Main.mouseItem.type == item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
            {
                Main.mouseItem.stack++;
                item.stack--;
            }
            else if (NotItem(Main.mouseItem) && !NotItem(item) && item.maxStack > 1)
            {
                Main.mouseItem = new Item(item.type, 1);
                item.stack--;
            }
            if (item.type != ItemID.None && item.stack < 1)
            {
                item = new Item();
            }
        }

        // 鼠标右键按住项目计时器，-1代表不进行计时。
        public int rightMouseDownTimer = -1;
        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseDown(UIMouseEvent evt)
        {
            rightMouseDownTimer = 0;
            RightMouseKeepPressItem();
        }

        /// <summary>
        /// 鼠标右键放开
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseUp(UIMouseEvent evt)
        {
            rightMouseDownTimer = -1;
        }

        /// <summary>
        /// Update 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (rightMouseDownTimer >= 60)
            {
                RightMouseKeepPressItem();
            }
            else if (rightMouseDownTimer >= 30 && rightMouseDownTimer % 3 == 0)
            {
                RightMouseKeepPressItem();
            }
            else if (rightMouseDownTimer >= 15 && rightMouseDownTimer % 6 == 0)
            {
                RightMouseKeepPressItem();
            }
            if (rightMouseDownTimer != -1)
            {
                rightMouseDownTimer++;
            }
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="sb"></param>
        protected override void DrawSelf(SpriteBatch sb)
        {
            // 按下 Ctrl 改变鼠标指针外观
            if (ContainsPoint(Main.MouseScreen) && !NotItem(item) && (Main.keyState.IsKeyDown(Keys.LeftControl) ||
                PlayerInput.GetPressedKeys().Contains(Keys.RightControl)))
            {
                Main.cursorOverride = 5;
            }
            // 绘制背景框
            CalculatedStyle dimensions = GetDimensions();
            sb.Draw(backgroundTexture, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);

            // 绘制物品
            if (!NotItem(item))
            {
                Rectangle rectangle;
                if (Main.itemAnimations[item.type] == null)
                {
                    rectangle = texture.Frame(1, 1, 0, 0);
                }
                else
                {
                    rectangle = Main.itemAnimations[item.type].GetFrame(texture.Value);
                }
                float textureSize = 30f;
                float size = (rectangle.Width > textureSize || rectangle.Height > textureSize) ?
                    rectangle.Width > rectangle.Height ? textureSize / rectangle.Width : textureSize / rectangle.Height :
                    1f;
                sb.Draw(texture.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), item.GetAlpha(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
                sb.Draw(texture.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), item.GetColor(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
            }
            // 物品信息
            if (IsMouseHovering)
            {
                Main.hoverItemName = item.Name;
                Main.HoverItem = item.Clone();
            }
            text.SetText(NotItem(item) || item.stack <= 1 ? "" : item.stack.ToString(), 0.8f, false);
            text.Recalculate();
        }

        /// <summary>
        /// 不是物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool NotItem(Item item)
        {
            return (item.type == ItemID.None || item.stack < 0);
        }
    }
}
