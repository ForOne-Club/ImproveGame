using System.Collections.Generic;

using ImproveGame.UIFramework.BaseViews;
namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionArray : OptionCollections
{
    private Type itemType;

    protected override bool CanAdd => false;

    protected override void AddItem()
    {
        throw new NotImplementedException();
    }

    protected override void ClearCollection()
    {
        throw new NotImplementedException();
    }

    protected override void InitializeCollection()
    {
        throw new NotImplementedException();
    }

    protected override void NullCollection()
    {
        throw new NotImplementedException();
    }

    protected override void PrepareTypes()
    {
        itemType = VariableInfo.Type.GetElementType();
    }

    protected override void SetupList()
    {
        //ListPanel.Clear();
        OptionView.ListView.RemoveAllChildren();
        Array array = VariableInfo.GetValue(Item) as Array;
        int count = array.Length;
        for (int i = 0; i < count; i++)
        {
            var e = WrapIt(OptionView.ListView, Config, VariableInfo, Item, array, itemType, i, this);
            int idx = i;
            e.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem) return;
                var c = 0;
                float h = 0;
                while (c < idx)
                {
                    h += OptionView.ListView.Elements[c].Height.Pixels;
                    c++;
                }
                var pos = new Vector2(0, h - OptionView.ScrollBar.TargetScrollPosition.Y);
                offset = evt.MousePosition - pos;


                float deltaY = evt.MousePosition.Y - OptionView.GetDimensions().Y - h;
                if (deltaY < elem.Height.Pixels * .5f) 
                {
                    e.RelativeMode = RelativeMode.None;
                    e.Remove();
                    e.JoinParent(OptionView.MaskView);
                    currentDraggingOption = e;
                }

            };
            e.OnUpdate += (elem) =>
            {
                if (currentDraggingOption == null || currentDraggingOption != elem)
                    return;
                var pos = Main.MouseScreen - offset;
                e.SetPosPixels(pos);
                e.Recalculate();
            };
            e.OnLeftMouseUp += (evt, elem) =>
            {

                if (currentDraggingOption != elem) return;
                currentDraggingOption = null;
                int length = OptionView.ListView.Children.Count();
                var selfDimension = elem.GetDimensions();
                e.Remove();
                int n = 0;
                var y = selfDimension.Y;
                var en = OptionView.ListView.Children.GetEnumerator();
                while (en.MoveNext())
                {
                    var cur = en.Current;
                    var curDimension = cur.GetDimensions();
                    if (y < curDimension.Center().Y)
                        break;
                    n++;
                }
                var dummy = array.GetValue(idx);
                if (idx > n)
                    for (int m = idx - 1; m >= n; m--)
                        array.SetValue(array.GetValue(m), m + 1);
                else if (idx < n)
                    for (int m = idx + 1; m <= n; m++)
                        array.SetValue(array.GetValue(m), m - 1);
                array.SetValue(dummy, n);
                SetupList();
                pendingChanges = true;
            };
        }
        OptionView.Recalculate();
        Height.Set(Math.Min((OptionView.ListView.Children.Any() ? OptionView.ListView.Height.Pixels : 0) + 70, 360), 0);//不知道为什么MaxHeight不管用了
        //TODO 排版上改成网格式而不是纵向列表

    }
    ModernConfigOption currentDraggingOption;
    Vector2 offset;
}
