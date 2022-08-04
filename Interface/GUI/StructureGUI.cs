using ImproveGame.Common.ConstructCore;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.UIElements_Shader;
using System.Diagnostics;
using System.Reflection;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class StructureGUI : UIState
    {
        public static bool Visible { get; private set; }
        public bool CacheSetupStructures; // 缓存，在下一帧Setup

        public Asset<Texture2D> ButtonPanel;
        public Asset<Texture2D> ButtonPanel_Highlight;

        public UserInterface UserInterface;

        private ModUIPanel BasePanel; // 背景板
        public ModScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表

        public override void OnInitialize()
        {
            ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
            ButtonPanel_Highlight = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

            BasePanel = new(draggable: false)
            {
                Top = StyleDimension.FromPixels(90f),
                HAlign = 0.5f
            };
            BasePanel.SetSize(600f, 0f, precentHeight: 0.8f);
            BasePanel.BorderColor = new(29, 34, 70);
            Append(BasePanel);

            UIList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
            };
            UIList.SetPadding(2f);
            BasePanel.Append(UIList);

            Scrollbar = new(UserInterface)
            {
                HAlign = 1f,
                VAlign = 0.5f
            };
            Scrollbar.Left.Set(-2f, 0f);
            Scrollbar.Height.Set(-10f, 1f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            BasePanel.Append(Scrollbar);

            var refreshButton = QuickButton(GetTexture("UI/Construct/Folder"));
            refreshButton.SetPos(new(-296f, 40f), 0.5f, 0f);
            refreshButton.OnMouseDown += (_, _) =>
            {
                FileOperator.CachedStructureDatas.Clear();
                SetupStructures();
            };
            Append(refreshButton);

            var folderButton = QuickButton(GetTexture("UI/Construct/Folder"));
            folderButton.SetPos(new(-246f, 40f), 0.5f, 0f);
            folderButton.OnMouseDown += (_, _) => TrUtils.OpenFolder(FileOperator.SavePath);
            Append(folderButton);

            var modeButton = QuickButton(GetTexture("UI/Construct/Folder"));
            modeButton.SetPos(new(-196f, 40f), 0.5f, 0f);
            modeButton.OnMouseDown += (_, _) =>
            {
                if (WandSystem.ConstructMode == WandSystem.Construct.Place)
                    WandSystem.ConstructMode = WandSystem.Construct.Save;
                else
                    WandSystem.ConstructMode = WandSystem.Construct.Place;
            };
            Append(modeButton);

            var closeButton = QuickButton(GetTexture("Close"));
            closeButton.SetPos(new(246f, 40f), 0.5f, 0f);
            closeButton.OnMouseDown += (_, _) => Close();
            Append(closeButton);
        }

        private ModImageButton QuickButton(Asset<Texture2D> texture)
        {
            var button = new ModImageButton(texture, Color.White, Color.White);
            button.SetBackgroundImage(ButtonPanel);
            button.SetHoverImage(ButtonPanel_Highlight);
            button.SetSize(44, 44);
            return button;
        }

        private void SetupScrollBar(bool resetViewPosition = true)
        {
            float height = UIList.GetInnerDimensions().Height;
            Scrollbar.SetView(height, UIList.GetTotalHeight());
            if (resetViewPosition)
                Scrollbar.ViewPosition = 0f;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (BasePanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
                Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var innerList = UIList.GetType().GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(UIList) as UIElement;
            if (Scrollbar is not null && innerList is not null)
            {
                innerList.Top.Set(-Scrollbar.GetValue(), 0);
            }
            UIList.Recalculate();

            base.DrawSelf(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (CacheSetupStructures)
            {
                SetupStructures();
                CacheSetupStructures = false;
            }

            Recalculate();

            base.Update(gameTime);

            if (BasePanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Structure GUI");
            }
        }

        public void SetupStructures()
        {
            UIList.Clear();

            var filePaths = Directory.GetFiles(FileOperator.SavePath);
            foreach (string path in filePaths)
            {
                if (Path.GetExtension(path) == FileOperator.Extension)
                {
                    UIList.Add(new StructurePanel(path));
                }
            }

            Recalculate();
            SetupScrollBar();
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open()
        {
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SetupStructures();
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close()
        {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
