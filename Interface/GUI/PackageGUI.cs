using ImproveGame.Interface.UIElements_Shader;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class PackageGUI : UIState
    {
        private static bool visible;
        public static bool Visible
        {
            get
            {
                if (!Main.playerInventory)
                    visible = false;
                return visible;
            }
            set => visible = value;
        }

        private Vector2 offset;
        public bool dragging;

        public SUIPanel mainPanel;
        public UITitle title;
        public BackgroundImage button;
        public SUIPanel gridPanel;
        public PackageGrid grid;

        private readonly Color background = new(44, 57, 105, 160);
        public override void OnInitialize()
        {
            Append(mainPanel = new(Color.Black, background) { HAlign = 0.5f, VAlign = 0.5f });
            mainPanel.Append(title = new("Title", 0.5f));
            mainPanel.OnMouseDown += (uie, evt) =>
            {
                if (!(button.IsMouseHovering || grid.Parent.IsMouseHovering))
                {
                    dragging = true;
                    offset = Main.MouseScreen - mainPanel.GetPPos();
                }
            };
            mainPanel.OnMouseUp += (_, _) => dragging = false;
            mainPanel.OnUpdate += (_) =>
            {
                if (dragging)
                {
                    mainPanel.SetPos(Main.MouseScreen - offset).Recalculate();
                }
            };

            mainPanel.Append(button = new(GetTexture("Close").Value) { HAlign = 1f });
            button.Height.Pixels = title.Height.Pixels;
            button.OnMouseDown += (_, _) =>
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Visible = false;
            };

            mainPanel.Append(gridPanel = new(title.background, title.background, 12, 0, false));
            gridPanel.Append(grid = new());
            gridPanel.Top.Pixels = title.Bottom() + 10;
            gridPanel.Width.Pixels = grid.Width.Pixels + gridPanel.HPadding();
            gridPanel.Height.Pixels = grid.Height.Pixels + gridPanel.VPadding();

            mainPanel.Width.Pixels = gridPanel.Width.Pixels + mainPanel.HPadding();
            mainPanel.Height.Pixels = gridPanel.Bottom() + mainPanel.VPadding() - 1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void Open(List<Item> items, string title)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true; Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            Recalculate();
        }
    }

    public class PackageGrid : UIElement
    {
        public PackageList list;
        public ZeroScrollbar scrollbar;
        public List<Item> items;

        public PackageGrid()
        {
            // 正经人谁要内边距啊. 你要内边距吗?
            SetPadding(0);

            // 隐藏子元素, 可显示的范围是计算 Padding 之后的.
            OverflowHidden = true;

            Append(list = new(new(52), new(5, 5), new(8)));

            Vector2 liseSize = list.DisplaySize;
            // 滚动条, 一定要放到滚动主体后面, 问就是 UIElement 的锅.
            Append(scrollbar = new());
            scrollbar.Left.Pixels = liseSize.X + 7;
            scrollbar.Height.Pixels = liseSize.Y;

            // 既然尺寸都已知了, 那就直接设置他们爹地的大小吧.
            Width.Pixels = scrollbar.Right() + 1;
            Height.Pixels = scrollbar.Height() + 1;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        // 如果少于原来的数量就重新计算
        public override void Update(GameTime gameTime)
        {
            if (items is not null && list.Children.Count() != items.Count)
            {
                SetInventory(items);
                scrollbar.SetView(MathF.Min(scrollbar.Height.Pixels, list.Height.Pixels), list.Height.Pixels);
                Recalculate();
            }
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (scrollbar.ViewPosition != list.Top.Pixels)
            {
                list.Top.Pixels = -scrollbar.ViewPosition;
                list.Recalculate();
            }
        }

        // 咱就是说, 这名字挺好看的.
        public void SetInventory(List<Item> items)
        {
            this.items = items;
            list.SetInventory(items);
            scrollbar.SetView(MathF.Min(scrollbar.Height.Pixels, list.Height.Pixels), list.Height.Pixels);
        }
    }

    public class PackageList : UIElement
    {
        public List<Item> items;
        private Vector2 ChildSize;
        private Vector2 Spaceing;
        private Point DisplayCount;

        public override bool ContainsPoint(Vector2 point) => Parent.GetInnerDimensions().Contains(Main.MouseScreen);
        public Vector2 DisplaySize => ChildSize * DisplayCount.ToVector2() + (DisplayCount - new Point(1, 1)).ToVector2() * Spaceing;
        public PackageList(Vector2 childSize, Point displayCount, Vector2 spaceing)
        {
            SetPadding(0);
            ChildSize = childSize;
            DisplayCount = displayCount;
            Spaceing = spaceing;
        }

        // 只绘制范围内的孩子.
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions1 = Parent.GetDimensions();
            var position1 = dimensions1.Position();
            var size1 = dimensions1.Size();
            foreach (UIElement uie in Elements)
            {
                CalculatedStyle dimensions2 = uie.GetDimensions();
                var position2 = dimensions2.Position();
                var size2 = dimensions2.Size();
                if (Collision.CheckAABBvAABBCollision(position1, size1, position2, size2))
                {
                    uie.Draw(spriteBatch);
                }
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            if (evt.Target == this)
            {
                int bannerID = ItemToBanner(Main.mouseItem);
                if (bannerID == -1)
                {
                    return;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].IsAir)
                    {
                        items.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (items[i].type == Main.mouseItem.type && items[i].stack < items[i].maxStack && ItemLoader.CanStack(items[i], Main.mouseItem))
                    {
                        int stackAvailable = items[i].maxStack - items[i].stack;
                        int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
                        Main.mouseItem.stack -= stackAddition;
                        items[i].stack += stackAddition;
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                        if (Main.mouseItem.stack <= 0)
                            Main.mouseItem.TurnToAir();
                    }
                }
                if (!Main.mouseItem.IsAir && items.Count < 200)
                {
                    items.Add(Main.mouseItem.Clone());
                    Main.mouseItem.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);
                }

            }
        }

        public void SetInventory(List<Item> items)
        {
            RemoveAllChildren();
            this.items = items;
            for (int i = 0; i < items.Count; i++)
            {
                float col = i % DisplayCount.X;
                float row = i / DisplayCount.X;
                PackageItemSlot ItemSlot;
                Append(ItemSlot = new(items, i));
                ItemSlot.SetPos(col * (ChildSize.X + Spaceing.X), row * (ChildSize.Y + Spaceing.Y));
            }
            int VCount = items.Count / DisplayCount.X + (items.Count % DisplayCount.X > 0 ? 1 : 0);
            Height.Pixels = ChildSize.Y * VCount + Spaceing.Y * (VCount - 1);
            Width.Pixels = DisplaySize.X;
        }
    }
}
