using ImproveGame.UIFramework.SUIElements;
using System.Collections;
using Terraria.ModLoader.Config;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionList : OptionCollections
{
    private Type listType;

    protected override bool CanAdd => true;

    protected override void AddItem()
    {
        ((IList)Data).Add(CreateCollectionElementInstance(listType));
    }

    protected override void ClearCollection()
    {
        ((IList)Data).Clear();

    }

    protected override void InitializeCollection()
    {
        Data = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
        SetValueDirect(Data);
    }


    protected override void PrepareTypes()
    {
        listType = VariableInfo.Type.GetGenericArguments()[0];
        JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(VariableInfo.MemberInfo, listType);
    }

    protected override void SetupList()
    {
        //ListPanel.Clear();
        OptionView.ListView.RemoveAllChildren();
        IList list = VariableInfo.GetValue(Item) as IList;
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            var e = WrapIt(OptionView.ListView, Config, VariableInfo, Item, list, listType, i, this);
            var DeleteButton = new SUICross()
            {
                RelativeMode = RelativeMode.None,
                BgColor = Color.Black * 0.4f,
                Rounded = new Vector4(4f),
                Width = new(25, .0f),
                Height = new(25, .0f),
                Left = new(0, 0),
                Top = new(6, 0),
            };
            DeleteButton.OnLeftClick += (evt, elem) =>
            {
                ((IList)Data).RemoveAt(index);
                SetupList();
                pendingChanges = true;
            };
            DeleteButton.JoinParent(e);
            if (e.Elements[0] is OptionLabelElement label)
            {
                label.Left = new(30, 0);
            }
            int idx = i;
            e.OnLeftMouseDown += (evt, elem) =>
            {
                var dimension = elem.GetDimensions();
                if (evt.Target != elem) return;

                var c = 0;
                float h = 0;
                while (c < idx) 
                {
                    h += OptionView.ListView.Elements[c].Height.Pixels;
                    c++;
                }
                var pos = new Vector2(0,h);
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
                var dummy = list[idx];
                list.RemoveAt(idx);
                list.Insert(n, dummy);
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
