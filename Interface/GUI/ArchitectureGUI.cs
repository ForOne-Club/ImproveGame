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

        public static int CurrentSlot;
        public static Item CurrentItem {
            get => Main.LocalPlayer.inventory[CurrentSlot];
            set => Main.LocalPlayer.inventory[CurrentSlot] = value;
        }
        public static CreateWand CurrentWand => CurrentItem.ModItem as CreateWand;

        public static Dictionary<string, ModItemSlot> ItemSlot => itemSlot;

        private const float InventoryScale = 0.85f;

        private static bool PrevMouseRight;
        private static bool HoveringOnSlots;
        private bool Dragging;
        private Vector2 Offset;

        private static UIPanel basePanel;
        private static Dictionary<string, ModItemSlot> itemSlot = new();
        private static UIText materialTitle;
        private static UIIconTextButton styleButton;

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
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Block), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Wall)] = CreateItemSlot(slotSecond, slotFirst, nameof(CreateWand.Wall),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || item.createWall > -1,
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Wall), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Platform)] = CreateItemSlot(slotThird, slotFirst, nameof(CreateWand.Platform),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Platforms[item.createTile]),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Platform), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Torch)] = CreateItemSlot(slotFirst, slotSecond, nameof(CreateWand.Torch),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Torch[item.createTile]),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Torch), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Chair)] = CreateItemSlot(slotSecond, slotSecond, nameof(CreateWand.Chair),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Chairs),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Chair), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Workbench)] = CreateItemSlot(slotThird, slotSecond, nameof(CreateWand.Workbench),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.WorkBenches),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Workbench), item.Clone(), CurrentSlot)),

                [nameof(CreateWand.Bed)] = CreateItemSlot(slotFirst, slotThird, nameof(CreateWand.Bed),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Beds),
                    (Item item) => CurrentWand.SetItem(nameof(CreateWand.Bed), item.Clone(), CurrentSlot))
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
            styleButton = new(Language.GetText("Mods.ImproveGame.Architecture.StyleChange"), Color.White, "Images/UI/DisplaySlots_5");
            styleButton.Left.Set(slotSecond, 0f);
            styleButton.Top.Set(slotThird, 0f);
            styleButton.Width.Set(104f, 0f);
            styleButton.Height.Set(42f, 0f);
            styleButton.OnClick += (UIMouseEvent _, UIElement _) => CreateWand.ToggleStyle();
            basePanel.Append(styleButton);
        }

        /// <summary>
        /// 快捷做一个ItemSlot
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="iconTextureName">空物品时显示的贴图</param>
        /// <param name="canPlace">是否可以放入物品的判断</param>
        /// <param name="onItemChanged">物品更改时执行</param>
        /// <returns>一个<see cref="ModItemSlot"/>实例</returns>
        private ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, Func<Item, Item, bool> canPlace = null, Action<Item> onItemChanged = null) {
            ModItemSlot slot = new(InventoryScale, $"ImproveGame/Assets/Images/UI/Icon_{iconTextureName}");
            slot.Left.Set(x, 0f);
            slot.Top.Set(y, 0f);
            slot.Width.Set(46f, 0f);
            slot.Height.Set(46f, 0f);
            slot.OnUpdate += (UIElement _) => HoveringOnSlots |= slot.IsMouseHovering;
            if (canPlace is not null)
                slot.OnCanPlaceItem += canPlace;
            if (onItemChanged is not null)
                slot.OnItemChange += onItemChanged;
            basePanel.Append(slot);
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

            //Initialize();

            //basePanel.Draw(spriteBatch);
            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen)) {
                player.mouseInterface = true;
            }
        }

        /// <summary>
        /// 更新GUI物品槽与物品的同步
        /// </summary>
        /// <param name="createWand">建筑魔杖<see cref="CreateWand"/>实例</param>
        public static void RefreshSlots(CreateWand createWand) {
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
        public static void Open() {
            Main.playerInventory = true;
            PrevMouseRight = true; // 防止一打开就关闭
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentSlot = Main.LocalPlayer.selectedItem;
            RefreshSlots(CurrentWand);

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            materialTitle.SetText(Language.GetText("Mods.ImproveGame.Architecture.Materials"));
            styleButton.SetText(Language.GetText("Mods.ImproveGame.Architecture.StyleChange"), 1f, Color.White);
            // 翻源码发现UIIconTextButton.SetText之后会重设Width和Height，尚且不知道为啥，不过我们可以给他改回来
            styleButton.Width.Set(104f, 0f);
            styleButton.Height.Set(42f, 0f);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public static void Close() {
            CurrentSlot = -1;
            Visible = false;
            PrevMouseRight = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
