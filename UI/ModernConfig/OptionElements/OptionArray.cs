using ImproveGame.UIFramework.BaseViews;
namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionArray : OptionCollections
{
    private Type itemType;

    protected override bool CanItemBeAdded => false;

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
                var pos = new Vector2(0, h - OptionView.ScrollBar.TargetScrollPosition.Y);// 
                offset = evt.MousePosition - pos - GetDimensions().Position();

                float deltaY = evt.MousePosition.Y - OptionView.GetDimensions().Y - h;
                if (deltaY < elem.Height.Pixels * .5f)
                {
                    e.RelativeMode = RelativeMode.None;
                    e.Remove();
                    e.JoinParent(OptionView.MaskView);
                    OptionView.Recalculate();
                    currentDraggingOption = e;
                }

            };
            e.OnUpdate += (elem) =>
            {
                if (currentDraggingOption == null || currentDraggingOption != elem)
                    return;
                var pos = Main.MouseScreen - GetDimensions().Position() - offset;// + new Vector2(0, OptionView.ScrollBar.TargetScrollPosition.Y);
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

                NetSyncManually();
            };
            //e.OnRightMouseDown += (evt, elem) => e.SetValueDirect(CreateCollectionElementInstance(itemType));
        }
        OptionView.Recalculate();
        Height.Set(Math.Min((OptionView.ListView.Children.Any() ? OptionView.ListView.Height.Pixels : 0) + 70, 360), 0);//不知道为什么MaxHeight不管用了
        //TODO 排版上改成网格式而不是纵向列表

    }

    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        base.DrawChildren(spriteBatch);
        if (currentDraggingOption != null)
        {
            float y = Main.mouseY;
            var en = OptionView.ListView.Children.GetEnumerator();

            CalculatedStyle curDimension = default;
            bool last = true;
            while (en.MoveNext())
            {
                var cur = en.Current;
                curDimension = cur.GetDimensions();
                if (y < curDimension.Center().Y)
                {
                    y = curDimension.Position().Y;
                    last = false;
                    break;
                }
            }
            if (last)
                y = curDimension.Position().Y + curDimension.Height;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(GetDimensions().X - 10, y), new Rectangle(0, 0, 1, 1), Color.White, 0, new Vector2(.5f), new Vector2(240, 2), 0, 0);
            goto label;
        }
        foreach (var elem in OptionView.ListView.Elements)
        {
            var dimemsion = elem.GetDimensions();
            if (elem.IsMouseHovering && Main.mouseY < dimemsion.Y + dimemsion.Height * .5f)
            {
                goto label;
            }
        }
        return;
        label:
        if (!Main.instance._mouseTextCache.isValid)
            Main.instance.MouseText("[i:897]", 0, 0, Main.mouseX + 16, Main.mouseY - 10);

    }
    ModernConfigOption currentDraggingOption;
    Vector2 offset;
}
