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
        public SUIPanel mainPanel;
        public SUIFork fork;
        public SUIPictureButton[] buttons = new SUIPictureButton[4];
        // 物品列表
        public ModItemGrid ItemGrid;
        public SUISwitch[] suiSwitch = new SUISwitch[3];

        public override void OnInitialize()
        {
            // 主面板
            Append(mainPanel = new(UIColor.Default.PanelBorder, UIColor.Default.PanelBackground)
            {
                Shaded = true,
                Draggable = true
            });

            // 标题
            mainPanel.Append(title = new SUITitle(GetText("SuperVault.Name"), 0.5f));

            // Fork
            mainPanel.Append(fork = new SUIFork(30)
            {
                HAlign = 1f,
                Height = new StyleDimension(title.Height.Pixels, 0)
            });
            fork.OnMouseDown += (evt, uie) => Close();

            UIPlayerSetting setting = Main.LocalPlayer.GetModPlayer<UIPlayerSetting>();

            // 开关
            Vector2 SwitchInterval = new(10, 10);
            mainPanel.Append(suiSwitch[0] = new SUISwitch(() => setting.SuperVault_HeCheng, state =>
            {
                setting.SuperVault_HeCheng = state;
                Recipe.FindRecipes();
            }, GetText("SuperVault.Synthesis"), 0.8f)
            {
                First = true,
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Vertical,
                Interval = SwitchInterval
            });

            mainPanel.Append(suiSwitch[1] = new SUISwitch(() => setting.SuperVault_SmartGrab, state =>
            {
                setting.SuperVault_SmartGrab = state;
            }, GetText("SuperVault.SmartPickup"), 0.8f)
            {
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Horizontal,
                Interval = SwitchInterval
            });

            mainPanel.Append(suiSwitch[2] = new SUISwitch(() => setting.SuperVault_OverflowGrab, (bool state) =>
            {
                setting.SuperVault_OverflowGrab = state;
            }, GetText("SuperVault.OverflowPickup"), 0.8f)
            {
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Horizontal,
                Interval = SwitchInterval
            });

            // 按钮
            Vector2 ButtonInterval = new(10, 8);
            mainPanel.Append(buttons[0] = new(GetTexture("UI/Quick").Value, Lang.inter[29].Value)
            {
                First = true,
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Vertical,
                Interval = ButtonInterval
            });
            buttons[0].SetText(Lang.inter[29].Value);
            buttons[0].OnMouseDown += (_, _) => QuickTakeOutToPlayerInventory();

            mainPanel.Append(buttons[1] = new(GetTexture("UI/Put").Value, Lang.inter[30].Value)
            {
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Horizontal,
                Interval = ButtonInterval
            });
            buttons[1].SetText(Lang.inter[30].Value);
            buttons[1].OnMouseDown += (_, _) => PutAll();

            mainPanel.Append(buttons[2] = new(GetTexture("UI/Put").Value, Lang.inter[31].Value)
            {
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Horizontal,
                Interval = ButtonInterval
            });
            buttons[2].SetText(Lang.inter[31].Value);
            buttons[2].OnMouseDown += (_, _) => Replenish();

            mainPanel.Append(buttons[3] = new(GetTexture("UI/Put").Value, GetText("SuperVault.Sort"))
            {
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Horizontal,
                Interval = ButtonInterval
            });
            buttons[3].SetText(GetText("SuperVault.Sort"));
            buttons[3].OnMouseDown += (_, _) => Sort();

            // Inventory 滚动视图
            mainPanel.Append(ItemGrid = new ModItemGrid()
            {
                First = true,
                Relative = true,
                Mode = BaseUIEs.RelativeUIE.RelativeMode.Vertical,
                Interval = ButtonInterval
            });
            ItemGrid.ItemList.OnMouseDownSlot += NetSyncItem;
            mainPanel.SetSizeInside(ItemGrid.Width.Pixels, ItemGrid.Bottom());
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
            if (mainPanel.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ImproveGame: BigBagGUI");

            if (mainPanel.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
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
