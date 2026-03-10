using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using MonoMod.Cil;
using System.Collections;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionDefinition : ModernConfigOption
{
    Type EntityDefinitionElementType;//原版DefinitionElement的Type，用来获取它的函数进而套壳
    Type EntityDefinitionElementOptionType;//原版DefinitionElementOption的Type，也就是点开来之后列表里的一个个子选项，同样用作套壳
    ConfigElement ElementInstance;
    UIElement OptionChoice;
    MethodInfo createDefinitionMethod;
    MethodInfo tweakDefinitionMethod;
    MethodInfo getDefinitionListMethod;
    MethodInfo getPassedDefinitionListMethod;
    PropertyInfo optionProperty;
    PropertyInfo filterModProperty;
    PropertyInfo filterNameProperty;
    FieldInfo leftClickEvtInfo;

    PropertyInfo tooltipProperty;
    MethodInfo setItemMethod;

    float showMaxHeight;
    bool SelectionExpanded;
    bool UpdateNeeded;
    SUIImage modIcon;
    SUIEditableText filteringName;
    SUIScrollView2 OptionView;
    View MainOptionPanel;
    List<Texture2D> modIconTextures = [];
    List<string> modNames = [];
    int currentModIndex = 0;
    IList Options
    {
        get => optionProperty.GetValue(ElementInstance) as IList;
        set => optionProperty.SetValue(ElementInstance, value);
    }
    UIFocusInputTextField FilterMod => filterModProperty.GetValue(ElementInstance) as UIFocusInputTextField;
    UIFocusInputTextField FilterName => filterNameProperty.GetValue(ElementInstance) as UIFocusInputTextField;

    protected override void CheckAttributes()
    {
        if (!VarType.IsSubclassOf(typeof(EntityDefinition)))
            throw new Exception($"Field \"{OptionName}\" is not a Definition");
        var customItem = GetAttribute<CustomModConfigItemAttribute>();
        if (customItem != null)
            EntityDefinitionElementType = customItem.Type;
        else
        {
            Type type = VarType;
            if (type == typeof(ItemDefinition))
            {
                EntityDefinitionElementType = typeof(ItemDefinitionElement);
                EntityDefinitionElementOptionType = typeof(ItemDefinitionOptionElement);
            }
            else if (type == typeof(ProjectileDefinition))
            {
                EntityDefinitionElementType = typeof(ProjectileDefinitionElement);
                EntityDefinitionElementOptionType = typeof(ProjectileDefinitionOptionElement);
            }
            else if (type == typeof(NPCDefinition))
            {
                EntityDefinitionElementType = typeof(NPCDefinitionElement);
                EntityDefinitionElementOptionType = typeof(NPCDefinitionOptionElement);
            }
            else if (type == typeof(PrefixDefinition))
            {
                EntityDefinitionElementType = typeof(PrefixDefinitionElement);
                EntityDefinitionElementOptionType = typeof(PrefixDefinitionOptionElement);
            }
            else if (type == typeof(BuffDefinition))
            {
                EntityDefinitionElementType = typeof(BuffDefinitionElement);
                EntityDefinitionElementOptionType = typeof(BuffDefinitionOptionElement);
            }
            else if (type == typeof(TileDefinition))
            {
                EntityDefinitionElementType = typeof(TileDefinitionElement);
                EntityDefinitionElementOptionType = typeof(TileDefinitionOptionElement);
            }
        }
        var methods = EntityDefinitionElementType.GetMethods(BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        byte bitsByte = 0;
        foreach (var method in methods)
        {
            switch (method.Name)
            {
                case "CreateDefinitionOptionElement":
                    createDefinitionMethod = method;
                    bitsByte += 1;
                    break;
                case "GetPassedOptionElements":
                    getPassedDefinitionListMethod = method;
                    bitsByte += 2;
                    break;
                case "CreateDefinitionOptionElementList":
                    getDefinitionListMethod = method;
                    bitsByte += 4;
                    break;
                case "TweakDefinitionOptionElement":
                    tweakDefinitionMethod = method;
                    bitsByte += 8;
                    break;
            }
            if (bitsByte == 15)
                break;
        }
        if (customItem != null)
            EntityDefinitionElementOptionType = createDefinitionMethod.ReturnType;
        optionProperty = EntityDefinitionElementType.GetProperty("Options", BindingFlags.Instance | BindingFlags.NonPublic);
        filterNameProperty = EntityDefinitionElementType.GetProperty("ChooserFilter", BindingFlags.Instance | BindingFlags.NonPublic);
        filterModProperty = EntityDefinitionElementType.GetProperty("ChooserFilterMod", BindingFlags.Instance | BindingFlags.NonPublic);
        base.CheckAttributes();
    }

    protected override void OnBind()
    {
        leftClickEvtInfo = typeof(UIElement).GetField("OnLeftClick", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        ElementInstance = Activator.CreateInstance(EntityDefinitionElementType) as ConfigElement;
        ElementInstance.Bind(VariableInfo, Item, List, index);
        ElementInstance.OnBind();

        OptionChoice = createDefinitionMethod.Invoke(ElementInstance, []) as UIElement;
        OptionChoice.Top.Set(2f, 0f);
        OptionChoice.Left.Set(-30, 1f);
        OptionChoice.OnLeftClick += (a, b) =>
        {
            SelectionExpanded = !SelectionExpanded;
            UpdateNeeded = true;
        };

        tweakDefinitionMethod.Invoke(ElementInstance, [OptionChoice]);
        Append(OptionChoice);

        setItemMethod = OptionChoice.GetType().GetMethod("SetItem", BindingFlags.Instance | BindingFlags.Public);
        tooltipProperty = OptionChoice.GetType().GetProperty("Tooltip", BindingFlags.Instance | BindingFlags.Public);

        OptionChoice.Top.Set(OptionChoice.Top.Pixels + 2, 0f);

        MainOptionPanel = new View()
        {
            RelativeMode = RelativeMode.Vertical,
            Width = new(0, 1.0f),
            Height = new(-60, 1.0f),

        };


        modNames.Add("");
        modIconTextures.Add(Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad).Value);
        foreach (var m in ModLoader.Mods)
        {
            var file = m.File;
            if (file != null && file.HasFile("icon.png"))
            {
                try
                {
                    using (file.Open())
                    using (var s = file.GetStream("icon.png"))
                    {
                        var iconTexture = Main.Assets.CreateUntracked<Texture2D>(s, ".png");

                        if (iconTexture.Width() == 80 && iconTexture.Height() == 80)
                        {
                            modIconTextures.Add(iconTexture.Value);
                            modNames.Add(m.DisplayNameClean);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.tML.Error("Unknown error", e);
                }
            }
        }
        modIcon = new SUIImage(modIconTextures[0])
        {
            Width = new(80, 0),
            Height = new(80, 0),
            RelativeMode = RelativeMode.Horizontal
        };
        modIcon.ImageScale = 80f / modIcon.Texture.Width;
        modIcon.JoinParent(MainOptionPanel);
        modIcon.OnLeftClick += (evt, elem) =>
        {
            if (evt.Target != elem) return;
            currentModIndex++;
            currentModIndex %= modIconTextures.Count;
            modIcon.Texture = modIconTextures[currentModIndex];
            FilterMod.SetText(modNames[currentModIndex]);
            modIcon.ImageScale = 80f / modIcon.Texture.Width;
            UpdateNeeded = true;
        };

        filteringName = new SUIEditableText()
        {
            RelativeMode = RelativeMode.Horizontal,
            Width = new(-120, 1f),
            Height = new(40, 0),
            BgColor = Color.Black * .3f,
            Rounded = new(8),
            Spacing = new(20, 0)
        };
        filteringName.ContentsChanged += (ref string content) =>
        {
            FilterName.SetText(content);
            UpdateNeeded = true;
        };
        filteringName.OnRightClick += (evt, elem) =>
        {
            FilterName.SetText("");
            UpdateNeeded = true;
        };
        filteringName.JoinParent(MainOptionPanel);

        OptionView = new SUIScrollView2(Orientation.Vertical)
        {
            RelativeMode = RelativeMode.None,
            Top = new(100, 0),
            Spacing = new Vector2(6)
        };
        OptionView.SetPadding(0f, 0f);
        OptionView.SetSize(0f, -90, 1f, 1f);
        OptionView.JoinParent(MainOptionPanel);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!UpdateNeeded) return;
        UpdateNeeded = false;
        MainOptionPanel.Remove();
        if (SelectionExpanded)
        {
            MainOptionPanel.JoinParent(this);
            SetUpList();
        }
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
    void SetUpList()
    {
        OptionView.ListView.RemoveAllChildren();
        //Options ??= getDefinitionListMethod.Invoke(ElementInstance, []) as IList;
        if (Options == null)
        {
            Options = getDefinitionListMethod.Invoke(ElementInstance, []) as IList;
            foreach (var p in Options)
            {
                var elem = p as UIElement;
                var eventDelegate = (MouseEvent)leftClickEvtInfo.GetValue(elem);
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        elem.OnLeftClick -= (MouseEvent)handler;
                    }
                }
                elem.OnLeftClick += (a, b) =>
                {
                    //SetValueDirect(((dynamic)p).Definition);
                    var prop = p.GetType().GetProperty("Definition");
                    SetValueDirect(prop.GetValue(p));
                    UpdateNeeded = true;
                    SelectionExpanded = false;
                    setItemMethod?.Invoke(OptionChoice, [GetValue()]);
                };
            }
        }
        IList passed = getPassedDefinitionListMethod.Invoke(ElementInstance, []) as IList;
        List<UIElement> passedElem = [];
        foreach (var p in passed)
            passedElem.Add(p as UIElement);

        float fullWidth = GetDimensions().Width - 25;
        if (passedElem.Count == 0) return;
        Vector2 unitSize = passedElem.First().GetSize() + new Vector2(8);
        int singleLineOptCount = (int)(fullWidth / unitSize.X);

        int counter = 0;
        View currentContainer = null;
        foreach (var elem in passedElem)
        {
            if (counter % singleLineOptCount == 0)
            {
                currentContainer = new View()
                {
                    Width = new(0, 1),
                    Height = new(unitSize.Y, 0),
                    RelativeMode = RelativeMode.Vertical
                };
                currentContainer.JoinParent(OptionView.ListView);
            }
            View singleContainer = new View()
            {
                Width = new(unitSize.X, 0),
                Height = new(unitSize.Y, 0),
                RelativeMode = RelativeMode.Horizontal
            };
            singleContainer.JoinParent(currentContainer);
            singleContainer.Append(elem);
            counter++;
        }
    }
    public override void Recalculate()
    {
        if (!SelectionExpanded)
        {
            Height.Set(46, 0);
            base.Recalculate();
            return;
        }
        showMaxHeight = 500;
        float h = OptionView.ListView.GetDimensions().Height + 160;
        h = Utils.Clamp(h, 160, showMaxHeight);
        Height.Set(h, 0f);
        Parent?.Height.Set(h, 0f);
        base.Recalculate();
    }
    public static bool InModernConfig;
    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        InModernConfig = true;
        base.DrawChildren(spriteBatch);
        if (OptionChoice.IsMouseHovering)
            UICommon.TooltipMouseText(tooltipProperty.GetValue(OptionChoice).ToString());
        else
            foreach (var elem in OptionView.ListView.Elements)
                foreach (var optionItem in elem.Elements)
                    if (optionItem.IsMouseHovering)
                    {
                        UICommon.TooltipMouseText(tooltipProperty.GetValue(optionItem.Elements[0]).ToString());
                        break;
                    }
        InModernConfig = false;
    }
    protected override void OnSetValueExternal(object value)
    {
        UpdateNeeded = true;
        SelectionExpanded = false;
        setItemMethod.Invoke(OptionChoice, [GetValue()]);
    }

    public override string Label => base.Label + ":" + tooltipProperty?.GetValue(OptionChoice).ToString() ?? "";
}
public class DefinitionSUIDetour : ILoadable
{
    public void Load(Mod mod)
    {
        On_UIPanel.DrawSelf += SUIize;
        MonoModHooks.Modify(typeof(NPCDefinitionOptionElement).GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance), Modify_SUIDefinition_Normal);
        MonoModHooks.Modify(typeof(BuffDefinitionOptionElement).GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance), Modify_SUIDefinition_Normal);
        MonoModHooks.Modify(typeof(ProjectileDefinitionOptionElement).GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance), Modify_SUIDefinition_Normal);
        MonoModHooks.Modify(typeof(ItemDefinitionOptionElement).GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance), Modify_SUIDefinition_Item);
        MonoModHooks.Modify(typeof(TileDefinitionOptionElement).GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance), Modify_SUIDefinition_Tile);
    }
    void Modify_SUIDefinition_Normal(ILContext il)
    {
        var cursor = new ILCursor(il);
        for (int n = 0; n < 4; n++)
            if (!cursor.TryGotoNext(i => i.MatchLdarg0()))
                return;

        var ilLabel = cursor.MarkLabel();

        cursor.Index = 0;
        if (!cursor.TryGotoNext(i => i.MatchLdarg1()))
            return;
        cursor.EmitLdloc0();
        cursor.EmitDelegate<Func<CalculatedStyle, bool>>(dimension =>
        {
            if (OptionDefinition.InModernConfig)
            {
                SDFRectangle.HasBorder(dimension.Position(), dimension.Size(), new Vector4(dimension.Width * .25f), UIStyle.PanelBg, 1f, UIStyle.PanelBorder, Main.UIScaleMatrix);

                return true;
            }
            return false;
        });
        cursor.EmitBrtrue(ilLabel);
    }
    void Modify_SUIDefinition_Item(ILContext il)
    {
        var cursor = new ILCursor(il);
        for (int n = 0; n < 5; n++)
            if (!cursor.TryGotoNext(i => i.MatchLdarg0()))
                return;

        var ilLabel = cursor.MarkLabel();

        cursor.Index = 0;
        if (!cursor.TryGotoNext(i => i.MatchLdarg1()))
            return;
        cursor.EmitLdloc0();
        cursor.EmitDelegate<Func<CalculatedStyle, bool>>(dimension =>
        {
            if (OptionDefinition.InModernConfig)
            {
                SDFRectangle.HasBorder(dimension.Position(), dimension.Size(), new Vector4(dimension.Width * .25f), UIStyle.PanelBg, 1f, UIStyle.PanelBorder, Main.UIScaleMatrix);

                return true;
            }
            return false;
        });
        cursor.EmitBrtrue(ilLabel);
    }
    void Modify_SUIDefinition_Tile(ILContext il)
    {
        var cursor = new ILCursor(il);
        for (int n = 0; n < 5; n++)
            if (!cursor.TryGotoNext(i => i.MatchLdarg0()))
                return;

        var ilLabel = cursor.MarkLabel();

        cursor.Index = 0;
        if (!cursor.TryGotoNext(i => i.MatchLdloc0()))
            return;
        cursor.EmitLdarg0();
        cursor.EmitDelegate<Func<UIElement, bool>>(element =>
        {
            if (OptionDefinition.InModernConfig)
            {
                var dimension = element.GetInnerDimensions();
                SDFRectangle.HasBorder(dimension.Position(), dimension.Size(), new Vector4(dimension.Width * .25f), UIStyle.PanelBg, 1f, UIStyle.PanelBorder, Main.UIScaleMatrix);

                return true;
            }
            return false;
        });
        cursor.EmitBrtrue(ilLabel);
    }
    private void SUIize(On_UIPanel.orig_DrawSelf orig, UIPanel self, SpriteBatch spriteBatch)
    {
        if (OptionDefinition.InModernConfig)
        {
            var dimension = self.GetDimensions();
            SDFRectangle.HasBorder(dimension.Position(), dimension.Size(), new Vector4(12f), UIStyle.PanelBg, 2f, UIStyle.PanelBorder, Main.UIScaleMatrix);
        }
        else
            orig.Invoke(self, spriteBatch);
    }

    public void Unload()
    {

    }
}
