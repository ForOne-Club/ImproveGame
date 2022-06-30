using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class BuffTrackerGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;

        private bool HoverOnClose;
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
        }

        private void TryForwardPage(UIMouseEvent evt, UIElement listeningElement) {
            int count = ApplyBuffItem.BuffTypesShouldHide.Count;
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
            if (HoverOnClose) return;

            var dimensions = listeningElement.GetDimensions().ToRectangle();
            Offset = new Vector2(evt.MousePosition.X - dimensions.Left, evt.MousePosition.Y - dimensions.Top);
            Dragging = true;
        }

        // 可拖动界面
        private void DragEnd(UIMouseEvent evt, UIElement listeningElement) {
            if (HoverOnClose || !Dragging) return;

            Vector2 end = evt.MousePosition;
            Dragging = false;

            listeningElement.Left.Set(end.X - Offset.X, 0f);
            listeningElement.Top.Set(end.Y - Offset.Y, 0f);

            listeningElement.Recalculate();
        }

        public override void Update(GameTime gameTime) {
            ApplyBuffItem.BuffTypesShouldHide.Sort(); // 升序排序
            // 去掉重复的
            for (int i = 1; i < ApplyBuffItem.BuffTypesShouldHide.Count; i++) {
                if (ApplyBuffItem.BuffTypesShouldHide[i] == ApplyBuffItem.BuffTypesShouldHide[i - 1]) {
                    ApplyBuffItem.BuffTypesShouldHide.RemoveAt(i);
                    i--;
                }
            }

            int count = ApplyBuffItem.BuffTypesShouldHide.Count;
            while (count <= page * 44) {
                page--;
            }
            SetPageText(page);

            base.Update(gameTime);

            if (Dragging) {
                basePanel.Left.Set(Main.mouseX - Offset.X, 0f);
                basePanel.Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
            }
        }

        public void SetPageText(int page) {
            int count = ApplyBuffItem.BuffTypesShouldHide.Count;
            int currentPageMaxBuffIndex = Math.Min(page * 44 + 45, count);
            pageText.SetText($"{page * 44 + 1} - {currentPageMaxBuffIndex} ({count})");
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (BuffHoverBorder is null)
                BuffHoverBorder = MyUtils.GetTexture("UI/Buff_HoverBorder");

            Player player = Main.LocalPlayer;

            base.Draw(spriteBatch);

            if (basePanel.ContainsPoint(Main.MouseScreen)) {
                player.mouseInterface = true;
            }

            var panelDimensions = basePanel.GetDimensions();
            bool hoverOnBuff = false;
            int viewMax = Math.Min(page * 44 + 43, ApplyBuffItem.BuffTypesShouldHide.Count);
            for (int i = page * 44; i < viewMax; i++) {
                int x = 14 + i * 38;
                int y = 56;
                if (i >= 11) {
                    x = 14 + Math.Abs(i % 11) * 38;
                    y += 40 * (i / 11);
                }

                int buffType = ApplyBuffItem.BuffTypesShouldHide[i];
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
            ApplyBuffItem.BuffTypesShouldHide.Sort(); // 升序排序
            // 去掉重复的
            for (int i = 1; i < ApplyBuffItem.BuffTypesShouldHide.Count; i++) {
                if (ApplyBuffItem.BuffTypesShouldHide[i] == ApplyBuffItem.BuffTypesShouldHide[i - 1]) {
                    ApplyBuffItem.BuffTypesShouldHide.RemoveAt(i);
                    i--;
                }
            }
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
