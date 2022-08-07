using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI
{
    public class LiquidWandGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        public int CurrentSlot;
        public Item CurrentItem
        {
            get => Main.LocalPlayer.inventory[CurrentSlot];
            set => Main.LocalPlayer.inventory[CurrentSlot] = value;
        }
        public LiquidWand CurrentWand => CurrentItem.ModItem as LiquidWand;

        private ModUIPanel basePanel;
        private LiquidWandSlot waterSlot;
        private LiquidWandSlot lavaSlot;
        private LiquidWandSlot honeySlot;
        private UIText title;
        private ModIconTextButton modeButton;

        private static bool PrevMouseRight;
        private static bool HoveringOnSlots;
        private static bool SpecialClickSlot;

        private static int LeftMouseTimer = 0;

        public override void OnInitialize()
        {
            panelLeft = 600f;
            panelTop = 80f;
            panelHeight = 148f;
            panelWidth = 190f;

            basePanel = new ModUIPanel();
            basePanel.Left.Set(panelLeft, 0f);
            basePanel.Top.Set(panelTop, 0f);
            basePanel.Width.Set(panelWidth, 0f);
            basePanel.Height.Set(panelHeight, 0f);
            Append(basePanel);

            const float slotFirst = 0f;
            const float slotSecond = 60f;
            const float slotThird = 120f;

            const float colorHover = 0.6f;
            const float colorNormal = 1f;
            const float Xoffst = 2f;

            waterSlot = new(LiquidID.Water, colorHover, colorNormal);
            waterSlot.Left.Set(slotFirst + Xoffst, 0f);
            waterSlot.Top.Set(slotSecond, 0f);
            waterSlot.Width.Set(40f, 0f);
            waterSlot.Height.Set(40f, 0f);
            waterSlot.OnMouseDown += (_, _) =>
            {
                LeftMouseTimer = 0;
                if (WandSystem.LiquidMode != LiquidID.Water && !SpecialClickSlot)
                {
                    WandSystem.LiquidMode = LiquidID.Water;
                    int iconItemID = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.WaterBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemID}", true);
                }
            };
            basePanel.Append(waterSlot);

            lavaSlot = new(LiquidID.Lava, colorHover, colorNormal);
            lavaSlot.Left.Set(slotSecond + Xoffst, 0f);
            lavaSlot.Top.Set(slotSecond, 0f);
            lavaSlot.Width.Set(40f, 0f);
            lavaSlot.Height.Set(40f, 0f);
            lavaSlot.OnMouseDown += (_, _) =>
            {
                LeftMouseTimer = 0;
                if (WandSystem.LiquidMode != LiquidID.Lava && !SpecialClickSlot)
                {
                    WandSystem.LiquidMode = LiquidID.Lava;
                    int iconItemID = WandSystem.AbsorptionMode ? ItemID.LavaAbsorbantSponge : ItemID.LavaBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemID}", true);
                }
            };
            basePanel.Append(lavaSlot);

            honeySlot = new(LiquidID.Honey, colorHover, colorNormal);
            honeySlot.Left.Set(slotThird + Xoffst, 0f);
            honeySlot.Top.Set(slotSecond, 0f);
            honeySlot.Width.Set(40f, 0f);
            honeySlot.Height.Set(40f, 0f);
            honeySlot.OnMouseDown += (_, _) =>
            {
                LeftMouseTimer = 0;
                if (WandSystem.LiquidMode != LiquidID.Honey && !SpecialClickSlot)
                {
                    WandSystem.LiquidMode = LiquidID.Honey;
                    // 1.4.4出来之后把这块换成蜂蜜吸水棉
                    int iconItemID = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.HoneyBucket;
                    modeButton.SetIconTexture($"Images/Item_{iconItemID}", true);
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
            title.Width.Set(panelWidth, 0f);
            title.Height.Set(40, 0f);
            basePanel.Append(title);

            // 使用模式修改按钮
            modeButton = new(Language.GetText("Mods.ImproveGame.Common.Switch"), Color.White, $"Images/Item_{ItemID.WaterBucket}");
            modeButton.Left.Set(slotFirst, 0f);
            modeButton.Top.Set(slotFirst, 0f);
            modeButton.Width.Set(166f, 0f);
            modeButton.Height.Set(42f, 0f);
            modeButton.OnMouseDown += (UIMouseEvent _, UIElement _) =>
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
        public void TryChangeLiquidAmount(byte liquidType, ref byte addAmount, bool store, bool ignoreLiquidMode = false)
        {
            //Main.NewText($"TryChangeLiquidAmount");
            if (liquidType != WandSystem.LiquidMode && !ignoreLiquidMode)
                return;
            switch (liquidType)
            {
                case LiquidID.Water:
                    if (store) waterSlot.StoreLiquid(ref addAmount);
                    else waterSlot.TakeLiquid(ref addAmount);
                    CurrentWand.Water = waterSlot.GetLiquidAmount();
                    if (CurrentSlot == 58 && Main.mouseItem.ModItem is not null && Main.mouseItem.ModItem is LiquidWand)
                        (Main.mouseItem.ModItem as LiquidWand).Water = waterSlot.GetLiquidAmount();
                    break;
                case LiquidID.Lava:
                    if (store) lavaSlot.StoreLiquid(ref addAmount);
                    else lavaSlot.TakeLiquid(ref addAmount);
                    CurrentWand.Lava = lavaSlot.GetLiquidAmount();
                    if (CurrentSlot == 58 && Main.mouseItem.ModItem is not null && Main.mouseItem.ModItem is LiquidWand)
                        (Main.mouseItem.ModItem as LiquidWand).Lava = lavaSlot.GetLiquidAmount();
                    break;
                case LiquidID.Honey:
                    if (store) honeySlot.StoreLiquid(ref addAmount);
                    else honeySlot.TakeLiquid(ref addAmount);
                    CurrentWand.Honey = honeySlot.GetLiquidAmount();
                    if (CurrentSlot == 58 && Main.mouseItem.ModItem is not null && Main.mouseItem.ModItem is LiquidWand)
                        (Main.mouseItem.ModItem as LiquidWand).Honey = honeySlot.GetLiquidAmount();
                    break;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, CurrentSlot, Main.LocalPlayer.inventory[CurrentSlot].prefix);
            }
        }

        // 主要是可拖动和一些判定吧
        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.inventory.IndexInRange(CurrentSlot) || CurrentItem is null || CurrentItem.ModItem is not LiquidWand)
            {
                Close();
                return;
            }

            HoveringOnSlots = false;

            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                Close();
                return;
            }

            // 右键点击空白直接关闭
            if (Main.mouseRight && !PrevMouseRight && basePanel.IsMouseHovering && !HoveringOnSlots)
            {
                Close();
                return;
            }

            PrevMouseRight = Main.mouseRight;

            waterSlot.IsAltHovering = false;
            lavaSlot.IsAltHovering = false;
            honeySlot.IsAltHovering = false;
            SpecialClickSlot = false;

            string hoverText = "";
            int mode = -1;
            LiquidWandSlot listeningSlot = null;

            if (waterSlot.IsMouseHovering)
            {
                hoverText = MyUtils.GetTextWith("LiquidWand.Water", new { LiquidAmount = $"{waterSlot.GetLiquidAmount():p1}" });
                // 如果遮挡到百分比文本，就虚化百分比文本
                int lengthToMouse = Main.mouseX - (int)waterSlot.GetDimensions().X;
                waterSlot.IsAltHovering = lengthToMouse <= 21;
                lavaSlot.IsAltHovering = true;
                honeySlot.IsAltHovering = lengthToMouse > 8;
                // 附加文本
                if (!Main.mouseItem.IsAir)
                {
                    switch (Main.mouseItem.type)
                    {
                        case ItemID.EmptyBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickOut");
                            listeningSlot = waterSlot;
                            mode = 0;
                            SpecialClickSlot = true;
                            break;
                        case ItemID.WaterBucket:
                        case ItemID.BottomlessBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickIn");
                            listeningSlot = waterSlot;
                            mode = 1;
                            SpecialClickSlot = true;
                            break;
                        case ItemID.SuperAbsorbantSponge:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickAbsorbant");
                            listeningSlot = waterSlot;
                            mode = 2;
                            SpecialClickSlot = true;
                            break;
                    }
                }
            }

            if (lavaSlot.IsMouseHovering)
            {
                hoverText = MyUtils.GetTextWith("LiquidWand.Lava", new { LiquidAmount = $"{lavaSlot.GetLiquidAmount():p1}" });
                // 如果遮挡到百分比文本，就虚化百分比文本
                int lengthToMouse = Main.mouseX - (int)lavaSlot.GetDimensions().X;
                lavaSlot.IsAltHovering = lengthToMouse <= 21;
                honeySlot.IsAltHovering = true;
                // 附加文本
                if (!Main.mouseItem.IsAir)
                {
                    switch (Main.mouseItem.type)
                    {
                        case ItemID.EmptyBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickOut");
                            listeningSlot = lavaSlot;
                            mode = 0;
                            SpecialClickSlot = true;
                            break;
                        case ItemID.LavaBucket:
                        case ItemID.BottomlessLavaBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickIn");
                            listeningSlot = lavaSlot;
                            mode = 1;
                            SpecialClickSlot = true;
                            break;
                        case ItemID.LavaAbsorbantSponge:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickAbsorbant");
                            listeningSlot = lavaSlot;
                            mode = 2;
                            SpecialClickSlot = true;
                            break;
                    }
                }
            }

            if (honeySlot.IsMouseHovering)
            {
                hoverText = MyUtils.GetTextWith("LiquidWand.Honey", new { LiquidAmount = $"{honeySlot.GetLiquidAmount():p1}" });
                // 如果遮挡到百分比文本，就虚化百分比文本
                int lengthToMouse = Main.mouseX - (int)honeySlot.GetDimensions().X;
                honeySlot.IsAltHovering = lengthToMouse <= 21;
                // 附加文本（还没有无底蜂蜜桶和蜂蜜吸收棉呢，不过我看1.4.4马上就会有了，而且也是这个ID名）
                if (!Main.mouseItem.IsAir)
                {
                    switch (Main.mouseItem.type)
                    {
                        case ItemID.EmptyBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickOut");
                            listeningSlot = honeySlot;
                            mode = 0;
                            SpecialClickSlot = true;
                            break;
                        case ItemID.HoneyBucket:
                            //case ItemID.BottomlessHoneyBucket:
                            hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickIn");
                            listeningSlot = honeySlot;
                            mode = 1;
                            SpecialClickSlot = true;
                            break;
                            //case ItemID.LavaAbsorbantSponge:
                            //    hoverText += "\n" + MyUtils.GetText("LiquidWand.ClickAbsorbant");
                            //    SpecialClickSlot = true;
                            //    mode = 2;
                            //    break;
                    }
                }
            }

            if (hoverText is not "")
            {
                Main.instance.MouseText(hoverText);
            }

            if (listeningSlot is not null && mode is not -1 && SpecialClickSlot && Main.mouseLeft)
            {
                if (LeftMouseTimer >= 60)
                    ApplySpecialClick(mode, listeningSlot);
                else if (LeftMouseTimer >= 30 && LeftMouseTimer % 3 == 0)
                    ApplySpecialClick(mode, listeningSlot);
                else if (LeftMouseTimer >= 15 && LeftMouseTimer % 6 == 0)
                    ApplySpecialClick(mode, listeningSlot);
                else if (LeftMouseTimer == 0)
                    ApplySpecialClick(mode, listeningSlot);
                LeftMouseTimer++;
            }

            // 正在放东西，别拉了
            if (basePanel.Dragging && SpecialClickSlot)
            {
                basePanel.Dragging = false;
            }
        }

        public void ApplySpecialClick(int mode, LiquidWandSlot slot)
        {
            switch (mode)
            {
                case 0: // 空桶带走
                    // 传进去的是液体需求量，所以这里假装我们需要255个液体则要传0
                    byte reduceNumber = 0;
                    TryChangeLiquidAmount(slot.LiquidID, ref reduceNumber, false, true);
                    // 水量太小了，不提供新的一桶
                    if (reduceNumber <= 20)
                        return;
                    Main.mouseItem.stack--;
                    switch (slot.LiquidID)
                    {
                        case LiquidID.Water:
                            Main.LocalPlayer.PutItemInInventoryFromItemUsage(ItemID.WaterBucket, Main.LocalPlayer.selectedItem);
                            break;
                        case LiquidID.Lava:
                            Main.LocalPlayer.PutItemInInventoryFromItemUsage(ItemID.LavaBucket, Main.LocalPlayer.selectedItem);
                            break;
                        case LiquidID.Honey:
                            Main.LocalPlayer.PutItemInInventoryFromItemUsage(ItemID.HoneyBucket, Main.LocalPlayer.selectedItem);
                            break;
                    }
                    break;
                case 1: // 液体桶尝试放进去
                    byte addNumber = 255;
                    TryChangeLiquidAmount(slot.LiquidID, ref addNumber, true, true);
                    // 没变，说明没必要加了
                    if (addNumber == 255)
                    {
                        break;
                    }

                    if (Main.mouseItem.UseSound is not null)
                    {
                        SoundEngine.PlaySound(Main.mouseItem.UseSound.Value);
                    }

                    // 后面是减少堆叠和放空桶
                    if (Main.mouseItem.type == ItemID.BottomlessBucket || Main.mouseItem.type == ItemID.BottomlessLavaBucket/* || Main.mouseItem.type == ItemID.BottomlessHoneyBucket*/)
                        break;

                    Main.mouseItem.stack--;
                    Main.LocalPlayer.PutItemInInventoryFromItemUsage(ItemID.EmptyBucket, Main.LocalPlayer.selectedItem);
                    break;
                case 2: // 直接吸取
                    // 传进去的是液体需求量，所以这里假装我们需要255个液体则要传0
                    byte absorbant = 0;
                    TryChangeLiquidAmount(slot.LiquidID, ref absorbant, false, true);
                    break;
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
        }

        public void SetIconTexture()
        {
            int iconItemID = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.WaterBucket;
            switch (WandSystem.LiquidMode)
            {
                case LiquidID.Lava:
                    iconItemID = WandSystem.AbsorptionMode ? ItemID.LavaAbsorbantSponge : ItemID.LavaBucket;
                    break;
                case LiquidID.Honey:
                    iconItemID = WandSystem.AbsorptionMode ? ItemID.SuperAbsorbantSponge : ItemID.HoneyBucket;
                    break;
            }
            modeButton.SetIconTexture($"Images/Item_{iconItemID}", true);
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(int setSlotIndex = -1)
        {
            Main.playerInventory = true;
            PrevMouseRight = true; // 防止一打开就关闭
            Visible = true;
            basePanel.Dragging = false;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            CurrentSlot = Main.LocalPlayer.selectedItem;
            if (setSlotIndex is not -1)
            {
                CurrentSlot = setSlotIndex;
            }

            // 设置
            waterSlot.SetLiquidAmount(CurrentWand.Water);
            lavaSlot.SetLiquidAmount(CurrentWand.Lava);
            honeySlot.SetLiquidAmount(CurrentWand.Honey);
            // 设置图标
            SetIconTexture();

            // 关掉本Mod其他的同类UI
            if (ArchitectureGUI.Visible) UISystem.Instance.ArchitectureGUI.Close();

            // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
            title.SetText(Language.GetText("Mods.ImproveGame.LiquidWand.Title"));
            modeButton.SetText(Language.GetText("Mods.ImproveGame.Common.Switch"), 1f, Color.White);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close()
        {
            CurrentSlot = -1;
            Visible = false;
            PrevMouseRight = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
