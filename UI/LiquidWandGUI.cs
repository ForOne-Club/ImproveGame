using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Items;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.UIElements;
using Terraria.UI.Chat;

namespace ImproveGame.UI
{
    public class LiquidWandGUI : BaseBody
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

        private const float PanelLeft = 590f;
        private const float PanelTop = 120f;
        private const float PanelHeight = 148f;
        private const float PanelWidth = 190f;

        public Item CurrentItem;

        public LiquidWand CurrentWand => CurrentItem.ModItem as LiquidWand;

        private SUIPanel basePanel;
        private LiquidWandSlot waterSlot;
        private LiquidWandSlot lavaSlot;
        private LiquidWandSlot honeySlot;
        private UIText title;
        private ModIconTextButton modeButton;

        private static bool _prevMouseRight;
        private static bool _hoveringOnSlots;

        public override void OnInitialize()
        {
            Append(basePanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
            {
                Shaded = true,
                ShadowThickness = UIStyle.ShadowThicknessThinnerer,
                Draggable = true,
                Left = {Pixels = PanelLeft},
                Top = {Pixels = PanelTop},
                Width = {Pixels = PanelWidth},
                Height = {Pixels = PanelHeight}
            });

            const float slotFirst = 0f;
            const float slotSecond = 60f;
            const float slotThird = 120f;

            const float colorHover = 0.6f;
            const float colorNormal = 1f;
            const float xOffst = 2f;

            waterSlot = new LiquidWandSlot(LiquidID.Water, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, colorHover,
                colorNormal);
            waterSlot.Left.Set(slotFirst + xOffst, 0f);
            waterSlot.Top.Set(slotSecond, 0f);
            waterSlot.OnLeftMouseDown += (_, _) =>
            {
                if (WandSystem.LiquidMode != LiquidID.Water)
                {
                    WandSystem.LiquidMode = LiquidID.Water;
                    int iconItemId = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.WaterBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemId}", true);
                }
            };
            basePanel.Append(waterSlot);

