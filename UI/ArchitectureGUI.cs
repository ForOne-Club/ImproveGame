using ImproveGame.Content.Items;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.UIElements;

namespace ImproveGame.UI
{
    public class ArchitectureGUI : BaseBody
    {
        private static bool _visible;

        public static bool Visible
        {
            get
            {
                return _visible && Main.playerInventory && Main.LocalPlayer.HeldItem is not null;
            }
            private set => _visible = value;
        }

        public override bool Enabled { get => Visible; set => Visible = value; }

        public override bool CanSetFocusTarget(UIElement target)
            => (target != this && basePanel.IsMouseHovering) || basePanel.IsLeftMousePressed;

        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        public Item CurrentItem;
        public CreateWand CurrentWand => CurrentItem.ModItem as CreateWand;

        public Dictionary<string, ModItemSlot> ItemSlot => itemSlot;

        private static bool PrevMouseRight;
        private static bool HoveringOnSlots;

        private SUIPanel basePanel;
        private Dictionary<string, ModItemSlot> itemSlot = new();
        private UIText materialTitle;
        private ModIconTextButton styleButton;

        public override void OnInitialize() {
            panelLeft = 590f;
            panelTop = 120f;
            panelHeight = 190f;
            panelWidth = 190f;

            Append(basePanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
            {
                Shaded = true,
                ShadowThickness = UIStyle.ShadowThicknessThinnerer,
                Draggable = true,
                Left = {Pixels = panelLeft},
                Top = {Pixels = panelTop},
                Width = {Pixels = panelWidth},
                Height = {Pixels = panelHeight},
            });

            // 排布
            // O O O
            // O O O
            // O 切换
            // 排布如上
            const float slotFirst = 0f;
            const float slotSecond = 60f;
            const float slotThird = 120f;
            itemSlot = new() {
                [nameof(CreateWand.Block)] = CreateItemSlot(slotFirst, slotFirst, nameof(CreateWand.Block),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile]), 
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Block), item.Clone()),
                    () => GetText($"Architecture.{nameof(CreateWand.Block)}")),

                [nameof(CreateWand.Wall)] = CreateItemSlot(slotSecond, slotFirst, nameof(CreateWand.Wall),
                    (i, item) => SlotPlace(i, item) || item.createWall > -1,
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Wall), item.Clone()),
                    () => GetText($"Architecture.{nameof(CreateWand.Wall)}")),

                [nameof(CreateWand.Platform)] = CreateItemSlot(slotThird, slotFirst, nameof(CreateWand.Platform),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Platforms[item.createTile]),
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Platform), item.Clone()),
                    () => GetText($"Architecture.{nameof(CreateWand.Platform)}")),

                [nameof(CreateWand.Torch)] = CreateItemSlot(slotFirst, slotSecond, nameof(CreateWand.Torch),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Torch[item.createTile]),
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Torch), item.Clone()),
                    () => Lang.GetItemNameValue(ItemID.Torch)),

                [nameof(CreateWand.Chair)] = CreateItemSlot(slotSecond, slotSecond, nameof(CreateWand.Chair),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Chairs),
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Chair), item.Clone()),
                    () => GetText($"Architecture.{nameof(CreateWand.Chair)}")),

                [nameof(CreateWand.Workbench)] = CreateItemSlot(slotThird, slotSecond, nameof(CreateWand.Workbench),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.WorkBenches),
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Workbench), item.Clone()),
                    () => Lang.GetItemNameValue(ItemID.WorkBench)),

                [nameof(CreateWand.Bed)] = CreateItemSlot(slotFirst, slotThird, nameof(CreateWand.Bed),
                    (i, item) => SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Beds),
                    (item, _) => CurrentWand.SetItem(nameof(CreateWand.Bed), item.Clone()),
                    () => Lang.GetItemNameValue(ItemID.Bed)),
            };

            // 头顶大字
            materialTitle = new("Materials", 0.5f, large: true) {
                HAlign = 0.5f
            };
            materialTitle.Left.Set(0, 0f);
            materialTitle.Top.Set(-40, 0f);
            materialTitle.Width.Set(panelWidth, 0f);
            materialTitle.Height.Set(40, 0f);
            basePanel.Append(materialTitle);

            // 房屋样式修改按钮
            styleButton = new(Language.GetText("Mods.ImproveGame.Common.Switch"), Color.White, "Images/UI/DisplaySlots_5");
            styleButton.Left.Set(slotSecond, 0f);
            styleButton.Top.Set(slotThird, 0f);
            styleButton.Width.Set(104f, 0f);
            styleButton.Height.Set(42f, 0f);
            styleButton.OnLeftClick += (_, _) => CreateWand.NextStyle();
            basePanel.Append(styleButton);
        }

        public ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, Func<Item, Item, bool> canPlace = null, Action<Item, bool> onItemChanged = null, Func<string> emptyText = null) {
            ModItemSlot slot = MyUtils.CreateItemSlot(x, y, iconTextureName, 0.85f, canPlace, onItemChanged, emptyText, basePanel, "Architecture");
            slot.OnUpdate += _ => HoveringOnSlots |= slot.IsMouseHovering;
            return slot;
        }

        // 主要是可拖动和一些判定吧
        public override void Update(GameTime gameTime) {
            if (CurrentItem?.ModItem is not CreateWand) {
                Close();
                return;
            }

            HoveringOnSlots = false;

            base.Update(gameTime);

            if (!Main.playerInventory) {
                Close();
                return;
            }

            // 右键点击空白直接关闭
            if (Main.mouseRight && !PrevMouseRight && basePanel.IsMouseHovering && !HoveringOnSlots) {
                Close();
                return;
            }

            PrevMouseRight = Main.mouseRight;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            Player player = Main.LocalPlayer;

            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen)) {
                player.mouseInterface = true;
            }
        }

        /// <summary>
        /// 更新GUI物品槽与物品的同步
        /// </summary>
        /// <param name="createWand">建筑魔杖<see cref="CreateWand"/>实例</param>
        public void RefreshSlots(CreateWand createWand) {
            itemSlot[nameof(CreateWand.Block)].Item = createWand.Block;
            itemSlot[nameof(CreateWand.Wall)].Item = createWand.Wall;
            itemSlot[nameof(CreateWand.Platform)].Item = createWand.Platform;
            itemSlot[nameof(CreateWand.Torch)].Item = createWand.Torch;
            itemSlot[nameof(CreateWand.Chair)].Item = createWand.Chair;
            itemSlot[nameof(CreateWand.Workbench)].Item = createWand.Workbench;
            itemSlot[nameof(CreateWand.Bed)].Item = createWand.Bed;
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(CreateWand wand) {
            OperateInventory(true);
            PrevMouseRight = true; // 防止一打开就关闭
            basePanel.Dragging = false;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentItem = wand.Item;
            RefreshSlots(CurrentWand);

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            materialTitle.SetText(Language.GetText("Mods.ImproveGame.Architecture.Materials"));
            styleButton.SetText(Language.GetText("Mods.ImproveGame.Common.Switch"), 1f, Color.White);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            CurrentItem = new Item();
            Visible = false;
            PrevMouseRight = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
