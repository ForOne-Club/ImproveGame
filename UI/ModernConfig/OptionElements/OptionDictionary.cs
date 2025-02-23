using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using ImproveGame.UIFramework.SUIElements;
using Newtonsoft.Json;
using Terraria.ModLoader.UI;
using System.Collections;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    internal class OptionDictionary : OptionCollections
    {
        internal Type keyType;
        internal Type valueType;
        internal UIText save;
        public List<IDictionaryElementWrapper> dataWrapperList;

        // These 2 hold the default value of the dictionary value, hence ValueValue
        protected DefaultDictionaryKeyValueAttribute defaultDictionaryKeyValueAttribute;
        protected JsonDefaultDictionaryKeyValueAttribute jsonDefaultDictionaryKeyValueAttribute;
        protected override bool CanItemBeAdded => true;
        PropertyFieldWrapper wrappermemberInfo;

        protected override void AddItem()
        {
            object keyValue;

            if (defaultDictionaryKeyValueAttribute != null)
            {
                keyValue = defaultDictionaryKeyValueAttribute.Value;
            }
            else
            {
                keyValue = ConfigManager.AlternateCreateInstance(keyType);

                if (!keyType.IsValueType && keyType != typeof(string))
                {
                    string json = jsonDefaultDictionaryKeyValueAttribute?.Json ?? "{}";

                    JsonConvert.PopulateObject(json, keyValue, ConfigManager.serializerSettings);
                }
            }
            var dict = (IDictionary)Data;
            if (!dict.Contains(keyValue))
                dict.Add(keyValue, CreateCollectionElementInstance(valueType));
        }

        protected override void ClearCollection()
        {
            ((IDictionary)Data).Clear();


        }

        protected override void InitializeCollection()
        {
            Data = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));

            SetValueDirect(Data);
        }


        protected override void PrepareTypes()
        {
            keyType = VariableInfo.Type.GetGenericArguments()[0];
            valueType = VariableInfo.Type.GetGenericArguments()[1];
            JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(VariableInfo.MemberInfo, valueType);
            defaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<DefaultDictionaryKeyValueAttribute>(VariableInfo, null, null);
            jsonDefaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<JsonDefaultDictionaryKeyValueAttribute>(VariableInfo, null, null);
        }

        protected override void SetupList()
        {
            //ListPanel.Clear();
            OptionView.ListView.RemoveAllChildren();
            dataWrapperList = new List<IDictionaryElementWrapper>();
            Type genericType = typeof(DictionaryElementWrapper<,>).MakeGenericType(keyType, valueType);

            if (Data != null)
            {
                var keys = ((IDictionary)Data).Keys;
                var values = ((IDictionary)Data).Values;
                var keysEnumerator = keys.GetEnumerator();
                var valuesEnumerator = values.GetEnumerator();
                int i = 0;

                while (keysEnumerator.MoveNext())
                {
                    valuesEnumerator.MoveNext();
                    var proxy = (IDictionaryElementWrapper)Activator.CreateInstance(genericType, [keysEnumerator.Current, valuesEnumerator.Current, (IDictionary)Data]);

                    dataWrapperList.Add(proxy);
                    Type itemType = VariableInfo.Type.GetGenericArguments()[0];
                    wrappermemberInfo ??= ConfigManager.GetFieldsAndProperties(this).ToList()[0];

                    // 设置了RelativeMode就不能设置Top.Pixels了，因此这里用元素套Box实现和上边界留有间隔
                    var helperBox = new View
                    {
                        RelativeMode = RelativeMode.Horizontal,
                        IsAdaptiveWidth = true,
                        HAlign = 0f,
                        Height = new(36, .0f),
                        PaddingTop = 8,
                    };

                    var deleteButton = new SUICross()
                    {
                        Spacing = new Vector2(4f, 10f),
                        BgColor = Color.Black * 0.4f,
                        Rounded = new Vector4(4f),
                        Width = new(25, .0f),
                        Height = new(25, .0f),
                    };
                    object o = keysEnumerator.Current;
                    deleteButton.OnLeftClick += (evt, elem) =>
                    {
                        ((IDictionary)Data).Remove(o);
                        SetupList();
                        pendingChanges = true;
                    };
                    deleteButton.JoinParent(helperBox);

                    var e = WrapIt(OptionView.ListView, Config, wrappermemberInfo, Item, dataWrapperList, genericType, i, this, preLabelAppend: helperBox.JoinParent);

                    if (e.Elements[0] is OptionLabelElement label)
                    {
                        label.Left = new(30, 0);
                    }

                    i++;
                }
            }
        }
    }
}
