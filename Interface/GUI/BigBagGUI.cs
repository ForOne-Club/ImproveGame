using ImproveGame.Interface.UIElements;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;

namespace ImproveGame.Interface.GUI
{
    public class BigBagGUI : UIState
    {
        private static bool _visible = true;
        private Vector2 offset = Vector2.Zero;
        public bool dragging;
        public static bool Visible {
            get {
                if (!Main.playerInventory)
                    _visible = false;
                return _visible;
            }
            set => _visible = value;
        }

        public UIText title;
        public UIPanel MainPanel;
        public UIImageButton CloseButton;
        public JuItemGrid ItemGrid;

        public JuButton[] buttons = new JuButton[4];

        public void SetSuperVault(Item[] items, Vector2 SuperVaultPos) {
            title.SetText(MyUtils.GetText("SuperVault.Name"));
            title.SetSize(MyUtils.GetBigTextSize(MyUtils.GetText("SuperVault.Name")) * 0.5f);
            
            buttons[0].SetText(Lang.inter[29].Value);
            buttons[0].SetPos(0f, title.Bottom() - 10f);

            buttons[1].SetText(Lang.inter[30].Value);
            buttons[1].SetPos(buttons[0].Right() + 10f, buttons[0].Top());

            buttons[2].SetText(Lang.inter[31].Value);
            buttons[2].SetPos(buttons[1].Right() + 10f, buttons[0].Top());

            buttons[3].SetText(MyUtils.GetText("SuperVault.Sort"));
            buttons[3].SetPos(buttons[2].Right() + 10f, buttons[0].Top());

            ItemGrid.SetInventory(items);

            MainPanel.SetPos(SuperVaultPos);
            MainPanel.Width.Pixels = MainPanel.HPadding() + ItemGrid.Width();
            MainPanel.Height.Pixels = MainPanel.VPadding() + ItemGrid.Height() + ItemGrid.Top();
            _visible = Visible;
            Recalculate();
        }

        public override void OnInitialize() {
            MainPanel = new UIPanel();
            MainPanel.OnMouseDown += (evt, uie) => {
                if (!ItemGrid.IsMouseHovering && !CloseButton.IsMouseHovering) {
                    dragging = true;
                    offset = evt.MousePosition - uie.GetDimensions().Position();
                }
            };
            MainPanel.OnMouseUp += (evt, uie) => dragging = false;
            MainPanel.OnUpdate += (uie) => {
                if (dragging) {
                    uie.SetPos(Main.MouseScreen - offset);
                    uie.Recalculate();
                }
                if (!Collision.CheckAABBvAABBCollision(uie.GetDimensions().Position(), uie.GetDimensions().ToRectangle().Size(), Vector2.Zero, Main.ScreenSize.ToVector2())) {
                    uie.SetPos(Vector2.Zero);
                    uie.Recalculate();
                }
                if (uie.IsMouseHovering) {
                    Main.LocalPlayer.mouseInterface = true;
                }
            };
            Append(MainPanel);

            title = new(MyUtils.GetText("SuperVault.Name"), 0.5f, true);
            title.Top.Pixels = 10f;
            title.SetSize(MyUtils.GetBigTextSize(MyUtils.GetText("SuperVault.Name")) * 0.5f);
            MainPanel.Append(title);

            buttons[0] = new(MyUtils.GetTexture("UI/Quick").Value, Lang.inter[29].Value);
            buttons[0].SetPos(0f, title.Bottom() - 10f);
            buttons[0].OnClick += (_, _) => QuickTakeOutToPlayerInventory();
            MainPanel.Append(buttons[0]);

            buttons[1] = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[30].Value);
            buttons[1].SetPos(buttons[0].Right() + 10f, buttons[0].Top());
            buttons[1].OnClick += (_, _) => PutAll();
            MainPanel.Append(buttons[1]);

            buttons[2] = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[31].Value);
            buttons[2].SetPos(buttons[1].Right() + 10f, buttons[0].Top());
            buttons[2].OnClick += (_, _) => Replenish();
            MainPanel.Append(buttons[2]);

            buttons[3] = new(MyUtils.GetTexture("UI/Put").Value, MyUtils.GetText("SuperVault.Sort"));
            buttons[3].SetPos(buttons[2].Right() + 10f, buttons[0].Top());
            buttons[3].OnClick += (_, _) => Sort();
            MainPanel.Append(buttons[3]);

            CloseButton = new(MyUtils.GetTexture("Close")) { HAlign = 1f };
            CloseButton.Left.Set(-2, 0);
            CloseButton.OnClick += (evt, uie) => Visible = false;
            MainPanel.Append(CloseButton);

            ItemGrid = new JuItemGrid();
            ItemGrid.Top.Pixels = buttons[0].Top() + buttons[0].Height() + 10f;
            MainPanel.Append(ItemGrid);
        }

        public void Sort() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] items = ItemGrid.ItemList.items;

            // 拿出来非空非收藏的物品
            List<Item> testSort = new();
            for (int i = 0; i < items.Length; i++) {
                if (!items[i].IsAir && !items[i].favorited) {
                    testSort.Add(items[i]);
                    items[i] = new();
                }
            }

            // 优先级排序
            testSort.Sort((a, b) => {
                return -a.rare.CompareTo(b.rare) * 100 - a.stack.CompareTo(b.stack) * 10 + a.type.CompareTo(b.type);
            });

            // 放入背包
            for (int i = 0; i < testSort.Count; i++) {
                MyUtils.ItemStackToInventory(items, testSort[i], false);
            }
            Recipe.FindRecipes();
        }

        public void Replenish() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 58; i++) {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin) {
                    if (MyUtils.HasItem(BigBag, -1, inventory[i].type)) {
                        inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
                    }
                }
            }
            Recipe.FindRecipes();
        }

        public void PutAll() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 50; i++) {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin)
                    inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
            }
            Recipe.FindRecipes();
        }

        public void QuickTakeOutToPlayerInventory() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 0; i < BigBag.Length; i++) {
                if (!BigBag[i].IsAir && !BigBag[i].favorited && !BigBag[i].IsACoin) {
                    BigBag[i] = MyUtils.ItemStackToInventory(inventory, BigBag[i], false, 50);
                }
            }
            Recipe.FindRecipes();
        }

        public void Open() {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            dragging = false;
            _visible = true;
        }

        public void Close() {
            SoundEngine.PlaySound(SoundID.MenuClose);
            _visible = false;
        }
    }
}
