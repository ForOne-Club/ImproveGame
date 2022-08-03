using ImproveGame.Common.ConstructCore;
using ImproveGame.Interface.UIElements;
using System.Reflection;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class StructureGUI : UIState
    {
        public static bool Visible { get; private set; }

        public UserInterface UserInterface;

        private ModUIPanel BasePanel; // 背景板
        public ModScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表

        public override void OnInitialize()
        {
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
            Scrollbar.Height.Set(-20f, 1f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            BasePanel.Append(Scrollbar);
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
                UIList.Add(new StructurePanel(path));
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
