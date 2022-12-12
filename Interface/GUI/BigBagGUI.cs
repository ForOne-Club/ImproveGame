using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Common.Packets;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using ImproveGame.Interface.UIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class BigBagGUI : UIState
    {
        private static bool visible = true;
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

        public SUITitle title;
        public SUIPanel MainPanel;
        public SUIFork fork;
        public SUIPictureButton[] buttons = new SUIPictureButton[4];
        // 物品列表
        public ModItemGrid ItemGrid;
        public SUICheckbox[] checkbox = new SUICheckbox[3];

        public override void OnInitialize()
        {
            Append(MainPanel = new(UIColor.Default.PanelBorder, UIColor.Default.PanelBackground) { Draggable = true });

            MainPanel.Append(title = new(GetText("SuperVault.Name"), 0.5f));

            MainPanel.Append(fork = new(30) { HAlign = 1f });
            fork.Height.Pixels = title.Height();
            fork.OnMouseDown += (evt, uie) => Close();

            MainPanel.Append(checkbox[0] = new SUICheckbox(() =>
            {
                return Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng;
            }, (bool state) =>
            {
                Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng = state;
                Recipe.FindRecipes();
            }, GetText("SuperVault.Synthesis"), 0.8f));
            checkbox[0].Top.Pixels = title.Bottom() + 10f;

            MainPanel.Append(checkbox[1] = new SUICheckbox(() =>
            {
                return Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_SmartGrab;
            }, (bool state) =>
            {
                Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_SmartGrab = state;
            }, GetText("SuperVault.SmartPickup"), 0.8f));
            checkbox[1].Left.Pixels = checkbox[0].Right() + 10;
            checkbox[1].Top.Pixels = checkbox[0].Top();

            MainPanel.Append(checkbox[2] = new SUICheckbox(() =>
            {
                return Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_OverflowGrab;
            }, (bool state) =>
            {
                Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_OverflowGrab = state;
            }, GetText("SuperVault.OverflowPickup"), 0.8f));
            checkbox[2].Left.Pixels = checkbox[1].Right() + 10;
            checkbox[2].Top.Pixels = checkbox[0].Top();

            buttons[0] = new(GetTexture("UI/Quick").Value, Lang.inter[29].Value);
            buttons[0].SetText(Lang.inter[29].Value);
            buttons[0].SetPos(0f, checkbox[0].Bottom() + 10f);
            buttons[0].OnMouseDown += (_, _) => QuickTakeOutToPlayerInventory();
            MainPanel.Append(buttons[0]);

            buttons[1] = new(GetTexture("UI/Put").Value, Lang.inter[30].Value);
            buttons[1].SetText(Lang.inter[30].Value);
            buttons[1].SetPos(buttons[0].Right() + 10f, buttons[0].Top());
            buttons[1].OnMouseDown += (_, _) => PutAll();
            MainPanel.Append(buttons[1]);

            buttons[2] = new(GetTexture("UI/Put").Value, Lang.inter[31].Value);
            buttons[2].SetText(Lang.inter[31].Value);
            buttons[2].SetPos(buttons[1].Right() + 10f, buttons[0].Top());
            buttons[2].OnMouseDown += (_, _) => Replenish();
            MainPanel.Append(buttons[2]);

            buttons[3] = new(GetTexture("UI/Put").Value, GetText("SuperVault.Sort"));
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
            if (Main.netMode == NetmodeID.MultiplayerClient && listeningElement is ItemSlot_BigBag itemSlot)
            {
                var packet = BigBagSlotPacket.Get(itemSlot.Item, Main.myPlayer, itemSlot.index);
                packet.Send(runLocally: false);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (MainPanel.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ImproveGame: BigBagGUI");

            if (MainPanel.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 shadowWidth = new(40);
            Vector2 pos = MainPanel.GetDimensions().Position() - shadowWidth;
            Vector2 size = MainPanel.GetDimensions().Size() + shadowWidth * 2;
            PixelShader.DrawShadow(pos, size, 10, new Color(0, 0, 0, 0.25f), shadowWidth.X);
        }

        public void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            visible = true;
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            visible = false;
            AdditionalConfig.Save();
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
                return -a.rare.CompareTo(b.rare) * 100 + a.type.CompareTo(b.type) * 10 - a.stack.CompareTo(b.stack);
            });

            // 放入背包
            for (int i = 0; i < testSort.Count; i++)
            {
                ItemStackToInventory(items, testSort[i], false);
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
                    if (HasItem(BigBag, -1, inventory[i].type))
                    {
                        inventory[i] = ItemStackToInventory(BigBag, inventory[i], false);
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
                    inventory[i] = ItemStackToInventory(BigBag, inventory[i], false);
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
                    BigBag[i] = ItemStackToInventory(inventory, BigBag[i], false, 50);
                }
            }
            Recipe.FindRecipes();
        }
    }
}
