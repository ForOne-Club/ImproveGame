/*
namespace ImproveGame.UIFramework.SUIElements;
{
    public class SUIScrollViewDraggingSortable : SUIScrollView2
    {
        public SUIScrollViewDraggingSortable(Orientation scrollOrientation, bool fixedSize = true) : base(scrollOrientation, fixedSize)
        {

        }
        //初次添加的控件请使用这个函数，这样才有拖拽排序效果
        public void AddToList(UIElement element) 
        {
            ListView.Append(element);
            element.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem)
                    return;
                elem.Remove();
                elem.Append(MaskView);
                dragging = true;
                currentDraggingElement = elem;
                startPos = evt.MousePosition;
            };
            element.OnUpdate += (elem) =>
            {
                if (!dragging || elem != currentDraggingElement)
                    return;
                elem.SetPos(Main.MouseScreen - startPos + oldPos);
            };
            element.OnLeftMouseUp += (evt, elem) =>
            {
                if (evt.Target != elem)
                    return;
                currentDraggingElement = null;
                dragging = false;
                int length = ListView.Children.Count();
                int n = 0;
                bool vertical = ScrollOrientation == Orientation.Vertical;
                var y = vertical ? elem.Top.Pixels : elem.Left.Pixels;
                var en = ListView.Children.GetEnumerator();
                while (en.MoveNext()) 
                {
                    var cur = en.Current;
                    if (y < (vertical ? (cur.Top.Pixels + cur.Height.Pixels * .5f) : (cur.Left.Pixels + cur.Width.Pixels * .5f)))
                        break;
                    n++;
                }
                ListView.Elements.Insert(n, elem);
            };
        }
        bool dragging;
        UIElement currentDraggingElement;
        Vector2 startPos;
        Vector2 oldPos;
    }
}
*/