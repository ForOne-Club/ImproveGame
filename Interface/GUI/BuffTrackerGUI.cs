using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace ImproveGame.Interface.GUI
{
    public class BuffTrackerGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        private bool HoverOnBuff;
        private bool Dragging;
        private Vector2 Offset;
        /// <summary>
        /// 当前是哪一页（一页44个Buff）
        /// </summary>
        private static int page = 0;

        private UIPanel basePanel;
        private UIText title;
        private UIText pageText;
        private Asset<Texture2D> BuffHoverBorder;
        internal BuffTrackerBattler BuffTrackerBattler;

        public override void OnInitialize() {
            panelLeft = 630f;
            panelTop = 160f;
            panelHeight = 220f;
            panelWidth = 436f;

            basePanel = new UIPanel();
            basePanel.Left.Set(panelLeft, 0f);
            basePanel.Top.Set(panelTop, 0f);
            basePanel.Width.Set(panelWidth, 0f);
            basePanel.Height.Set(panelHeight, 0f);
            basePanel.OnMouseDown += DragStart;
            basePanel.OnMouseUp += DragEnd;
            Append(basePanel);

            UIImageButton backButton = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Back"));
            backButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Border"));
            backButton.SetVisibility(1f, 1f);
            backButton.SetSnapPoint("BackPage", 0);
            backButton.OnMouseDown += TryBackPage;
            basePanel.Append(backButton);

            UIImageButton forwardButton = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Forward")) {
                Left = new StyleDimension(backButton.Width.Pixels + 1f, 0f),
                Top = backButton.Top
            };
            forwardButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Border"));
            forwardButton.SetVisibility(1f, 1f);
            forwardButton.SetSnapPoint("NextPage", 0);
            forwardButton.OnMouseDown += TryForwardPage;
            basePanel.Append(forwardButton);

            UIPanel textPanel = new() {
                Left = new StyleDimension(backButton.Width.Pixels + 1f + forwardButton.Width.Pixels + 3f, 0f),
                Width = new StyleDimension(135f, 0f),
                Height = backButton.Height,
                BackgroundColor = new Color(35, 40, 83),
                BorderColor = new Color(35, 40, 83)
            };
            textPanel.SetPadding(0f);
            basePanel.Append(textPanel);
            pageText = new("9000-9999 (9001)", 0.8f) {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            textPanel.Append(pageText);

            UIImageButton closeButton = new(MyUtils.GetTexture("UI/Button_Close")) {
                Left = new StyleDimension(-28f, 1f),
                Top = backButton.Top,
                Width = StyleDimension.FromPixels(28f),
                Height = StyleDimension.FromPixels(28f)
            };
            closeButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Border"));
            closeButton.SetVisibility(1f, 1f);
            closeButton.SetSnapPoint("ExitButton", 0);
            closeButton.OnMouseDown += (UIMouseEvent evt, UIElement e) => Close();
            basePanel.Append(closeButton);

            // 标题
            string titleText = "Buff Tracker";
            int titleWidth = (int)(FontAssets.DeathText.Value.MeasureString(titleText).X * 0.5f);
            title = new(titleText, 0.5f, large: true) {
                Left = new StyleDimension(-126f - titleWidth * 0.5f, 1f),
                Top = StyleDimension.FromPixels(8f),
                Width = StyleDimension.FromPixels(titleWidth),
                Height = StyleDimension.FromPixels(28f)
            };
            basePanel.Append(title);

            UIHorizontalSeparator separator = new() {
                Top = StyleDimension.FromPixels(backButton.Width.Pixels + 5f),
                Width = StyleDimension.FromPercent(1f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            basePanel.Append(separator);

            BuffTrackerBattler = new();
            BuffTrackerBattler.Initialize();
            BuffTrackerBattler.MainPanel.Left = new StyleDimension(16f, 1f);
            BuffTrackerBattler.MainPanel.Top = StyleDimension.FromPixels(0f);
            basePanel.Append(BuffTrackerBattler.MainPanel);
        }

        private void TryForwardPage(UIMouseEvent evt, UIElement listeningElement) {
            int count = HideBuffSystem.HideBuffCount();
            if (page * 44 < count) {
                page++;
            }
            SetPageText(page);
        }

        private void TryBackPage(UIMouseEvent evt, UIElement listeningElement) {
            if (page > 0) {
                page--;
            }
            SetPageText(page);
        }

        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement) {
            var dimensions = listeningElement.GetDimensions().ToRectangle();
            Offset = new Vector2(evt.MousePosition.X - dimensions.Left, evt.MousePosition.Y - dimensions.Top);
            Dragging = true;
        }

        // 可拖动界面
        private void DragEnd(UIMouseEvent evt, UIElement listeningElement) {
            if (!Dragging) return;

            Vector2 end = evt.MousePosition;
            Dragging = false;

            listeningElement.Left.Set(end.X - Offset.X, 0f);
            listeningElement.Top.Set(end.Y - Offset.Y, 0f);

            listeningElement.Recalculate();
        }

        public override void Update(GameTime gameTime) {
            int count = HideBuffSystem.HideBuffCount();
            if (count > 0) {
                while (count <= page * 44) {
                    page--;
                }
                SetPageText(page);
            }
            else {
                pageText.SetText(MyUtils.GetText("Common.Unavailable"));
            }

            base.Update(gameTime);
            BuffTrackerBattler.Update();

            if (Dragging) {
                basePanel.Left.Set(Main.mouseX - Offset.X, 0f);
                basePanel.Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
            }
        }

        public void SetPageText(int page) {
            int count = HideBuffSystem.HideBuffCount();
            int currentPageMaxBuffIndex = Math.Min(page * 44 + 44, count);
            pageText.SetText($"{page * 44 + 1} - {currentPageMaxBuffIndex} ({count})");
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (BuffHoverBorder is null)
                BuffHoverBorder = MyUtils.GetTexture("UI/Buff_HoverBorder");

            Player player = Main.LocalPlayer;
            if (player.dead || !player.active) {
                Close();
                return;
            }

            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen)) {
                player.mouseInterface = true;
            }

            var panelDimensions = basePanel.GetDimensions();
            bool hoverOnBuff = false;
            int viewMax = Math.Min(43, HideBuffSystem.HideBuffCount() - 1);

            if (viewMax == -1) { // 没Buff 显示提示
                Vector2 drawCenter = panelDimensions.Center();
                drawCenter.Y += 10f; // 加上顶栏的一半高度，保证绘制在下面区域的中央
                float scale = 0.5f;
                string text = MyUtils.GetText("BuffTracker.NoInfBuff");
                // 设置都没开，加个提示
                if (!MyUtils.Config.NoConsume_Potion) {
                    string textAlt = $"{MyUtils.GetText("BuffTracker.NoInfBuffAlt")}";
                    float height = FontAssets.DeathText.Value.MeasureString(textAlt).Y * 0.5f;
                    drawCenter.Y += height * 0.5f;
                    float textAltWidth = FontAssets.DeathText.Value.MeasureString(textAlt).X * 0.5f;
                    Vector2 originAlt = new(textAltWidth, 0f);
                    Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.DeathText.Value, textAlt, drawCenter.X, drawCenter.Y, Color.White, Color.Black, originAlt, scale);
                    drawCenter.Y -= height;
                }
                float textWidth = FontAssets.DeathText.Value.MeasureString(text).X * 0.5f;
                Vector2 origin = new(textWidth, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.DeathText.Value, text, drawCenter.X, drawCenter.Y, Color.White, Color.Black, origin, scale);
                return;
            }

            int buffCount = -1;
            for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++) {
                if (!HideBuffSystem.BuffTypesShouldHide[i])
                    continue;
                buffCount++;
                if (buffCount > viewMax + page * 44)
                    break;
                if (buffCount < page * 44)
                    continue;
                int buffPageCount = buffCount - page * 44;
                int x = 14 + buffPageCount * 38;
                int y = 56;
                if (buffPageCount >= 11) {
                    x = 14 + Math.Abs(buffPageCount % 11) * 38;
                    y += 40 * (buffPageCount / 11);
                }

                int buffType = i;
                bool buffEnabled = InfBuffPlayer.Get(Main.LocalPlayer).CheckInfBuffEnable(buffType);

                Vector2 drawPosition = new(x + panelDimensions.X, y + panelDimensions.Y);
                Asset<Texture2D> buffAsset = TextureAssets.Buff[buffType];
                Texture2D texture = buffAsset.Value;
                int width = buffAsset.Width();
                int height = buffAsset.Height();
                float grayScale = 1f;
                if (!buffEnabled) {
                    grayScale = 0.4f;
                }
                spriteBatch.Draw(texture, drawPosition, new Color(grayScale, grayScale, grayScale));

                Rectangle mouseRectangle = new((int)drawPosition.X, (int)drawPosition.Y, width, height);
                if (mouseRectangle.Intersects(new Rectangle(Main.mouseX, Main.mouseY, 1, 1))) {
                    hoverOnBuff = true;
                    // 绘制边框
                    drawPosition.X -= 2;
                    drawPosition.Y -= 2;
                    spriteBatch.Draw(BuffHoverBorder.Value, drawPosition, Color.White);

                    string mouseText = Lang.GetBuffName(buffType);
                    if (buffEnabled) {
                        mouseText += MyUtils.GetText("BuffTracker.LeftClickDisable");
                    }
                    else {
                        mouseText += MyUtils.GetText("BuffTracker.LeftClickEnable");
                    }
                    Main.instance.MouseText(mouseText);

                    if (Main.mouseLeft && Main.mouseLeftRelease) {
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        InfBuffPlayer.Get(Main.LocalPlayer).ToggleInfBuff(buffType);
                    }
                }
            }
            // 指针移入的声音（移开没声音）
            if (hoverOnBuff && !HoverOnBuff) {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            HoverOnBuff = hoverOnBuff;
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open() {
            Dragging = false;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            title.SetText(MyUtils.GetText("BuffTracker.Title"));
            int titleWidth = (int)(FontAssets.DeathText.Value.MeasureString(title.Text).X * 0.5f);
            title.Left = new StyleDimension(-120f - titleWidth * 0.5f, 1f);
            title.Width = StyleDimension.FromPixels(titleWidth);

            SetPageText(page);
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
