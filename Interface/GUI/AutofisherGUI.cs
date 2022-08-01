using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Items.Placeable;
using ImproveGame.Interface.UIElements;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.GUI
{
    public class AutofisherGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        private Asset<Texture2D> selectPoolOff;
        private Asset<Texture2D> selectPoolOn;

        private UIPanel basePanel;
        private ModItemSlot accessorySlot = new();
        private ModItemSlot fishingPoleSlot = new();
        private ModItemSlot baitSlot = new();
        private FishItemSlot[] fishSlot = new FishItemSlot[15];
        private UIText tipText;
        private UIText title;
        private UIImage relocateButton;
        private UIPanel textPanel;

        internal static bool RequireRefresh = false;

        internal static List<int> FishingAccessories = new() { ItemID.TackleBox/*, ItemID.HighTestFishingLine*/, ItemID.AnglerEarring, ItemID.AnglerTackleBag, ItemID.LavaFishingHook, ItemID.LavaproofTackleBag };

        public override void OnInitialize() {
            panelTop = Main.instance.invBottom + 60;
            panelLeft = 100f;
            panelHeight = 250f;
            panelWidth = 270f;

            basePanel = new UIPanel();
            basePanel.Left.Set(panelLeft, 0f);
            basePanel.Top.Set(panelTop, 0f);
            basePanel.Width.Set(panelWidth, 0f);
            basePanel.Height.Set(panelHeight, 0f);
            Append(basePanel);

            accessorySlot = CreateItemSlot(
                25f, 0f,
                canPlace: (Item i, Item item) => SlotPlace(i, item) || FishingAccessories.Contains(item.type),
                onItemChanged: ChangeAccessorySlot,
                emptyText: () => GetText($"Autofisher.Accessory"),
                parent: basePanel,
                folderName: "Autofisher",
                iconTextureName: "Slot_Accessory"
            );
            accessorySlot.AllowFavorite = false;

            fishingPoleSlot = CreateItemSlot(
                75f, 0f,
                canPlace: (Item i, Item item) => SlotPlace(i, item) || item.fishingPole > 0,
                onItemChanged: ChangeFishingPoleSlot,
                emptyText: () => GetText($"Autofisher.FishingPole"),
                parent: basePanel,
                folderName: "Autofisher",
                iconTextureName: "Slot_FishingPole"
            );
            fishingPoleSlot.AllowFavorite = false;

            baitSlot = CreateItemSlot(
                125f, 0f,
                canPlace: (Item i, Item item) => SlotPlace(i, item) || item.bait > 0,
                onItemChanged: ChangeBaitSlot,
                emptyText: () => GetText($"Autofisher.Bait"),
                parent: basePanel,
                folderName: "Autofisher",
                iconTextureName: "Slot_Bait"
            );
            baitSlot.OnRightClickItemChange += ChangeBaitSlotStack;
            baitSlot.AllowFavorite = false;

            const int slotFirst = 50;
            for (int i = 0; i < fishSlot.Length; i++) {
                int x = i % 5 * slotFirst;
                int y = i / 5 * slotFirst + slotFirst;
                fishSlot[i] = new(i);
                fishSlot[i].SetPos(x, y);
                fishSlot[i].SetSize(46f, 46f);
                fishSlot[i].AllowFavorite = false;
                fishSlot[i].OnFishChange += ChangeFishSlot;
                fishSlot[i].OnFishRightClickChange += ChangeFishSlotStack;
                basePanel.Append(fishSlot[i]);
            }

            // 头顶大字
            title = new("Autofisher", 0.5f, large: true) {
                HAlign = 0.5f
            };
            title.Left.Set(0, 0f);
            title.Top.Set(-40, 0f);
            title.Width.Set(panelWidth, 0f);
            title.Height.Set(30, 0f);
            basePanel.Append(title);
            
            textPanel = new() {
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(200f),
                Width = StyleDimension.FromPixels(basePanel.Width.Pixels - 16f),
                Height = StyleDimension.FromPixels(30f),
                BackgroundColor = new Color(35, 40, 83),
                BorderColor = new Color(35, 40, 83)
            };
            textPanel.SetPadding(0f);
            basePanel.Append(textPanel);
            tipText = new("Error", 0.8f) {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            textPanel.Append(tipText);

            selectPoolOff = GetTexture("UI/Autofisher/SelectPoolOff");
            selectPoolOn = GetTexture("UI/Autofisher/SelectPoolOn");
            relocateButton = new(selectPoolOff);
            relocateButton.Left.Set(175f, 0f);
            relocateButton.Top.Set(0f, 0f);
            relocateButton.Width.Set(46f, 0f);
            relocateButton.Height.Set(46f, 0f);
            relocateButton.OnMouseDown += (_, _) => ToggleSelectPool();
            basePanel.Append(relocateButton);
        }

        public void ToggleSelectPool() {
            WandSystem.SelectPoolMode = !WandSystem.SelectPoolMode;
            if (WandSystem.SelectPoolMode) {
                title.SetText(GetText("Autofisher.SelectPool"));
                relocateButton.SetImage(selectPoolOn);
            }
            else {
                title.SetText(GetText("Autofisher.Title"));
                relocateButton.SetImage(selectPoolOff);
            }
        }

        private void ChangeAccessorySlot(Item item, bool rightClick) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.accessory = item;
            if (Main.netMode is NetmodeID.MultiplayerClient) {
                NetAutofish.ClientSendItem(17, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void ChangeFishingPoleSlot(Item item, bool rightClick) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fishingPole = item;
            if (Main.netMode is NetmodeID.MultiplayerClient) {
                NetAutofish.ClientSendItem(15, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void ChangeBaitSlot(Item item, bool rightClick) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.bait = item;
            if (Main.netMode is NetmodeID.MultiplayerClient && !rightClick) {
                NetAutofish.ClientSendItem(16, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        } 

        private void ChangeBaitSlotStack(Item item, int stackChange, bool typeChange) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;
            if (!typeChange && stackChange != 0) {
                NetAutofish.SendStackChange(AutofishPlayer.LocalPlayer.Autofisher, 16, stackChange);
            }
        }

        private void ChangeFishSlot(Item item, int i, bool rightClick) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fish[i] = item;
            if (Main.netMode == NetmodeID.MultiplayerClient && !rightClick) {
                NetAutofish.ClientSendItem((byte)i, item, AutofishPlayer.LocalPlayer.Autofisher);
            }
        }

        private void ChangeFishSlotStack(Item item, int i, int stackChange, bool typeChange) {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;
            if (Main.netMode is NetmodeID.MultiplayerClient && !typeChange && stackChange != 0) {
                NetAutofish.SendStackChange(AutofishPlayer.LocalPlayer.Autofisher, (byte)i, stackChange);
            }
        }

        public void RefreshItems(byte slotType = 18) {
            if (Main.netMode is NetmodeID.MultiplayerClient) {
                if (RequireRefresh) {
                    SyncFromTileEntity();
                    RequireRefresh = false;
                }
                else {
                    NetAutofish.ClientSendSyncItem(AutofishPlayer.LocalPlayer.Autofisher, slotType);
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
                accessorySlot.Item = autofisher.accessory;
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

            if (AutofishPlayer.LocalPlayer.TryGetAutofisher(out var autofisher)) {
                if (baitSlot.Item.type == ItemID.TruffleWorm) {
                    autofisher.SetFishingTip(Language.GetTextValue("GameUI.FishingWarning"));
                }
                if (baitSlot.Item.IsAir || fishingPoleSlot.Item.IsAir || autofisher.FishingTip == "Error") {
                    autofisher.SetFishingTip(GetText("Autofisher.Unavailable"));
                }

                tipText.SetText(autofisher.FishingTip);

                base.Draw(spriteBatch);

                if (basePanel.ContainsPoint(Main.MouseScreen)) {
                    player.mouseInterface = true;
                }
            }

            // 用 title.IsMouseHovering 出框之后就会没
            if (title.GetDimensions().ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)) || textPanel.IsMouseHovering)
            {
                var dimension = basePanel.GetDimensions();
                var position = dimension.Position() + new Vector2(dimension.Width + 20f, 0f);

                var tooltip = Lang.GetTooltip(ModContent.ItemType<Autofisher>());
                int lines = tooltip.Lines;
                var font = FontAssets.MouseText.Value;
                int widthOffset = 14;
                int heightOffset = 9;
                float lengthX = 0f;
                float lengthY = 0f;

                for (int i = 0; i < lines; i++)
                {
                    string line = tooltip.GetLine(i);
                    var stringSize = ChatManager.GetStringSize(font, line, Vector2.One);
                    lengthX = Math.Max(lengthX, stringSize.X + 8);
                    lengthY += stringSize.Y;
                }

                Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)position.X - widthOffset, (int)position.Y - heightOffset, (int)lengthX + widthOffset * 2, (int)lengthY + heightOffset + heightOffset / 2), new Color(23, 25, 81, 255) * 0.925f);

                for (int i = 0; i < lines; i++)
                {
                    string line = tooltip.GetLine(i);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, line, position, Color.White, 0f, Vector2.Zero, Vector2.One, spread: 1.6f);
                    position.Y += (int)ChatManager.GetStringSize(font, line, Vector2.One).Y;
                }
            }
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(Point16 point) {
            WandSystem.SelectPoolMode = false;
            Main.playerInventory = true;
            Visible = true;
            title.SetText(GetText("Autofisher.Title"));
            SoundEngine.PlaySound(AutofishPlayer.LocalPlayer.Autofisher != Point16.NegativeOne ? SoundID.MenuTick : SoundID.MenuOpen);
            AutofishPlayer.LocalPlayer.SetAutofisher(point);
            RefreshItems();
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            relocateButton.SetImage(selectPoolOff);
            WandSystem.SelectPoolMode = false;
            AutofishPlayer.LocalPlayer.SetAutofisher(Point16.NegativeOne);
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
