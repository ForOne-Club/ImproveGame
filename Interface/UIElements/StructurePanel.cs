using ImproveGame.Common.ConstructCore;
using ImproveGame.Common.Systems;

namespace ImproveGame.Interface.UIElements
{
    internal class StructurePanel : UIPanel
    {
        public string FilePath { get; private set; }
        public string Name { get; private set; }

        public readonly Color BorderSelectedColor = new(89, 116, 213);
        public readonly Color BorderUnselectedColor = new(39, 46, 100);
        public readonly Color SelectedColor = new(73, 94, 171);
        public readonly Color UnselectedColor = new(62, 80, 146);

        public UIText NameText;
        public UIText PathText;

        public StructurePanel(string filePath) : base()
        {
            FilePath = filePath;

            BorderColor = BorderUnselectedColor;
            BackgroundColor = UnselectedColor;

            Width = StyleDimension.FromPixels(540f);
            string name = FilePath.Split('\\').Last();
            name = name[..^FileOperator.Extension.Length]; // name.Substring(0, name.Length - FileOperator.Extension.Length)
            NameText = new(name, 1.05f)
            {
                Top = StyleDimension.FromPixels(-2f),
                Left = StyleDimension.FromPixels(2f),
                Height = StyleDimension.FromPixels(24f)
            };
            Append(NameText);

            UIHorizontalSeparator separator = new()
            {
                Top = StyleDimension.FromPixels(NameText.Height.Pixels - 4f),
                Width = StyleDimension.FromPercent(1f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            Append(separator);

            UIPanel pathPanel = new()
            {
                Top = new StyleDimension(separator.Height.Pixels + separator.Top.Pixels + 3f, 0f),
                BackgroundColor = new Color(35, 40, 83),
                BorderColor = new Color(35, 40, 83),
                OverflowHidden = true,
                PaddingLeft = 6f,
                PaddingRight = 6f,
                PaddingBottom = 0f,
                PaddingTop = 0f
            };
            pathPanel.SetSize(new(0f, 20f), precentWidth: 1f);
            Append(pathPanel);
            PathText = new($"Path: {FilePath}", 0.7f)
            {
                Left = StyleDimension.FromPixels(2f),
                HAlign = 0f,
                VAlign = 0.5f,
                TextColor = Color.Gray
            };
            pathPanel.Append(PathText);

            Height = StyleDimension.FromPixels(pathPanel.Top.Pixels + pathPanel.Height.Pixels + 18f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                BorderColor = BorderSelectedColor;
                BackgroundColor = SelectedColor;
                NameText.TextColor = Color.White;
            }
            else
            {
                BorderColor = BorderUnselectedColor;
                BackgroundColor = UnselectedColor;
                NameText.TextColor = Color.LightGray;
            }
            if (WandSystem.ConstructFilePath == FilePath)
            {
                NameText.TextColor = new(255, 231, 69);
            }
            SetSizedText();
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (WandSystem.ConstructFilePath == FilePath)
            {
                WandSystem.ConstructFilePath = string.Empty;
                return;
            }
            WandSystem.ConstructFilePath = FilePath;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void SetSizedText()
        {
            var innerDimensions = GetInnerDimensions();
            var font = FontAssets.MouseText.Value;
            float scale = 0.7f;
            float dotWidth = font.MeasureString("...").X * scale;
            float pathWidth = font.MeasureString("Path: ").X * scale;
            if (font.MeasureString(FilePath).X * scale >= innerDimensions.Width - 20f - pathWidth - dotWidth)
            {
                float width = 0f;
                int i;
                for (i = FilePath.Length - 1; i > 0; i--)
                {
                    width += font.MeasureString(FilePath[i].ToString()).X * scale;
                    if (width >= innerDimensions.Width - 20f - pathWidth - dotWidth)
                    {
                        break;
                    }
                }
                PathText.SetText($"Path: ...{FilePath[i..]}");
            }
        }
    }
}