            lavaSlot = new(LiquidID.Lava, ItemID.BottomlessLavaBucket, ItemID.LavaAbsorbantSponge, colorHover,
                colorNormal);
            lavaSlot.Left.Set(slotSecond + xOffst, 0f);
            lavaSlot.Top.Set(slotSecond, 0f);
            lavaSlot.OnLeftMouseDown += (_, _) =>
            {
                if (WandSystem.LiquidMode != LiquidID.Lava)
                {
                    WandSystem.LiquidMode = LiquidID.Lava;
                    int iconItemId = WandSystem.AbsorptionMode ? ItemID.LavaAbsorbantSponge : ItemID.LavaBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemId}", true);
                }
            };
            basePanel.Append(lavaSlot);

            honeySlot = new(LiquidID.Honey, ItemID.BottomlessHoneyBucket, ItemID.HoneyAbsorbantSponge, colorHover, colorNormal);
            honeySlot.Left.Set(slotThird + xOffst, 0f);
            honeySlot.Top.Set(slotSecond, 0f);
            honeySlot.OnLeftMouseDown += (_, _) =>
            {
                if (WandSystem.LiquidMode != LiquidID.Honey)
                {
                    WandSystem.LiquidMode = LiquidID.Honey;
                    int iconItemId = WandSystem.AbsorptionMode ? ItemID.HoneyAbsorbantSponge : ItemID.HoneyBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemId}", true);
                }
            };
            basePanel.Append(honeySlot);

            // 头顶大字
            title = new("Materials", 0.5f, large: true)
            {
                HAlign = 0.5f
            };
            title.Left.Set(0, 0f);
            title.Top.Set(-40, 0f);
            title.Width.Set(PanelWidth, 0f);
            title.Height.Set(40, 0f);
            basePanel.Append(title);

            // 使用模式修改按钮
            modeButton = new(Language.GetText("Mods.ImproveGame.Common.Switch"), Color.White,
                $"Images/Item_{ItemID.WaterBucket}");
            modeButton.Left.Set(slotFirst, 0f);
            modeButton.Top.Set(slotFirst, 0f);
            modeButton.Width.Set(166f, 0f);
            modeButton.Height.Set(42f, 0f);
            modeButton.OnLeftMouseDown += (_, _) =>
            {
                WandSystem.ChangeAbsorptionMode();
                SetIconTexture();
            };
            basePanel.Append(modeButton);
        }

        /// <summary>
        /// 尝试更改某槽液体的数量
        /// </summary>
        /// <param name="liquidType">液体ID，选择哪个槽的依据</param>
        /// <param name="addAmount">更改数量</param>
        /// <param name="store">true则使用存液体模式，false则使用放液体模式</param>
        /// <param name="ignoreLiquidMode">是否无视<see cref="WandSystem.LiquidMode"/>判断</param>
        public void TryChangeLiquidAmount(byte liquidType, ref byte addAmount, bool store,
            bool ignoreLiquidMode = false)
        {
            //Main.NewText($"TryChangeLiquidAmount");
            if (liquidType != WandSystem.LiquidMode && !ignoreLiquidMode)
                return;

            foreach (var slot in from u in basePanel.Children
                     where u is LiquidWandSlot s && s.LiquidId == liquidType
                     select u as LiquidWandSlot)
            {
                if (store) slot.StoreLiquid(ref addAmount);
                else slot.TakeLiquid(ref addAmount);

                var wand = CurrentWand;

                ref float wandLiquid = ref wand.Water;
                switch ((short)liquidType)
                {
                    case LiquidID.Lava:
                        wandLiquid = ref wand.Lava;
                        break;
                    case LiquidID.Honey:
                        wandLiquid = ref wand.Honey;
                        break;
                }

                wandLiquid = slot.GetLiquidAmount();
            }
        }

        // 主要是可拖动和一些判定吧
        public override void Update(GameTime gameTime)
        {
            if (CurrentItem?.ModItem is not LiquidWand)
            {
                Close();
                return;
            }

            _hoveringOnSlots = false;

            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                Close();
                return;
            }

            // 右键点击空白直接关闭
            if (Main.mouseRight && !_prevMouseRight && basePanel.IsMouseHovering && !_hoveringOnSlots)
            {
                Close();
                return;
            }

            _prevMouseRight = Main.mouseRight;

            string hoverText = "";

            foreach (var slot in from u in basePanel.Children where u is LiquidWandSlot select u as LiquidWandSlot)
            {
                slot.IsAltHovering = false;

                if (!slot.IsMouseHovering)
                {
                    continue;
                }

                string liquid = slot.LiquidId switch
                {
                    LiquidID.Water => "Water",
                    LiquidID.Lava => "Lava",
                    LiquidID.Honey => "Honey",
                    _ => throw new ArgumentOutOfRangeException()
                };
                string amount = slot.Infinite == -1 ? $"{slot.GetLiquidAmount():p1}" : "∞";
                hoverText = GetTextWith($"LiquidWand.{liquid}", new { LiquidAmount = amount });
                break;
            }

            foreach (var slot in from u in basePanel.Children where u is LiquidWandSlot select u as LiquidWandSlot)
            {
                // 如果遮挡到百分比文本，就虚化百分比文本
                var textLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, hoverText, Vector2.One)
                    .ToPoint();
                var slotRect = slot.GetDimensions().ToRectangle();
                slotRect.Height = 24;
                slotRect.Y += 50;
                var textRect = new Rectangle(Main.mouseX + 10, Main.mouseY + 10, textLength.X, textLength.Y);
                if (slotRect.Intersects(textRect))
                {
                    slot.IsAltHovering = true;
                }
            }

            if (hoverText is not "")
            {
                Main.instance.MouseText(hoverText);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen))
            {
                player.mouseInterface = true;
            }

            string hoverText = "";

            foreach (var slot in from u in basePanel.Children where u is LiquidWandSlot select u as LiquidWandSlot)
            {
                if (!slot.IsMouseHovering)
                    continue;

                string liquid = slot.LiquidId switch
                {
                    LiquidID.Water => "Water",
                    LiquidID.Lava => "Lava",
                    LiquidID.Honey => "Honey",
                    _ => throw new ArgumentOutOfRangeException()
                };
                string amount = slot.Infinite == -1 ? $"{slot.GetLiquidAmount():p1}" : "∞";
                hoverText = GetTextWith($"LiquidWand.{liquid}", new { LiquidAmount = amount });
                break;
            }

            if (hoverText is not "")
                Main.instance.MouseText(hoverText);
        }

        public void SetIconTexture()
        {
            int iconItemId = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.WaterBucket;
            switch (WandSystem.LiquidMode)
            {
                case LiquidID.Lava:
                    iconItemId = WandSystem.AbsorptionMode ? ItemID.LavaAbsorbantSponge : ItemID.LavaBucket;
                    break;
                case LiquidID.Honey:
                    iconItemId = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.HoneyBucket;
                    break;
                //case LiquidID.Shimmer:
                //    iconItemId = WandSystem.AbsorptionMode ? ItemID.ShimmerAbsorbantSponge : ItemID.BottomlessShimmerBucket;
                //    break;
            }

            modeButton.SetIconTexture($"Images/Item_{iconItemId}", true);
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(LiquidWand wand)
        {
            Main.playerInventory = true;
            _prevMouseRight = true; // 防止一打开就关闭
            Visible = true;
            basePanel.Dragging = false;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentItem = wand.Item;

            // 设置
            waterSlot.SetLiquidAmount(CurrentWand.Water);
            lavaSlot.SetLiquidAmount(CurrentWand.Lava);
            honeySlot.SetLiquidAmount(CurrentWand.Honey);
            // 设置图标
            SetIconTexture();

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            title.SetText(Language.GetText("Mods.ImproveGame.LiquidWand.Title"));
            modeButton.SetText(Language.GetText("Mods.ImproveGame.Common.Switch"), 1f, Color.White);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close()
        {
            CurrentItem = null;
            Visible = false;
            _prevMouseRight = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}