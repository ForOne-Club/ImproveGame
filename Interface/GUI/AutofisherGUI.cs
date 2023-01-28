using ImproveGame.Common.Animations;
using ImproveGame.Common.Packets.NetAutofisher;
using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.SUIElements;
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

        private SUIPanel basePanel;
        private ModItemSlot accessorySlot = new();
        private ModItemSlot fishingPoleSlot = new();
        private ModItemSlot baitSlot = new();
        private FishItemSlot[] fishSlot = new FishItemSlot[15];
        private UIText tipText;
        private UIText title;
        private UIImage relocateButton;
        private SUIPanel textPanel;

        internal static bool RequireRefresh = false;

        internal static List<int> FishingAccessories = new() { ItemID.TackleBox/*, ItemID.HighTestFishingLine*/, ItemID.AnglerEarring, ItemID.AnglerTackleBag, ItemID.LavaFishingHook, ItemID.LavaproofTackleBag };

        public override void OnInitialize()
        {
            panelTop = Main.instance.invBottom + 60;
            panelLeft = 100f;
            panelHeight = 256f;
            panelWidth = 280f;

            basePanel = new SUIPanel(new Color(29, 34, 70), new Color(44, 57, 105, 160));
            basePanel.SetPos(panelLeft, panelTop).SetSize(panelWidth, panelHeight);
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
            for (int i = 0; i < fishSlot.Length; i++)
            {
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
            title = new("Autofisher", 0.5f, large: true)
            {
                HAlign = 0.5f
            };
            title.Left.Set(0, 0f);
            title.Top.Set(-40, 0f);
            title.Width.Set(panelWidth, 0f);
            title.Height.Set(30, 0f);
            basePanel.Append(title);

            textPanel = new(new Color(35, 40, 83), new Color(35, 40, 83), round: 10)
            {
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(200f),
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPixels(30f)
            };
            textPanel.SetPadding(0f);
            basePanel.Append(textPanel);
            tipText = new("Error", 0.8f)
            {
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

            float filtersX = panelLeft + panelWidth + 10f;
            float filtersY = panelTop + 8f;
            var filter = new CatchCratesFilter().SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchAccessoriesFilter().SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchToolsFilter().SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchWhiteRarityCatchesFilter().SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchNormalCatchesFilter().SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
        }

        public void ToggleSelectPool()
        {
            WandSystem.SelectPoolMode = !WandSystem.SelectPoolMode;
            if (WandSystem.SelectPoolMode)
            {
                title.SetText(GetText("Autofisher.SelectPool"));
                relocateButton.SetImage(selectPoolOn);
            }
            else
            {
                title.SetText(GetText("Autofisher.Title"));
                relocateButton.SetImage(selectPoolOff);
            }
        }

        private void ChangeAccessorySlot(Item item, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.accessory = item;
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, 17).Send(runLocally: false);
            }
        }

        private void ChangeFishingPoleSlot(Item item, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fishingPole = item;
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, 15).Send(runLocally: false);
            }
        }

        private void ChangeBaitSlot(Item item, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.bait = item;
            if (Main.netMode is NetmodeID.MultiplayerClient && !rightClick)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, 16).Send(runLocally: false);
            }
        }

        private void ChangeBaitSlotStack(Item item, int stackChange, bool typeChange)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;
            if (!typeChange && stackChange != 0)
            {
                ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher, 16, stackChange).Send(runLocally: false);
            }
        }

        private void ChangeFishSlot(Item item, int i, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;

            autofisher.fish[i] = item;
            if (Main.netMode == NetmodeID.MultiplayerClient && !rightClick)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, (byte)i).Send(runLocally: false);
            }
        }

        private void ChangeFishSlotStack(Item item, int i, int stackChange, bool typeChange)
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is null)
                return;
            if (Main.netMode is NetmodeID.MultiplayerClient && !typeChange && stackChange != 0)
            {
                ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher, (byte)i, stackChange).Send(runLocally: false);
            }
        }

        public void RefreshItems(byte slotType = 18)
        {
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                if (RequireRefresh)
                {
                    SyncFromTileEntity();
                    RequireRefresh = false;
                }
                else
                {
                    RequestItemPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, slotType).Send(runLocally: false);
                }
            }
            else
            {
                SyncFromTileEntity();
            }
        }

        private void SyncFromTileEntity()
        {
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is not null)
            {
                fishingPoleSlot.Item = autofisher.fishingPole;
                baitSlot.Item = autofisher.bait;
                accessorySlot.Item = autofisher.accessory;
                for (int i = 0; i < 15; i++)
                {
                    fishSlot[i].Item = autofisher.fish[i];
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                Close();
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (AutofishPlayer.LocalPlayer.TryGetAutofisher(out var autofisher))
            {
                if (baitSlot.Item.type == ItemID.TruffleWorm)
                {
                    autofisher.SetFishingTip(Autofisher.TipType.FishingWarning);
                }
                if (baitSlot.Item.IsAir || fishingPoleSlot.Item.IsAir || autofisher.FishingTip == "Error")
                {
                    autofisher.SetFishingTip(Autofisher.TipType.Unavailable);
                }

                tipText.SetText(autofisher.FishingTip);

                base.Draw(spriteBatch);

                if (basePanel.ContainsPoint(Main.MouseScreen))
                {
                    player.mouseInterface = true;
                }
            }

            // 用 title.IsMouseHovering 出框之后就会没
            if (title.GetDimensions().ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)) || textPanel.IsMouseHovering)
            {
                var dimension = basePanel.GetDimensions();
                var position = dimension.Position() + new Vector2(dimension.Width + 20f, 0f);

                var tooltip = Lang.GetTooltip(ModContent.ItemType<Content.Items.Placeable.Autofisher>());
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
        public void Open(Point16 point)
        {
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
        public void Close()
        {
            relocateButton.SetImage(selectPoolOff);
            WandSystem.SelectPoolMode = false;
            AutofishPlayer.LocalPlayer.SetAutofisher(Point16.NegativeOne);
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }

    internal class CatchCratesFilter : AutofisherFilterButton
    {
        internal CatchCratesFilter() : base(ItemID.WoodenCrate) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchCrates;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchCrates, 0).Send(runLocally: true);
        }
    }

    internal class CatchAccessoriesFilter : AutofisherFilterButton
    {
        internal CatchAccessoriesFilter() : base(ItemID.FrogLeg) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchAccessories;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchAccessories, 1).Send(runLocally: true);
        }
    }

    internal class CatchToolsFilter : AutofisherFilterButton
    {
        internal CatchToolsFilter() : base(ItemID.CrystalSerpent) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchTools;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchTools, 2).Send(runLocally: true);
        }
    }

    internal class CatchWhiteRarityCatchesFilter : AutofisherFilterButton
    {
        internal CatchWhiteRarityCatchesFilter() : base(ItemID.Bass) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchWhiteRarityCatches;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchWhiteRarityCatches, 3).Send(runLocally: true);
        }
    }

    internal class CatchNormalCatchesFilter : AutofisherFilterButton
    {
        internal CatchNormalCatchesFilter() : base(ItemID.GoldenCarp) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchNormalCatches;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchNormalCatches, 4).Send(runLocally: true);
        }
    }

    internal abstract class AutofisherFilterButton : UIElement
    {
        internal int ItemType;
        private AnimationTimer _timer; // 这是一个计时器哦~

        internal virtual bool IsActivated(TEAutofisher autofisher) => true;

        internal virtual void Clicked(TEAutofisher autofisher) { }

        internal AutofisherFilterButton(int itemType)
        {
            ItemType = itemType;
            _timer = new(TimerMax: 90f)
            {
                State = AnimationState.Close
            };
            Main.instance.LoadItem(itemType);
            this.SetSize(TextureAssets.Item[itemType].Size());
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (!AutofishPlayer.LocalPlayer.TryGetAutofisher(out var autofisher))
                return;
            Clicked(autofisher);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            _timer.Update();
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            var tex = TextureAssets.Item[ItemType];
            if (IsMouseHovering)
            {
                _timer.TryOpen();

                Main.LocalPlayer.mouseInterface = true;

                Main.instance.MouseText(GetText($"Autofisher.{GetType().Name}"));

            }
            else
            {
                _timer.TryClose();
            }

            if (_timer.Timer > 10)
            {
                Main.spriteBatch.End(); // End后Begin来使用shader绘制描边
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);

                Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply(); // 全白Shader
                for (int k = -1; k <= 1; k++)
                {
                    for (int l = -1; l <= 1; l++)
                    {
                        if (Math.Abs(k) + Math.Abs(l) == 1)
                        {
                            var offset = new Vector2(k * 2f, l * 2f) * _timer.Schedule;
                            spriteBatch.Draw(tex.Value, dimensions.Position() + offset, Main.OurFavoriteColor);
                        }
                    }
                }

                Main.spriteBatch.End(); // End之后Begin恢复原状
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            }

            if (!AutofishPlayer.LocalPlayer.TryGetAutofisher(out var autofisher))
                return;

            var color = Color.White;
            if (!IsActivated(autofisher))
                color = color.MultiplyRGB(Color.White * 0.4f);
            spriteBatch.Draw(tex.Value, dimensions.Position(), color);
        }
    }
}
