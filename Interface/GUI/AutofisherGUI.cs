using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Common.Utils;
using ImproveGame.Content.Items;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class AutofisherGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        private UIPanel basePanel;
        private ModItemSlot fishingPoleSlot = new();
        private ModItemSlot baitSlot = new();
        private FishItemSlot[] fishSlot = new FishItemSlot[15];
        private UIImageButton relocateButton;

        internal static bool RequireRefresh = false;

        public override void OnInitialize() {
            panelTop = Main.instance.invBottom + 60;
            panelLeft = 100f;
            panelHeight = 220f;
            panelWidth = 270f;

            basePanel = new UIPanel();
            basePanel.Left.Set(panelLeft, 0f);
            basePanel.Top.Set(panelTop, 0f);
            basePanel.Width.Set(panelWidth, 0f);
            basePanel.Height.Set(panelHeight, 0f);
            Append(basePanel);

            fishingPoleSlot = MyUtils.CreateItemSlot(
                50f, 0f,
                canPlace: (Item i, Item item) => item.fishingPole > 0,
                onItemChanged: ChangeFishingPoleSlot,
                emptyText: () => MyUtils.GetText($"Autofisher.FishingPole"),
                parent: basePanel
            );
            fishingPoleSlot.AllowFavorite = false;

            baitSlot = MyUtils.CreateItemSlot(
                100f, 0f,
                canPlace: (Item i, Item item) => item.bait > 0,
                onItemChanged: ChangeBaitSlot,
                emptyText: () => MyUtils.GetText($"Autofisher.Bait"),
                parent: basePanel
            );
            baitSlot.AllowFavorite = false;

            const int slotFirst = 50;
            for (int i = 0; i < fishSlot.Length; i++) {
                int x = i % 5 * slotFirst;
                int y = i / 5 * slotFirst + slotFirst;
                fishSlot[i] = new(i);
                fishSlot[i].SetPos(x, y);
                fishSlot[i].SetSize(46f, 46f);
                fishSlot[i].AllowFavorite = false;
                fishSlot[i].OnFishChange += SyncFish;
                basePanel.Append(fishSlot[i]);
            }

            relocateButton = new(Main.Assets.Request<Texture2D>("Images/UI/DisplaySlots_5"));
            relocateButton.Left.Set(150f, 0f);
            relocateButton.Top.Set(0f, 0f);
            relocateButton.Width.Set(46f, 0f);
            relocateButton.Height.Set(46f, 0f);
            basePanel.Append(relocateButton);
        }

        private void ChangeFishingPoleSlot(Item item) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fishingPole = item;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetHelper.Autofish_ClientSendItem(15, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void ChangeBaitSlot(Item item) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.bait = item;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetHelper.Autofish_ClientSendItem(16, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void SyncFish(Item item, int i) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fish[i] = item;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetHelper.Autofish_ClientSendItem((byte)i, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void RefreshItems() {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                if (RequireRefresh) {
                    SyncFromTileEntity();
                    RequireRefresh = false;
                }
                else {
                    NetHelper.Autofish_ClientSendSyncItem(AutofishPlayer.LocalPlayer.Autofisher, 17);
                }
            }
            else {
                SyncFromTileEntity();
            }
        }

        private void SyncFromTileEntity() {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is not null) {
                fishingPoleSlot.Item = autofisher.fishingPole;
                baitSlot.Item = autofisher.bait;
                for (int i = 0; i < 15; i++) {
                    fishSlot[i].Item = autofisher.fish[i];
                }
            }
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (!Main.playerInventory) {
                Close();
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            Player player = Main.LocalPlayer;

            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen)) {
                player.mouseInterface = true;
            }
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(Point16 point) {
            Main.playerInventory = true;
            SoundEngine.PlaySound(AutofishPlayer.LocalPlayer.Autofisher != Point16.NegativeOne ? SoundID.MenuTick : SoundID.MenuOpen);
            AutofishPlayer.LocalPlayer.SetAutofisher(point);
            Visible = true;
            RefreshItems();
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            AutofishPlayer.LocalPlayer.SetAutofisher(Point16.NegativeOne);
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
