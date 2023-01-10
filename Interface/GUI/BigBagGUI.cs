using ImproveGame.Common.Configs;
using ImproveGame.Common.Packets;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using ImproveGame.Interface.UIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class BigBagGUI : UIState, IUseEventTrigger
    {
        private static bool _visible = true;

        public static bool Visible
        {
            get
            {
                if (!Main.playerInventory)
                    _visible = false;
                return _visible;
            }
            set => _visible = value;
        }

        // 主面板
        public SUIPanel MainPanel;

        // 标题面板
        private SUIPanel TitlePanel;

        // 内容面板
        private View ContentPanel;

        // 标题
        private SUITitle Title;

        // 关闭按钮
        private SUICross Cross;

        // 控制开关
        private SUISwitch RecipesSwitch, SmartGrabSwitch, AutoGrabSwitch;

        // 按钮
        private SUIPictureButton QuickButton, PutButton, ReplenishButton, SortButton;

        // 物品列表
        public ModItemGrid ItemGrid;

        public override void OnInitialize()
        {
            UIPlayerSetting setting = Main.LocalPlayer.GetModPlayer<UIPlayerSetting>();
            // 主面板
            MainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                Shaded = true,
                Draggable = true
            };
            MainPanel.SetPadding(0f);
            MainPanel.Join(this);

            TitlePanel = new SUIPanel(UIColor.PanelBorder, UIColor.TitleBg2)
            {
                Width = new StyleDimension(0f, 1f),
                Height = new StyleDimension(50f, 0f),
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(10f, 10f, 0f, 0f),
                Relative = RelativeMode.Vertical
            };
            TitlePanel.SetPadding(0f);
            TitlePanel.Join(MainPanel);

            // 标题
            Title = new SUITitle(GetText("SuperVault.Name"), 0.5f)
            {
                VAlign = 0.5f,
                background = Color.Transparent
            };
            Title.Join(TitlePanel);

            // Cross
            Cross = new SUICross(24)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Height = new StyleDimension(0f, 1f),
                beginBg = UIColor.TitleBg2,
                endBg = UIColor.TitleBg2,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(0f, 10f, 0f, 0f)
            };
            Cross.OnMouseDown += (_, _) => Close();
            Cross.Join(TitlePanel);

            ContentPanel = new View
            {
                Relative = RelativeMode.Vertical,
            };
            ContentPanel.SetPadding(12f, 10f, 11f, 12f);
            ContentPanel.Join(MainPanel);

            // 开关
            Vector2 switchSpacing = new Vector2(10, 10);
            RecipesSwitch = new SUISwitch(() => setting.SuperVault_HeCheng,
                state =>
                {
                    setting.SuperVault_HeCheng = state;
                    Recipe.FindRecipes();
                }, GetText("SuperVault.Synthesis"), 0.8f)
            {
                First = true,
                Relative = RelativeMode.Vertical,
                Spacing = switchSpacing
            };
            RecipesSwitch.Join(ContentPanel);

            SmartGrabSwitch = new SUISwitch(() => setting.SuperVault_SmartGrab,
                state => setting.SuperVault_SmartGrab = state,
                GetText("SuperVault.SmartPickup"), 0.8f)
            {
                Relative = RelativeMode.Horizontal,
                Spacing = switchSpacing
            };
            SmartGrabSwitch.Join(ContentPanel);

            AutoGrabSwitch = new SUISwitch(() => setting.SuperVault_OverflowGrab,
                state => setting.SuperVault_OverflowGrab = state,
                GetText("SuperVault.OverflowPickup"), 0.8f)
            {
                Relative = RelativeMode.Horizontal,
                Spacing = switchSpacing
            };
            AutoGrabSwitch.Join(ContentPanel);

            // 按钮
            Vector2 buttonSpacing = new Vector2(10, 8);
            QuickButton = new SUIPictureButton(GetTexture("UI/Quick").Value, Lang.inter[29].Value)
            {
                First = true,
                Relative = RelativeMode.Vertical,
                Spacing = buttonSpacing
            };
            QuickButton.SetText(Lang.inter[29].Value);
            QuickButton.OnMouseDown += (_, _) => QuickTakeOutToPlayerInventory();
            QuickButton.Join(ContentPanel);

            PutButton = new SUIPictureButton(GetTexture("UI/Put").Value, Lang.inter[30].Value)
            {
                Relative = RelativeMode.Horizontal,
                Spacing = buttonSpacing
            };
            PutButton.SetText(Lang.inter[30].Value);
            PutButton.OnMouseDown += (_, _) => PutAll();
            PutButton.Join(ContentPanel);

            ReplenishButton = new SUIPictureButton(GetTexture("UI/Put").Value, Lang.inter[31].Value)
            {
                Relative = RelativeMode.Horizontal,
                Spacing = buttonSpacing
            };
            ReplenishButton.SetText(Lang.inter[31].Value);
            ReplenishButton.OnMouseDown += (_, _) => Replenish();
            ReplenishButton.Join(ContentPanel);

            SortButton = new SUIPictureButton(GetTexture("UI/Put").Value, GetText("SuperVault.Sort"))
            {
                Relative = RelativeMode.Horizontal,
                Spacing = buttonSpacing
            };
            SortButton.SetText(GetText("SuperVault.Sort"));
            SortButton.OnMouseDown += (_, _) => Sort();
            SortButton.Join(ContentPanel);

            // Inventory 滚动视图
            ItemGrid = new ModItemGrid
            {
                First = true,
                Relative = RelativeMode.Vertical,
                Spacing = new Vector2(10, 15)
            };
            ItemGrid.ItemList.OnMouseDownSlot += NetSyncItem;
            ItemGrid.Join(ContentPanel);
            ContentPanel.SetInnerPixels(ItemGrid.Width.Pixels, ItemGrid.Bottom());
            MainPanel.SetInnerPixels(ContentPanel.Width.Pixels, ContentPanel.Bottom());
        }

        /// <summary>
        /// 点击操作，将物品发送给服务器（因为像药水袋和旗帜盒这俩左键是不改stack的，所以这来个同步）
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        private static void NetSyncItem(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient || listeningElement is not BigBagItemSlot itemSlot)
            {
                return;
            }

            var packet = BigBagSlotPacket.Get(itemSlot.Item, Main.myPlayer, itemSlot.Index);
            packet.Send(runLocally: false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (MainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: BigBagGUI");
                Main.LocalPlayer.mouseInterface = true;
            }

            if (Math.Abs(ContentPanel.GetInnerSizePixels().Y - ItemGrid.Bottom()) > 0.000000001)
            {
                ContentPanel.SetInnerPixels(ItemGrid.Width.Pixels, ItemGrid.Bottom());
                ContentPanel.Recalculate();
            }

            if ((Math.Abs(MainPanel.Height.Pixels - ContentPanel.Bottom()) < 0.000000001))
            {
                return;
            }

            MainPanel.SetInnerPixels(ContentPanel.Width.Pixels, ContentPanel.Bottom());
            MainPanel.Recalculate();
        }

        public void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            _visible = true;
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            _visible = false;
            AdditionalConfig.Save();
        }

        private void Sort()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] items = ItemGrid.ItemList.items;

            // 拿出来非空非收藏的物品
            List<Item> testSort = new();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].IsAir || items[i].favorited)
                {
                    continue;
                }

                testSort.Add(items[i]);
                items[i] = new Item();
            }

            // 优先级排序
            testSort.Sort((a, b) =>
                -a.rare.CompareTo(b.rare) * 100 + a.type.CompareTo(b.type) * 10 - a.stack.CompareTo(b.stack));

            // 放入背包
            foreach (var item in testSort)
            {
                ItemStackToInventory(items, item, false);
            }

            Recipe.FindRecipes();
        }

        private void Replenish()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] bigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 58; i++)
            {
                if (inventory[i].IsAir || inventory[i].favorited || inventory[i].IsACoin)
                {
                    continue;
                }

                if (HasItem(bigBag, -1, inventory[i].type))
                {
                    inventory[i] = ItemStackToInventory(bigBag, inventory[i], false);
                }
            }

            Recipe.FindRecipes();
        }

        private void PutAll()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] bigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 50; i++)
            {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin)
                    inventory[i] = ItemStackToInventory(bigBag, inventory[i], false);
            }

            Recipe.FindRecipes();
        }

        private void QuickTakeOutToPlayerInventory()
        {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] bigBag = ItemGrid.ItemList.items;
            for (int i = 0; i < bigBag.Length; i++)
            {
                if (!bigBag[i].IsAir && !bigBag[i].favorited && !bigBag[i].IsACoin)
                {
                    bigBag[i] = ItemStackToInventory(inventory, bigBag[i], false, 50);
                }
            }

            Recipe.FindRecipes();
        }

        public bool ToPrimary(UIElement target) => target != this;

        public bool CanOccupyCursor(UIElement target)
        {
            return (target != this && MainPanel.IsMouseHovering) || MainPanel.KeepPressed;
        }
    }
}