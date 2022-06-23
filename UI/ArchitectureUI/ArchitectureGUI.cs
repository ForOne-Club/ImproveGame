using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using ImproveGame.UI.UIElements;
using System.Collections.Generic;
using ImproveGame.Content.Items;
using System;

namespace ImproveGame.UI.ArchitectureUI
{
    public class ArchitectureGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        private const float InventoryScale = 0.85f;

        private bool Dragging;
        private Vector2 Offset;

        private UIPanel basePanel;
        private Dictionary<string, ModItemSlot> itemSlot = new();

        public override void OnInitialize() {
            panelLeft = 300f;
            panelTop = 300f;
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
            // O
            // 排布如上
            const float slotFirst = 0f;
            const float slotSecond = 60f;
            const float slotThird = 120f;
            itemSlot = new() {
                [nameof(CreateWand.Block)] = CreateItemSlot(slotFirst, slotFirst, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile])),
                [nameof(CreateWand.Wall)] = CreateItemSlot(slotSecond, slotFirst, (Item i, Item item) =>
                    SlotPlace(i, item) || item.createWall > -1),
                [nameof(CreateWand.Platform)] = CreateItemSlot(slotThird, slotFirst, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Platforms[item.createTile])),

                [nameof(CreateWand.Torch)] = CreateItemSlot(slotFirst, slotSecond, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Torch[item.createTile])),
                [nameof(CreateWand.Chair)] = CreateItemSlot(slotSecond, slotSecond, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Chairs)),
                [nameof(CreateWand.Workbench)] = CreateItemSlot(slotThird, slotSecond, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.WorkBenches)),

                [nameof(CreateWand.Bed)] = CreateItemSlot(slotFirst, slotThird, (Item i, Item item) =>
                    SlotPlace(i, item) || (item.createTile > -1 && item.createTile == TileID.Beds))
            };
        }

        private bool SlotPlace(Item slotItem, Item mouseItem) {
            return slotItem.type == mouseItem.type || mouseItem.IsAir;
        }

        private ModItemSlot CreateItemSlot(float x, float y, Func<Item, Item, bool> canPlace) {
            ModItemSlot slot = new(InventoryScale);
            slot.Left.Set(x, 0f);
            slot.Top.Set(y, 0f);
            slot.Width.Set(50f, 0f);
            slot.Height.Set(50f, 0f);
            slot.OnCanPlaceItem += canPlace;
            basePanel.Append(slot);
            return slot;
        }

        private void DragStart(UIMouseEvent evt, UIElement listeningElement) {
            var dimensions = listeningElement.GetDimensions().ToRectangle();
            Offset = new Vector2(evt.MousePosition.X - dimensions.Left, evt.MousePosition.Y - dimensions.Top);
            Dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement) {
            Vector2 end = evt.MousePosition;
            Dragging = false;

            listeningElement.Left.Set(end.X - Offset.X, 0f);
            listeningElement.Top.Set(end.Y - Offset.Y, 0f);

            listeningElement.Recalculate();
        }


        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (!Main.playerInventory && Visible) {
                Close();
            }

            if (Dragging) {
                basePanel.Left.Set(Main.mouseX - Offset.X, 0f);
                basePanel.Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
            }
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

        public static void Open() {
            Main.playerInventory = true;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Recipe.FindRecipes();
        }

        public static void Close() {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
            Recipe.FindRecipes();
        }
    }
}
