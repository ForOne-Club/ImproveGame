using ImproveGame.Common.Animations;
using ImproveGame.Common.Packets.NetStorager;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using PinyinNet;
using Terraria.GameInput;

namespace ImproveGame.Interface.ExtremeStorage
{
    public class ExtremeStorageGUI : ViewBody, ISidedView
    {
        // 用于捕获分辨率变化，以便 Recalculate 并且重新计算位置
        private class CaptureResolutionChange : ILoadable
        {
            public void Load(Mod mod)
            {
                Main.OnResolutionChanged += _ => _recalculateNextTick = true;
            }

            public void Unload() { }
        }

        public override bool Display { get => !Main.recBigList; set { } }

        public static bool Visible => SidedEventTrigger.IsOpened(UISystem.Instance.ExtremeStorageGUI);

        public static bool VisibleAndExpanded => Visible && !UISystem.Instance.ExtremeStorageGUI._foldTimer.CompleteClose;

        public static ItemGroup CurrentGroup { get; private set; } = ItemGroup.Misc;
        public static TEExtremeStorage Storage { get; set; }
        public static Item[] AllItemsCached { get; private set; } // 缓存的所有附近物品，用于减少获取全物品时的卡顿

        internal static Dictionary<int, float[]> ChestSlotsGlowHue;
        internal static int ChestSlotsGlowTimer;
        internal static bool DisplayCrafting;

        private static List<int> _prevChestIndexes;
        private static int _findChestCountdown;
        private static bool _recalculateNextTick;

        private AnimationTimer _foldTimer, _scrollBarTimer;
        private UIPanel _totalPanel; // 透明的用于定位的面板
        private SUIPanel _basePanel;
        private StorageGrids _itemGrid;

