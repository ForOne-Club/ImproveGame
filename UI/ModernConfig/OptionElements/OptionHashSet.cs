using ImproveGame.UIFramework.SUIElements;
using System.Collections;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using ImproveGame.UIFramework.BaseViews;
using System.Collections.Generic;

namespace ImproveGame.UI.ModernConfig.OptionElements;

internal class OptionHashSet : OptionCollections
{
    private Type setType;

    public List<ISetElementWrapper> DataWrapperList { get; set; }

    protected override bool CanAdd => true;

    MethodInfo addMethod;
    MethodInfo clearMethod;
    MethodInfo removeMethod;
    PropertyFieldWrapper wrappermemberInfo;
    protected override void AddItem()
    {
        addMethod ??= Data.GetType().GetMethods().FirstOrDefault(m => m.Name == "Add");
        addMethod?.Invoke(Data, [CreateCollectionElementInstance(setType)]);
    }

    protected override void ClearCollection()
    {
        clearMethod ??= Data.GetType().GetMethods().FirstOrDefault(m => m.Name == "Clear");
        clearMethod?.Invoke(Data, []);
    }

    protected override void InitializeCollection()
    {
        Data = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(setType));
        SetValueDirect(Data);
    }


    protected override void PrepareTypes()
    {
        setType = VariableInfo.Type.GetGenericArguments()[0];
        JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(VariableInfo.MemberInfo, setType);
    }

    protected override void SetupList()
    {
        //ListPanel.Clear();
        OptionView.ListView.RemoveAllChildren();
        var genericType = typeof(SetElementWrapper<>).MakeGenericType(setType);

        DataWrapperList = new List<ISetElementWrapper>();

        if (Data != null)
        {
            var valuesEnumerator = ((IEnumerable)Data).GetEnumerator();
            int i = 0;

            while (valuesEnumerator.MoveNext())
            {
                ISetElementWrapper proxy = (ISetElementWrapper)Activator.CreateInstance(genericType, [valuesEnumerator.Current, Data]);
                DataWrapperList.Add(proxy);
                wrappermemberInfo ??= ConfigManager.GetFieldsAndProperties(this).ToList().First(x => x.Name == "DataWrapperList");
                var e = WrapIt(OptionView.ListView, Config, wrappermemberInfo, Item, DataWrapperList, genericType, i, this);


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
                object o = valuesEnumerator.Current; // needed for closure?

                DeleteButton.OnLeftClick += (evt, elem) =>
                {
                    removeMethod ??= Data.GetType().GetMethods().FirstOrDefault(m => m.Name == "Remove");
                    removeMethod.Invoke(Data, [o]);
                    SetupList();
                    pendingChanges = true;
                };
                DeleteButton.JoinParent(e);
                if (e.Elements[0] is OptionLabelElement label)
                {
                    label.Left = new(30, 0);
                }
                i++;
            }
        }

        OptionView.Recalculate();
        Height.Set(Math.Min((OptionView.ListView.Children.Any() ? OptionView.ListView.Height.Pixels : 0) + 70, 360), 0);//不知道为什么MaxHeight不管用了
        //TODO 排版上改成网格式而不是纵向列表

    }
}
