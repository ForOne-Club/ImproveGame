using ImproveGame.Common.Animations;
using ImproveGame.Common.Packets.NetAutofisher;
using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.SUIElements;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.GUI
{
    public class AutofisherGUI : ViewBody, ISidedView
    {
        public override bool Display { get => true; set { } }

        public static bool Visible => SidedEventTrigger.IsOpened(UISystem.Instance.AutofisherGUI);

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
        
        public void OnSwapSlide(float factor)
        {
            float widthNext = basePanel.GetDimensions().Width;
            float shownPositionNext = panelLeft;
            float hiddenPositionNext = -widthNext - 40;
                
            basePanel.Left.Set((int)MathHelper.Lerp(hiddenPositionNext, shownPositionNext, factor), 0f);
            basePanel.Recalculate();
        }
        
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

            textPanel = new(new Color(35, 40, 83), new Color(35, 40, 83), rounded: 10)
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
            var filter = new CatchCratesFilter(basePanel).SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchAccessoriesFilter(basePanel).SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchToolsFilter(basePanel).SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchWhiteRarityCatchesFilter(basePanel).SetPos(filtersX, filtersY);
            filtersY += filter.Height.Pixels + 8f;
            Append(filter);
            filter = new CatchNormalCatchesFilter(basePanel).SetPos(filtersX, filtersY);
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
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;

            autofisher.accessory = item;
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, 17).Send(runLocally: false);
            }
        }

        private void ChangeFishingPoleSlot(Item item, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;

            autofisher.fishingPole = item;
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, 15).Send(runLocally: false);
            }
        }

        private void ChangeBaitSlot(Item item, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;

            autofisher.bait = item;
            if (Main.netMode is NetmodeID.MultiplayerClient && !rightClick)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, 16).Send(runLocally: false);
            }
        }

        private void ChangeBaitSlotStack(Item item, int stackChange, bool typeChange)
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;
            if (!typeChange && stackChange != 0)
            {
                ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, 16, stackChange).Send(runLocally: false);
            }
        }

        private void ChangeFishSlot(Item item, int i, bool rightClick)
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;

            autofisher.fish[i] = item;
            if (Main.netMode == NetmodeID.MultiplayerClient && !rightClick)
            {
                ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, (byte)i).Send(runLocally: false);
            }
        }

        private void ChangeFishSlotStack(Item item, int i, int stackChange, bool typeChange)
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (autofisher is null)
                return;
            if (Main.netMode is NetmodeID.MultiplayerClient && !typeChange && stackChange != 0)
            {
                ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, (byte)i, stackChange).Send(runLocally: false);
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
                    RequestItemPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, slotType).Send(runLocally: false);
                }
            }
            else
            {
                SyncFromTileEntity();
            }
        }

        private void SyncFromTileEntity()
        {
            var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
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
            base.Draw(spriteBatch);

            Player player = Main.LocalPlayer;

            if (AutofishPlayer.LocalPlayer.Autofisher is not null)
            {
                var autofisher = AutofishPlayer.LocalPlayer.Autofisher;

                if (baitSlot.Item.type == ItemID.TruffleWorm)
                {
                    autofisher.SetFishingTip(Autofisher.TipType.FishingWarning);
                }
                if (baitSlot.Item.IsAir || fishingPoleSlot.Item.IsAir || autofisher.FishingTip == "Error")
                {
                    autofisher.SetFishingTip(Autofisher.TipType.Unavailable);
                }

                tipText.SetText(autofisher.FishingTip);


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
        public void Open()
        {
            WandSystem.SelectPoolMode = false;
            Main.playerInventory = true;
            title.SetText(GetText("Autofisher.Title"));
            // AutofishPlayer.LocalPlayer.SetAutofisher(point);
            RefreshItems();
        }

        public void Close()
        {
            relocateButton.SetImage(selectPoolOff);
            WandSystem.SelectPoolMode = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public override bool CanPriority(UIElement target) => target != this;

        public override bool CanDisableMouse(UIElement target)
        {
            return (target != this && basePanel.IsMouseHovering) || basePanel.KeepPressed;
        }
    }

    internal class CatchCratesFilter : AutofisherFilterButton
    {
        internal CatchCratesFilter(SUIPanel panel) : base(ItemID.WoodenCrate, panel) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchCrates;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchCrates, 0).Send(runLocally: true);
        }
    }

    internal class CatchAccessoriesFilter : AutofisherFilterButton
    {
        internal CatchAccessoriesFilter(SUIPanel panel) : base(ItemID.FrogLeg, panel) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchAccessories;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchAccessories, 1).Send(runLocally: true);
        }
    }

    internal class CatchToolsFilter : AutofisherFilterButton
    {
        internal CatchToolsFilter(SUIPanel panel) : base(ItemID.CrystalSerpent, panel) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchTools;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchTools, 2).Send(runLocally: true);
        }
    }

    internal class CatchWhiteRarityCatchesFilter : AutofisherFilterButton
    {
        internal CatchWhiteRarityCatchesFilter(SUIPanel panel) : base(ItemID.Bass, panel) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchWhiteRarityCatches;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchWhiteRarityCatches, 3).Send(runLocally: true);
        }
    }

    internal class CatchNormalCatchesFilter : AutofisherFilterButton
    {
        internal CatchNormalCatchesFilter(SUIPanel panel) : base(ItemID.GoldenCarp, panel) { }

        internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchNormalCatches;

        internal override void Clicked(TEAutofisher autofisher)
        {
            FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchNormalCatches, 4).Send(runLocally: true);
        }
    }

    internal abstract class AutofisherFilterButton : UIElement
    {
        internal int ItemType;
        private readonly SUIPanel _panel;
        private readonly AnimationTimer _timer; // 这是一个计时器哦~

        internal virtual bool IsActivated(TEAutofisher autofisher) => true;

        internal virtual void Clicked(TEAutofisher autofisher) { }

        internal AutofisherFilterButton(int itemType, SUIPanel panel)
        {
            ItemType = itemType;
            _panel = panel;
            _timer = new(TimerMax: 90f)
            {
                State = AnimationState.Close
            };
            Main.instance.LoadItem(itemType);
            this.SetSize(TextureAssets.Item[itemType].Size());
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (AutofishPlayer.LocalPlayer.Autofisher is null)
                return;
            Clicked(AutofishPlayer.LocalPlayer.Autofisher);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            _timer.Update();
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            float filtersX = _panel.Left() + _panel.Width() + 10f;
            if (Left.Pixels != filtersX) {
                Left.Set(filtersX, 0f);
                Recalculate();
            }

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
            
            if (AutofishPlayer.LocalPlayer.Autofisher is null)
                return;

            var color = Color.White;
            if (!IsActivated(AutofishPlayer.LocalPlayer.Autofisher))
                color = color.MultiplyRGB(Color.White * 0.4f);
            spriteBatch.Draw(tex.Value, dimensions.Position(), color);
        }
    }
}
