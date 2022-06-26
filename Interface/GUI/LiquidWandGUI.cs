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
    public class LiquidWandGUI : UIState
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
        public static LiquidWand CurrentWand => CurrentItem.ModItem as LiquidWand;

        private static UIPanel basePanel;
        private static ModItemSlot emptyBucketSlot;
        private static UIText title;
        private static UIIconTextButton modeButton;

        private static bool PrevMouseRight;
        private static bool HoveringOnSlots;
        private bool Dragging;
        private Vector2 Offset;

        public override void OnInitialize() {
            panelLeft = 600f;
            panelTop = 80f;
            panelHeight = 110f;
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
            // O 切换
            // O O O
            // 排布如上
            const float slotFirst = 0f;
            const float slotSecond = 60f;
            const float slotThird = 120f;

            emptyBucketSlot = CreateItemSlot(slotFirst, slotFirst, nameof(LiquidWand.Bucket),
                    (Item i, Item item) => MyUtils.SlotPlace(i, item) || (item.createTile > -1 && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile]),
                    (Item item) => CurrentWand.SetBucket(item.Clone(), CurrentSlot),
                    () => Lang.GetItemNameValue(ItemID.EmptyBucket));

            // 头顶大字
            title = new("Materials", 0.5f, large: true) {
                HAlign = 0.5f
            };
            title.Left.Set(0, 0f);
            title.Top.Set(-40, 0f);
            title.Width.Set(panelWidth, 0f);
            title.Height.Set(40, 0f);
            basePanel.Append(title);

            // 使用模式修改按钮
            modeButton = new(Language.GetText("Mods.ImproveGame.LiquidWand.ModeChange"), Color.White, "Images/UI/DisplaySlots_5");
            modeButton.Left.Set(slotSecond, 0f);
            modeButton.Top.Set(slotThird, 0f);
            modeButton.Width.Set(104f, 0f);
            modeButton.Height.Set(42f, 0f);
            modeButton.OnClick += (UIMouseEvent _, UIElement _) => CreateWand.ToggleStyle();
            basePanel.Append(modeButton);
        }

        public static ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, Func<Item, Item, bool> canPlace = null, Action<Item> onItemChanged = null, Func<string> emptyText = null) {
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

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public static void Open(int setSlotIndex = -1) {
            Main.playerInventory = true;
            PrevMouseRight = true; // 防止一打开就关闭
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentSlot = Main.LocalPlayer.selectedItem;
            if (setSlotIndex is not -1) {
                CurrentSlot = setSlotIndex;
            }

            // 关掉本Mod其他的同类UI
            if (ArchitectureGUI.Visible) ArchitectureGUI.Close();

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            title.SetText(Language.GetText("Mods.ImproveGame.LiquidWand.Title"));
            modeButton.SetText(Language.GetText("Mods.ImproveGame.LiquidWand.ModeChange"), 1f, Color.White);
            // 翻源码发现UIIconTextButton.SetText之后会重设Width和Height，尚且不知道为啥，不过我们可以给他改回来
            modeButton.Width.Set(104f, 0f);
            modeButton.Height.Set(42f, 0f);
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
