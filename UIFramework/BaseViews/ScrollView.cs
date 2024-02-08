using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UIFramework.BaseViews
{
    // 通用滚动视图
    public class ScrollView : View
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly SUIScrollBar Scrollbar;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly TimerView ListView = new TimerView();

        public ScrollView()
        {
            OverflowHidden = true;
            DragIgnore = true;

            ListView.IsAdaptiveHeight = true;
            ListView.JoinParent(this);

            Scrollbar = new SUIScrollBar();
            Scrollbar.JoinParent(this);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            Scrollbar.BarTopBuffer += evt.ScrollWheelValue;
        }

        public static Vector2 GridSize(Vector2 size, Vector2 spacing, int h, int v)
        {
            return (size + spacing) * new Vector2(h, v) - spacing;
        }

        public static Vector2 GridSize(float size, float spacing, int h, int v)
        {
            return GridSize(new Vector2(size), new Vector2(spacing), h, v);
        }

        public static float GridSize(float size, float spacing, int hv)
        {
            return (size + spacing) * hv - spacing;
        }
    }
}