        public override void OnInitialize()
        {
            ChestSlotsGlowHue = new Dictionary<int, float[]>();
            _scrollBarTimer = new AnimationTimer();
            _foldTimer = new AnimationTimer();

            _totalPanel = new UIPanel
            {
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            _totalPanel.SetPos(60f, Main.instance.invBottom + 40).SetSize(560f, 600f);
            Append(_totalPanel);

            _basePanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg);
            _basePanel.SetPos(0, 34).SetSize(-78f, 0f, 1f, 1f);
            _totalPanel.Append(_basePanel);

            _basePanel.Append(_itemGrid = new StorageGrids
            {
                First = true,
                Relative = RelativeMode.Vertical,
                Spacing = new Vector2(10, 15)
            });

            _totalPanel.Append(new GroupTab(ItemGroup.Weapon));
            _totalPanel.Append(new GroupTab(ItemGroup.Tool));
            _totalPanel.Append(new GroupTab(ItemGroup.Ammo));
            _totalPanel.Append(new GroupTab(ItemGroup.Armor));
            _totalPanel.Append(new GroupTab(ItemGroup.Accessory));
            _totalPanel.Append(new GroupTab(ItemGroup.Furniture));
            _totalPanel.Append(new GroupTab(ItemGroup.Block));
            _totalPanel.Append(new GroupTab(ItemGroup.Material));
            _totalPanel.Append(new GroupTab(ItemGroup.Alchemy));
            _totalPanel.Append(new GroupTab(ItemGroup.Misc));
            _totalPanel.Append(new GroupTab(ItemGroup.Setting));

            // 这里的位置设置实际无用，因为在 Update 里会被重置，但是如果不设置的话，会导致按钮在第一次打开动画时位置不正确
            const int buttonCoordY = 38;
            _totalPanel.Append(new RecipeToggleButton(_foldTimer).SetPos(-72f, buttonCoordY, 1f));
            _totalPanel.Append(new SortButton(_foldTimer).SetPos(-72f, buttonCoordY * 2, 1f));
            _totalPanel.Append(new StackToStorageButton(_foldTimer).SetPos(-72f, buttonCoordY * 3, 1f));
            _totalPanel.Append(new DepositAllButton(_foldTimer).SetPos(-72f, buttonCoordY * 4, 1f));
            _totalPanel.Append(new StackToInventoryButton(_foldTimer).SetPos(-72f, buttonCoordY * 5, 1f));
            _totalPanel.Append(new LootAllButton(_foldTimer).SetPos(-72f, buttonCoordY * 6, 1f));

            _basePanel.Append(new SettingsPage());

            // 初始数据设置
            _foldTimer.Timer = _foldTimer.TimerMax;
            _foldTimer.State = AnimationState.CompleteOpen;
            _totalPanel.Left.Pixels = 20;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            foreach (var element in _totalPanel.Children)
            {
                if (element is not StorageGrids && element.IsMouseHovering)
                {
                    _itemGrid.AttemptStoppingUsingSearchbar();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_recalculateNextTick)
            {
                _recalculateNextTick = false;
                Recalculate();
            }

            base.Update(gameTime);

            // mouseInterface
            bool isMouseHovering = _totalPanel.Children.Any(element => element.IsMouseHovering);
            if (isMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            // 锁定 MouseScroll
            if (_basePanel.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Extreme Storage GUI");

            // 物品栏开启判断
            if (!Main.playerInventory)
            {
                Close();
                return;
            }

            // 动画计时器
            UpdateAnimationTimer();

            // 物品整理的箱子槽位发光
            UpdateChestSlotGlow();

            // Setting 并不是一个实际上的 ItemGroup，所以不要也不能更新
            if (_findChestCountdown % 6 == 0 && CurrentGroup is not ItemGroup.Setting)
            {
                FindChestsAndPopulate();
            }

            _findChestCountdown++;

            // 根据屏幕高度重设框高度
            _itemGrid.RecalculateScrollBar();
            var screenDimensions = GetDimensions();
            int screenHeightZoomed = (int)screenDimensions.Height;
            float oldWidth = _totalPanel.Width.Pixels;
            float oldHeight = _totalPanel.Height.Pixels;
            float oldLeft = _totalPanel.Left.Pixels;

            // 应用动画，改变面板大小
            float maxPanelHeight = screenHeightZoomed - Main.instance.invBottom - 80;
            float panelHeight = MathHelper.Lerp(200, maxPanelHeight, _foldTimer.Schedule);
            float panelWidth = MathHelper.Lerp(560f, 588f, _scrollBarTimer.Schedule);
            _totalPanel.SetSize(panelWidth, panelHeight);
            float panelLeft = MathHelper.Lerp(60, 20, _foldTimer.Schedule);
            _totalPanel.Left.Pixels = panelLeft;

            if (oldWidth != _totalPanel.Width.Pixels || oldHeight != _totalPanel.Height.Pixels ||
                oldLeft != _totalPanel.Left.Pixels)
                _recalculateNextTick = true;

            // 是否开启制作栏侧栏
            Main.hidePlayerCraftingMenu |= !DisplayCrafting;
        }

        private void UpdateAnimationTimer()
        {
            if (_itemGrid.ScrollBarFilled || CurrentGroup is ItemGroup.Setting)
                _scrollBarTimer.Close();
            else
                _scrollBarTimer.Open();

            if (!DisplayCrafting)
                _foldTimer.Open();
            else
                _foldTimer.Close();

            _scrollBarTimer.Update();
            _foldTimer.Update();
        }

        private void UpdateChestSlotGlow()
        {
            if (ChestSlotsGlowTimer <= 0) return;

            ChestSlotsGlowTimer--;

            if (ChestSlotsGlowTimer is 0)
                ChestSlotsGlowHue.Clear();
        }

        public void OnSwapSlide(float factor)
        {
            float widthNext = _totalPanel.GetDimensions().Width;
            float shownPositionNext = MathHelper.Lerp(60, 20, _foldTimer.Schedule);
            float hiddenPositionNext = -widthNext - 78;

            // 宽高
            var screenDimensions = GetDimensions();
            int screenHeightZoomed = (int)screenDimensions.Height;
            float maxPanelHeight = screenHeightZoomed - Main.instance.invBottom - 80;
            float panelHeight = MathHelper.Lerp(200, maxPanelHeight, _foldTimer.Schedule);
            float panelWidth = MathHelper.Lerp(560f, 588f, _scrollBarTimer.Schedule);
            _totalPanel.SetSize(panelWidth, panelHeight);

            _totalPanel.Left.Set((int)MathHelper.Lerp(hiddenPositionNext, shownPositionNext, factor), 0f);
        }

        // 寻找箱子并设置物品栏
        internal void FindChestsAndPopulate(bool forced = false)
        {
            // 查找名字相应的箱子
            var chestIndexes = Storage.FindAllNearbyChestsWithGroup(CurrentGroup);

            // 判断是否相同，如果相同则不更新
            if (!forced && _prevChestIndexes is not null && _prevChestIndexes.SequenceEqual(chestIndexes))
                return;

            // 更新物品
            var items = new List<ChestItemsInfo>();
            chestIndexes.ForEach(i =>
            {
                var chestItems = Main.chest[i].item;
                var displayedItemIndexes = GetDisplayedItemIndexes(chestItems);
                items.Add(new ChestItemsInfo(chestItems, i, displayedItemIndexes));
            });

            _prevChestIndexes = chestIndexes;
            _itemGrid.SetInventory(items);
            RefreshCachedAllItems();
            Recipe.FindRecipes();
        }

        private IEnumerable<int> GetDisplayedItemIndexes(IReadOnlyList<Item> chestItems)
        {
            if (string.IsNullOrEmpty(_itemGrid.SearchContent))
            {
                for (var k = 0; k < chestItems.Count; k++)
                    yield return k;
                yield break;
            }

            string RemoveSpaces(string s) => s.Replace(" ", "", StringComparison.Ordinal);

            string searchContent = RemoveSpaces(_itemGrid.SearchContent.ToLower());
            for (var k = 0; k < chestItems.Count; k++)
            {
                int itemType = chestItems[k].type;
                string internalName =
                    RemoveSpaces(ItemID.Search.GetName(itemType).ToLower()); // 《英文名》因为没法在非英语语言获取英文名，只能用内部名了
                string currentLanguageName = RemoveSpaces(Lang.GetItemNameValue(itemType).ToLower());
                if (internalName.Contains(searchContent) || currentLanguageName.Contains(searchContent))
                {
                    yield return k;
                    continue;
                }

                if (Language.ActiveCulture.Name is not "zh-Hans") continue;

                string pinyin = RemoveSpaces(PinyinConvert.GetPinyinForAutoComplete(currentLanguageName));
                if (pinyin.Contains(searchContent)) {
                    yield return k;
                }
            }
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open()
        {
            SetGroup(CurrentGroup); // 用于重置一些基础设置
            OpenStateUpdatePacket.Send(Storage.ID);

            Main.playerInventory = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            FindChestsAndPopulate(true);

            ChestSlotsGlowHue = new Dictionary<int, float[]>();
            ChestSlotsGlowTimer = 0;

            // 在这里先调整好动画计时器
            if (_itemGrid.ScrollBarFilled)
            {
                _scrollBarTimer.Timer = 0f;
                _scrollBarTimer.State = AnimationState.CompleteClose;
            }
            else
            {
                _scrollBarTimer.Timer = _scrollBarTimer.TimerMax;
                _scrollBarTimer.State = AnimationState.CompleteOpen;
            }
        }

        public void Close()
        {
            _itemGrid.AttemptStoppingUsingSearchbar();
            OpenStateUpdatePacket.SendClose();
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
            ChestSelection.IsSelecting = false;
            Recipe.FindRecipes();
        }

        public static void SetGroup(ItemGroup group)
        {
            CurrentGroup = group;
            _findChestCountdown = 0;
            _prevChestIndexes = null; // 用于强制刷新箱子列表，强制刷新物品栏
            // Setting 并不是一个实际上的 ItemGroup
            UISystem.Instance.ExtremeStorageGUI._itemGrid.Hide = CurrentGroup is ItemGroup.Setting;
            DisplayCrafting &= CurrentGroup is not ItemGroup.Setting;
        }

        public static void RefreshCachedAllItems() => AllItemsCached = GetAllItems();

        /// <summary>
        /// 根据当前group寻找附近所有箱子
        /// </summary>
        /// <returns></returns>
        public static List<int> FindChestsWithCurrentGroup() => Storage.FindAllNearbyChestsWithGroup(CurrentGroup);

        public static Item[] GetAllItems() => Storage.GetAllItemsByGroup(CurrentGroup);

        public override bool CanPriority(UIElement target) => target != this;

        public override bool CanDisableMouse(UIElement target) =>
            (target != this && _basePanel.IsMouseHovering) || _basePanel.KeepPressed;
    }
}