using ImproveGame.Interface.UIElements;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.ItemDropRules;

namespace ImproveGame.Interface.GUI
{
    public class GrabBagInfoGUI : UIState
    {
        public static bool Visible { get; private set; }
        private static float panelLeft;
        private static float panelWidth;
        private static float panelTop;
        private static float panelHeight;
        public static int ItemID { get; private set; }
        public UserInterface UserInterface;

        private ModUIPanel BasePanel;
        public ModScrollbar Scrollbar;
        public UIList UIList;
        public ModItemSlot ItemSlot;

        public override void OnInitialize()
        {
            panelLeft = 730f;
            panelTop = 160f;
            panelHeight = 260f;
            panelWidth = 310f;

            BasePanel = new ModUIPanel(resizeable: true, minResizeWidth: 220, minResizeHeight: 140);
            BasePanel.Left.Set(panelLeft, 0f);
            BasePanel.Top.Set(panelTop, 0f);
            BasePanel.Width.Set(panelWidth, 0f);
            BasePanel.Height.Set(panelHeight, 0f);
            BasePanel.BorderColor = new(29, 34, 70);
            BasePanel.OnResize += ResetUI; // 重设各部件
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
            Scrollbar.Left.Set(-6f, 0f);
            Scrollbar.Height.Set(-20f, 1f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            BasePanel.Append(Scrollbar);

            ItemSlot = new(1f);
            ItemSlot.SetSize(new(60f, 60f)).SetPos(-72f, -12f);
            BasePanel.Append(ItemSlot);
        }

        private void ResetUI(UIElement affectedElement)
        {
            panelLeft = affectedElement.Left.Pixels;
            panelTop = affectedElement.Top.Pixels;
            panelWidth = affectedElement.Width.Pixels;
            panelHeight = affectedElement.Height.Pixels;
            foreach (var uie in UIList) {
                uie.Width.Pixels = panelWidth - 60f;
            }
            Recalculate();
            SetupScrollBar(false);
        }

        private void SetupScrollBar(bool resetViewPosition = true) {
            float height = UIList.GetInnerDimensions().Height;
            Scrollbar.SetView(height, UIList.GetTotalHeight());
            if (resetViewPosition)
                Scrollbar.ViewPosition = 0f;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            if (Scrollbar.IsMouseHovering)
            {
                BasePanel.Dragging = false;
            }
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

        public void SetupDropRateInfoList()
        {
            UIList.Clear();

            var itemDropRules = Main.ItemDropsDB.GetRulesForItemID(ItemID, includeGlobalDrops: false);
            var list = new List<DropRateInfo>();
            var ratesInfo = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule item in itemDropRules)
            {
                item.ReportDroprates(list, ratesInfo);
            }

            foreach (DropRateInfo item2 in list)
            {
                UIList.Add(new GrabBagInfoPanel(item2, panelWidth - 60f));
            }
            Recalculate();
            SetupScrollBar();
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open(int itemID)
        {
            BasePanel.Dragging = false;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            ItemID = itemID;

            ItemSlot.Item = new(ItemID);
            SetupDropRateInfoList();
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
