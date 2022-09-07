using ImproveGame.Common.Players;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.UIElements_Shader;
using System.Collections.Generic;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class BigBagGUI : UIState
    {
        private static bool visible = true;
        private Vector2 offset = Vector2.Zero;
        public bool dragging;
        public static bool Visible
        {
            get
            {
                if (!Main.playerInventory) visible = false;
                return visible;
            }
            set => visible = value;
        }

        public UITitle title;
        public SUIPanel MainPanel;
        public BackgroundImage CloseButton;
        public PictureButton[] buttons = new PictureButton[4];
        // 物品列表
        public ModItemGrid ItemGrid;
        public Checkbox[] checkbox = new Checkbox[3];

        private BackgroundImage BackButton;
        public void AddBackButton()
        {
            if (MainPanel.HasChild(BackButton))
                MainPanel.RemoveChild(BackButton);
            MainPanel.Append(BackButton = new(GetTexture("Bank").Value) { HAlign = 1f });
            BackButton.OnMouseDown += (uie, evt) =>
            {
                ItemGrid.SetInventory(Main.LocalPlayer.GetModPlayer<DataPlayer>().SuperVault);
                MainPanel.RemoveChild(BackButton);
            };
            BackButton.Height.Pixels = CloseButton.Height.Pixels;
            BackButton.Left.Pixels = -BackButton.Width.Pixels - 10;
        }

        private readonly Color background = new(44, 57, 105, 160);
        public override void OnInitialize()
        {
            Append(MainPanel = new(Color.Black, background) { HAlign = 0.5f, VAlign = 0.5f });
            MainPanel.OnMouseDown += (evt, uie) =>
            {
                if (!ItemGrid.IsMouseHovering && !CloseButton.IsMouseHovering)
                {
                    dragging = true;
                    offset = evt.MousePosition - uie.GetPPos();
                }
            };
            MainPanel.OnMouseUp += (evt, uie) => dragging = false;
            MainPanel.OnUpdate += (uie) =>
            {
                if (dragging)
                {
                    uie.SetPos(Main.MouseScreen - offset).Recalculate();
                }
                if (!GetDimensions().ToRectangle().Intersects(uie.GetDimensions().ToRectangle()))
                {
                    uie.SetPos(Vector2.Zero).Recalculate();
                }
            };

            MainPanel.Append(title = new(GetText("SuperVault.Name"), 0.5f) { HAlign = 0f });

            MainPanel.Append(CloseButton = new(GetTexture("Close").Value) { HAlign = 1f });
            CloseButton.Height.Pixels = title.Height();
            CloseButton.OnMouseDown += (evt, uie) => Visible = false;

            MainPanel.Append(checkbox[0] = new Checkbox("参与合成", 0.8f));
            checkbox[0].Top.Pixels = title.Bottom() + 10f;

            MainPanel.Append(checkbox[1] = new Checkbox("智能拾取", 0.8f));
            checkbox[1].Left.Pixels = checkbox[0].Right() + 10;
            checkbox[1].Top.Pixels = checkbox[0].Top();

            MainPanel.Append(checkbox[2] = new Checkbox("背包溢出自动拾取", 0.8f));
            checkbox[2].Left.Pixels = checkbox[1].Right() + 10;
            checkbox[2].Top.Pixels = checkbox[0].Top();

            buttons[0] = new(MyUtils.GetTexture("UI/Quick").Value, Lang.inter[29].Value);
            buttons[0].SetText(Lang.inter[29].Value);
            buttons[0].SetPos(0f, checkbox[0].Bottom() + 10f);
            buttons[0].OnMouseDown += (_, _) => QuickTakeOutToPlayerInventory();
            MainPanel.Append(buttons[0]);

            buttons[1] = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[30].Value);
            buttons[1].SetText(Lang.inter[30].Value);
            buttons[1].SetPos(buttons[0].Right() + 10f, buttons[0].Top());
            buttons[1].OnMouseDown += (_, _) => PutAll();
            MainPanel.Append(buttons[1]);

            buttons[2] = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[31].Value);
            buttons[2].SetText(Lang.inter[31].Value);
            buttons[2].SetPos(buttons[1].Right() + 10f, buttons[0].Top());
            buttons[2].OnMouseDown += (_, _) => Replenish();
            MainPanel.Append(buttons[2]);

            buttons[3] = new(MyUtils.GetTexture("UI/Put").Value, MyUtils.GetText("SuperVault.Sort"));
            buttons[3].SetText(GetText("SuperVault.Sort"));
            buttons[3].SetPos(buttons[2].Right() + 10f, buttons[0].Top());
            buttons[3].OnMouseDown += (_, _) => Sort();
            MainPanel.Append(buttons[3]);

            ItemGrid = new ModItemGrid();
            ItemGrid.Top.Pixels = buttons[0].Top() + buttons[0].Height() + 10f;
            ItemGrid.ItemList.OnMouseDownSlot += NetSyncItem;
            MainPanel.SetSizeInside(ItemGrid.Width(), ItemGrid.Height() + ItemGrid.Top()).Recalculate();
            MainPanel.Append(ItemGrid);
        }

        // 点击操作，将物品发送给服务器（因为像药水袋和旗帜盒这俩左键是不改stack的，所以这来个同步）
        private void NetSyncItem(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && listeningElement is ArrayItemSlot itemSlot)
            {
                NetBigBag.SendSlot(itemSlot.index, itemSlot.Item, Main.myPlayer, -1, -1);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (MainPanel.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (MainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: BigBagGUI");
            }
        }

        public void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            dragging = false;
            visible = true;
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            visible = false;
        }

        public void Sort()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] items = ItemGrid.ItemList.items;

            // 拿出来非空非收藏的物品
            List<Item> testSort = new();
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].IsAir && !items[i].favorited)
                {
                    testSort.Add(items[i]);
                    items[i] = new();
                }
            }

            // 优先级排序
            testSort.Sort((a, b) =>
            {
                return -a.rare.CompareTo(b.rare) * 100 - a.stack.CompareTo(b.stack) * 10 + a.type.CompareTo(b.type);
            });

            // 放入背包
            for (int i = 0; i < testSort.Count; i++)
            {
                MyUtils.ItemStackToInventory(items, testSort[i], false);
            }
            Recipe.FindRecipes();
        }

        public void Replenish()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 58; i++)
            {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin)
                {
                    if (MyUtils.HasItem(BigBag, -1, inventory[i].type))
                    {
                        inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
                    }
                }
            }
            Recipe.FindRecipes();
        }

        public void PutAll()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 50; i++)
            {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin)
                    inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
            }
            Recipe.FindRecipes();
        }

        public void QuickTakeOutToPlayerInventory()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 0; i < BigBag.Length; i++)
            {
                if (!BigBag[i].IsAir && !BigBag[i].favorited && !BigBag[i].IsACoin)
                {
                    BigBag[i] = MyUtils.ItemStackToInventory(inventory, BigBag[i], false, 50);
                }
            }
            Recipe.FindRecipes();
        }
    }
}
