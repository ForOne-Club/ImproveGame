using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UI.ModernConfig.Categories;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionObject : ModernConfigOption
    {
        // 使用自动排列从右向左排，加个透明Box来控制位置，子元素从左到右全部加到这个Box里，就能从右向左排了
        public View HelperBox;

        //SeparatePage:有分页面，内外都为True
        //innerPage:处于分页面，内部为True
        //在外页面保留初始化按钮
        //在内部取消折叠按钮
        protected override void OnBind()
        {
            HelperBox = new View
            {
                IsAdaptiveWidth = true,
                HAlign = 1f,
                VAlign = 0.5f,
                Height = StyleDimension.Fill,
                PaddingTop = 8,
            };
            HelperBox.JoinParent(this);

            InitialButton = new SUITriangleIcon()
            {
                RelativeMode = RelativeMode.Horizontal,
                Spacing = new Vector2(4f, 0f),
                BgColor = Color.Black * 0.4f,
                Rounded = new Vector4(4f),
                Width = new(25, 0.0f),
                Height = new(25, 0.0f),
            };
            InitialButton.OnLeftClick += (evt, elem) =>
            {
                SoundEngine.PlaySound(SoundID.Tink);
                object data = Activator.CreateInstance(VarType, true);
                string json = JsonDefaultValueAttribute?.Json ?? "{}";
                JsonConvert.PopulateObject(json, data, ConfigManager.serializerSettings);
                SetValueDirect(data);
                pendingChanges = true;
                SetupList();
            };
            DeleteButton = new SUICross()
            {
                RelativeMode = RelativeMode.Horizontal,
                Spacing = new Vector2(4f, 0f),
                BgColor = Color.Black * 0.4f,
                Rounded = new Vector4(4f),
                Width = new(25, 0.0f),
                Height = new(25, 0.0f),
            };
            DeleteButton.OnLeftClick += (evt, elem) =>
            {
                SetValueDirect(null);
                pendingChanges = true;
            };
            ExpandButton = new SUITriangleIcon()
            {
                RelativeMode = RelativeMode.Horizontal,
                Spacing = new Vector2(4f, 0f),
                BgColor = Color.Black * 0.4f,
                Rounded = new Vector4(4f),
                Width = new(25, 0.0f),
                Height = new(25, 0.0f)
            };
            //ExpandButton.trianglePercentCoord[1] = new(0f, .5f);
            ExpandButton.OnLeftClick += (evt, elem) =>
            {
                expanded = !expanded;
                pendingChanges = true;
            };
            //if (HasCustom)
            //{
            //    var customButton = new SUIDrawingImage(v =>
            //    {
            //        var dimension = v.GetDimensions();
            //        SDFGraphics.NoBorderStarX(dimension.Center(), new(.5f), dimension.Width * .5f, 4, .5f, Color.Yellow, GetMatrix(true));
            //        //SDFRectangle.HasBorder(dimension.Position(),dimension.Size(),v.Rounded,v.BgColor,v.);
            //    }
            //    )
            //    {
            //        RelativeMode = RelativeMode.None,
            //        BgColor = Color.Black * 0.4f,
            //        Rounded = new Vector4(4f),
            //        Left = new(-120, 1),
            //        Top = new(6, 0),
            //        Width = new(25, .0f),
            //        Height = new(25, .0f)
            //    };
            //    customButton.JoinParent(this);
            //    customButton.OnLeftClick += (evt, elem) =>
            //    {
            //        SoundEngine.PlaySound(SoundID.MenuTick);
            //        _modInMemory = ModernConfigUI.Instance.currentMod;
            //        _categoryInMemory = ConfigOptionsPanel.CurrentCategory;
            //        Config.Open(() =>
            //        {
            //            ModernConfigUI.Instance.Open(_modInMemory);
            //            ConfigOptionsPanel.CurrentCategory = _categoryInMemory;
            //        }, OptionName);
            //    };
            //    customButton.OnMouseOver += (evt, elem) =>
            //    {
            //        string info = GetText("ModernConfig.CustomItemTip");
            //        ModernConfigUI.PopNewInfo(info, elem.GetDimensions().Position() - FontAssets.MouseText.Value.MeasureString(info), Color.Cyan);
            //    };
            //}
            OptionView = new SUIScrollView2(Orientation.Vertical)
            {
                RelativeMode = RelativeMode.None,
                Top = new(46, 0),
                Spacing = new Vector2(6)
            };
            OptionView.SetPadding(0f, 0f);
            OptionView.SetSize(0f, -60, 1f, 1f);
            object data = GetValue();
            if (data == null)
                InitialButton.JoinParent(this);
            else
            {
                if (NullAllowedAttribute != null)
                    DeleteButton.JoinParent(this);
                SetupList();
            }

            pendingChanges = true;
        }
        protected virtual void ObjectToOption(object data)
        {
            foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(data))
            {
                if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
                    continue;
                var e = WrapIt(OptionView.ListView, Config, variable, data, owner: this);
                if (e is OptionSlider slider)
                {
                    if (RangeAttribute != null)
                    {
                        slider.Max = Convert.ToDouble(RangeAttribute.Max);
                        slider.Min = Convert.ToDouble(RangeAttribute.Min);
                    }
                    if (IncrementAttribute != null)
                    {
                        slider.Increment = Convert.ToDouble(IncrementAttribute.Increment);
                    }
                }
                OnWrapOption(e);
            }
        }
        void SetupList()
        {
            OptionView.ListView.RemoveAllChildren();
            ObjectToOption(GetValue());
            OptionView.Recalculate();
            Height.Set(Math.Min((OptionView.ListView.Children.Any() ? OptionView.ListView.Height.Pixels : 0) + 70, 360), 0);//不知道为什么MaxHeight不管用了
            pendingChanges = true;
        }
        protected virtual void OnWrapOption(ModernConfigOption option)
        {

        }
        public override string Label => base.Label + (ShowStringValueInLabel ? $":{GetValue()?.ToString() ?? "null"}" : "");
        protected SUIScrollView2 OptionView;
        SUITriangleIcon ExpandButton;
        SUITriangleIcon InitialButton;
        SUICross DeleteButton;
        protected bool expanded = true;
        protected bool pendingChanges;
        protected JsonDefaultValueAttribute JsonDefaultValueAttribute;
        NullAllowedAttribute NullAllowedAttribute;
        SeparatePageAttribute SeparatePageAttribute;
        protected RangeAttribute RangeAttribute;
        protected IncrementAttribute IncrementAttribute;
        protected bool ShowStringValueInLabel;
        bool SeparatePage => SeparatePageAttribute != null;
        bool innerPage;//旧版实现子页面的手段，现在用不到了？
        bool HasCustom;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SeparatePage && !innerPage)
            {
                if (!pendingChanges)
                    return;
                InitialButton.Remove();
                if (GetValue() == null)
                    InitialButton.JoinParent(this);
                return;
            }
            #region Expand的更新
            if (ExpandButton != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 targetCoord = i switch
                    {
                        0 => expanded ? new(0.0f, 0.3f) : new(0.7f, 0.0f),
                        1 => expanded ? new(0.5f, 0.8f) : new(0.2f, 0.5f),
                        2 or _ => expanded ? new(1.0f, 0.3f) : new(0.7f, 1.0f)
                    };
                    ExpandButton.trianglePercentCoord[i] = Vector2.Lerp(ExpandButton.trianglePercentCoord[i], targetCoord, .15f + i * .05f);

                }
                //var targetCoord = expanded ? new Vector2(1, .5f) : new(.5f, 0f);
                //ExpandButton.trianglePercentCoord[0] = Vector2.Lerp(ExpandButton.trianglePercentCoord[0],targetCoord,.25f);
            }
            #endregion
            #region 高度上限调整
            if (dragging && !innerPage)
            {
                showMaxHeight = Main.MouseScreen.Y - mousePos.Y + oldHeight;
                showMaxHeight = Math.Max(150, showMaxHeight); // 别拖没了
                pendingChanges = true;
            }
            #endregion
            if (!pendingChanges)
                return;
            pendingChanges = false;
            InitialButton.Remove();
            ExpandButton.Remove();
            DeleteButton.Remove();
            OptionView.Remove();

            if (GetValue() == null)
                InitialButton.JoinParent(HelperBox);
            else
            {
                if (NullAllowedAttribute != null)
                    DeleteButton.JoinParent(HelperBox);
                if (!innerPage)
                    ExpandButton.JoinParent(HelperBox);
                if (expanded)
                    OptionView.JoinParent(this);
            }
            //Parent?.Recalculate();
            UIElement target = this;
            ModernConfigOption optionObject = this;
            while (target.Parent != null)
            {
                target = target.Parent;
                if (target is OptionObject optionO)
                    optionObject = optionO;
                if (target is OptionCollections optionC)
                    optionObject = optionC;
                if (target is OptionDefinition optionD)
                    optionObject = optionD;
            }
            optionObject.Recalculate();
            optionObject.Parent?.Recalculate();
        }
        public override void Recalculate()
        {
            if (!expanded || GetValue() == null || (SeparatePage && !innerPage))
            {
                Height.Set(46, 0);
                base.Recalculate();
                return;
            }
            float h = OptionView.ListView.GetDimensions().Height + 70;

            h = Utils.Clamp(h, 70, innerPage ? 214514 : showMaxHeight);
            if (h < showMaxHeight)
                showMaxHeight = Math.Max(h, 150);

            Height.Set(h, 0f);
            Parent?.Height.Set(h, 0f);
            base.Recalculate();
        }
        protected float showMaxHeight = 360;
        float oldHeight;
        Vector2 mousePos;
        bool dragging;
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //ModernConfigUI.PopNewInfo("我去是新信息耶耶耶(", evt.MousePosition - ModernConfigUI.Instance.MainPanel.GetDimensions().Position(),Color.Cyan);
            if (SeparatePage) return;
            var dimension = GetDimensions();
            if (evt.Target == this && evt.MousePosition.Y > dimension.Y + .9f * dimension.Height)
            {
                mousePos = evt.MousePosition;
                oldHeight = showMaxHeight;
                dragging = true;
            }
            base.LeftMouseDown(evt);
        }
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (SeparatePage) return;
            if (dragging)
            {
                dragging = false;
            }
            base.LeftMouseUp(evt);
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            if (SeparatePage && evt.Target == this)
            {
                object data = GetValue();
                if (data != null)
                {
                    List<KeyValuePair<PropertyFieldWrapper, ModConfig>> list = [];
                    //List<KeyValuePair<PropertyFieldWrapper, ModConfig>> list = [new(VariableInfo, Config)];
                    foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(data))
                    {
                        if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
                            continue;
                        list.Add(new(variable, Config));
                        //WrapIt(OptionView.ListView, Config, variable, data, owner: this);
                    }
                    CrossModCategoryCard card = new CrossModCategoryCard(list,
                    getLabel: () => List == null ? base.Label : $"{owner.Label}#{index + 1}",
                    getTooltip: () => List == null ? Tooltip : owner.Tooltip);
                    List<string> pth = [];
                    if (path != null)
                        pth.AddRange(path);
                    pth.Add(VariableInfo.Name);
                    ConfigOptionsPanel.SwitchToSubPage(card, data, pth);
                    //ConfigOptionsPanel.GlobalItem = Item;
                    //CrossModCategoryCard card = new CrossModCategoryCard(list);
                    //ConfigOptionsPanel.CurrentCategory = card;
                    //(ConfigOptionsPanel.Instance.AllOptions[0] as OptionObject).innerPage = true;
                    //ConfigOptionsPanel.GlobalItem = null;
                }

            }
            base.LeftClick(evt);
        }
        protected override void CheckAttributes()
        {
            var expandedAttribute = GetAttribute<ExpandAttribute>();
            if (expandedAttribute != null)
                expanded = expandedAttribute.Expand;
            JsonDefaultValueAttribute = GetAttribute<JsonDefaultValueAttribute>();
            NullAllowedAttribute = GetAttribute<NullAllowedAttribute>();
            SeparatePageAttribute = GetAttribute<SeparatePageAttribute>();
            RangeAttribute = GetAttribute<RangeAttribute>();
            IncrementAttribute = GetAttribute<IncrementAttribute>();
            HasCustom = GetAttribute<CustomModConfigItemAttribute>() != null;
            ShowStringValueInLabel = VarType.GetMethod("ToString", []).DeclaringType != typeof(object);
            base.CheckAttributes();
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dimemsion = GetDimensions();
            if ((Main.mouseY > dimemsion.Y + dimemsion.Height * .9f && IsMouseHovering) || dragging)
            {
                Main.instance.MouseText("[i:536]", 0, 0, Main.mouseX + 16, Main.mouseY - 10);
            }
        }
        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);
            pendingChanges = true;
        }
        protected override void OnSetDefault(object value)
        {
            if (value != null)
                SetupList();
            pendingChanges = true;
            base.OnSetDefault(value);
        }
    }
}
