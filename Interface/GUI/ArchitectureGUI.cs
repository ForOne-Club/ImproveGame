using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class ArchitectureGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        public int CurrentSlot;
        public Item CurrentItem {
            get => Main.LocalPlayer.inventory[CurrentSlot];
            set => Main.LocalPlayer.inventory[CurrentSlot] = value;
        }
        public CreateWand CurrentWand => CurrentItem.ModItem as CreateWand;

        public Dictionary<string, ModItemSlot> ItemSlot => itemSlot;

        private static bool PrevMouseRight;
        private static bool HoveringOnSlots;
        private bool Dragging;
        private Vector2 Offset;

        private UIPanel basePanel;
        private Dictionary<string, ModItemSlot> itemSlot = new();
        private UIText materialTitle;
        private ModIconTextButton styleButton;

        public override void OnInitialize() {
            panelLeft = 600f;
            panelTop = 80f;
            panelHeight = 190f;
            panelWidth = 190f;

            basePanel = new UIPanel();
            basePanel.Left.Set(panelLeft, 0f);
            basePanel.Top.Set(panelTop, 0f);
            basePanel.Width.Set(panelWidth, 0f);
            basePanel.Height.Set(panelHeight, 0f);
            basePanel.OnMouseDown += DragStart;
            basePanel.OnMouseUp += DragEnd;
            Append(basePanel);

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
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile]), 
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Block), item.Clone(), CurrentSlot),
                    () => MyUtils.GetText($"Architecture.{nameof(CreateWand.Block)}")),

                [nameof(CreateWand.Wall)] = CreateItemSlot(slotSecond, slotFirst, nameof(CreateWand.Wall),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || item.createWall > -1,
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Wall), item.Clone(), CurrentSlot),
                    () => MyUtils.GetText($"Architecture.{nameof(CreateWand.Wall)}")),

                [nameof(CreateWand.Platform)] = CreateItemSlot(slotThird, slotFirst, nameof(CreateWand.Platform),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Platforms[item.createTile]),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Platform), item.Clone(), CurrentSlot),
                    () => MyUtils.GetText($"Architecture.{nameof(CreateWand.Platform)}")),

                [nameof(CreateWand.Torch)] = CreateItemSlot(slotFirst, slotSecond, nameof(CreateWand.Torch),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Torch[item.createTile]),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Torch), item.Clone(), CurrentSlot),
                    () => Lang.GetItemNameValue(ItemID.Torch)),

                [nameof(CreateWand.Chair)] = CreateItemSlot(slotSecond, slotSecond, nameof(CreateWand.Chair),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Chairs),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Chair), item.Clone(), CurrentSlot),
                    () => MyUtils.GetText($"Architecture.{nameof(CreateWand.Chair)}")),

                [nameof(CreateWand.Workbench)] = CreateItemSlot(slotThird, slotSecond, nameof(CreateWand.Workbench),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.WorkBenches),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Workbench), item.Clone(), CurrentSlot),
                    () => Lang.GetItemNameValue(ItemID.WorkBench)),

                [nameof(CreateWand.Bed)] = CreateItemSlot(slotFirst, slotThird, nameof(CreateWand.Bed),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Beds),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Bed), item.Clone(), CurrentSlot),
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
            styleButton.OnClick += (_, _) => CreateWand.ToggleStyle();
            basePanel.Append(styleButton);
        }

        public ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, Func<Item, Item, bool> canPlace = null, Action<Item> onItemChanged = null, Func<string> emptyText = null) {
            ModItemSlot slot = MyUtils.CreateItemSlot(x, y, iconTextureName, 0.85f, canPlace, onItemChanged, emptyText, basePanel);
            slot.OnUpdate += (UIElement _) => HoveringOnSlots |= slot.IsMouseHovering;
            return slot;
        }

        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement) {
            var dimensions = listeningElement.GetDimensions().ToRectangle();
            Offset = new Vector2(evt.MousePosition.X - dimensions.Left, evt.MousePosition.Y - dimensions.Top);
            Dragging = true;
        }

        // 可拖动界面
        private void DragEnd(UIMouseEvent evt, UIElement listeningElement) {
            if (!Dragging)
                return;

            Vector2 end = evt.MousePosition;
            Dragging = false;

            listeningElement.Left.Set(end.X - Offset.X, 0f);
            listeningElement.Top.Set(end.Y - Offset.Y, 0f);

            listeningElement.Recalculate();
        }

        // 主要是可拖动和一些判定吧
        public override void Update(GameTime gameTime) {
            if (!Main.LocalPlayer.inventory.IndexInRange(CurrentSlot) || CurrentItem is null || CurrentItem.ModItem is not CreateWand) {
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

            if (Dragging) {
                basePanel.Left.Set(Main.mouseX - Offset.X, 0f);
                basePanel.Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
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
        public void Open(int setSlotIndex = -1) {
            Main.playerInventory = true;
            PrevMouseRight = true; // 防止一打开就关闭
            Dragging = false;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentSlot = Main.LocalPlayer.selectedItem;
            if (setSlotIndex is not -1) {
                CurrentSlot = setSlotIndex;
            }
            RefreshSlots(CurrentWand);

            // 关掉本Mod其他的同类UI
            if (LiquidWandGUI.Visible) UISystem.Instance.LiquidWandGUI.Close();

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            materialTitle.SetText(Language.GetText("Mods.ImproveGame.Architecture.Materials"));
            styleButton.SetText(Language.GetText("Mods.ImproveGame.Common.Switch"), 1f, Color.White);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            CurrentSlot = -1;
            Visible = false;
            PrevMouseRight = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
