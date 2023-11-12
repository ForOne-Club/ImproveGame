using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI.PlayerProperty;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerProperty;

public class PlayerPropertyGUI : ViewBody
{
    #region ViewBody
    public override bool Display { get => Visible; set => Visible = value; }

    public override bool CanPriority(UIElement target) => this != target && this != Body;

    public override bool CanDisableMouse(UIElement target)
    {
        if (target != this)
        {
            foreach (var item in Children)
            {
                if (item != Body && (item.IsMouseHovering || (item is View view && view.KeepPressed)))
                {
                    return true;
                }
            }

            foreach (var item in Body.Children)
            {
                if (item.IsMouseHovering || (item is View view && view.KeepPressed))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool _visible;

    public static bool Visible
    {
        get => UIConfigs.Instance.PlyInfo switch
        {
            UIConfigs.PAPDisplayMode.AlwaysDisplayed => _visible,
            UIConfigs.PAPDisplayMode.WhenOpeningBackpack => Main.playerInventory && _visible,
            UIConfigs.PAPDisplayMode.NotDisplayed => false,
            _ => false
        };
        set => _visible = value;
    }
    #endregion

    public View Body { get; set; }
    public SUIPanel Window { get; set; }
    public PropertyGrid PropertyGrid { get; set; }

    public SUIImage ControllerSwitch { get; set; }
    public static Vector2 SwitchPosition =>
        Main.playerInventory ? new Vector2(572, 20) : new Vector2(470, 25);

    public override void OnInitialize()
    {
        #region 所有的属性面板
        Body = new()
        {
            Width = new StyleDimension(0, 1f),
            Height = new StyleDimension(0, 1f),
        };
        Body.OnUpdate += Body_OnUpdate;
        Body.Join(this);

        foreach (var item in PlayerPropertySystem.Instance.Miximixis)
        {
            if (item.Value.Favorite)
            {
                PropertyCard card = CreatePropertyCardForDisplay(Body, item.Value);
                card.Join(Body);
            }
        }
        #endregion

        #region 控制窗口
        Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg, 10, 2, true);

        #region 标题栏
        // 标题
        var TitleView = new View
        {
            DragIgnore = true,
            BgColor = UIColor.TitleBg2,
            Border = 2f,
            BorderColor = UIColor.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        TitleView.SetPadding(0);
        TitleView.Width.Precent = 1f;
        TitleView.Height.Pixels = 42f;
        TitleView.Join(Window);

        var Title = new SUITitle("属性控制", 0.42f)
        {
            VAlign = 0.5f
        };
        Title.SetPadding(15f, 0f, 10f, 0f);
        Title.SetInnerPixels(Title.TextSize);
        Title.Join(TitleView);

        var Cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f
        };
        Cross.Width.Pixels = 42f;
        Cross.Height.Set(0f, 1f);
        Cross.Join(TitleView);

        Cross.OnLeftMouseDown += (_, _) =>
        {
            RemoveChild(Window);
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        #endregion

        // 一整个列表
        PropertyGrid = new PropertyGrid();

        foreach (var item in PlayerPropertySystem.Instance.Miximixis)
        {
            PropertyCard card = CreateCardForControl(Body, item.Value);

            var list1 = PropertyGrid.ListView.Children;
            var list2 = PropertyGrid.ListView2.Children;

            var b1 = list1.LastOrDefault()?.Bottom() ?? 0f;
            var b2 = list2.LastOrDefault()?.Bottom() ?? 0f;

            if (b1 <= b2)
            {
                card.Join(PropertyGrid.ListView);
            }
            else
            {
                card.Join(PropertyGrid.ListView2);
            }
        }

        PropertyGrid.Top.Pixels = TitleView.BottomPixels();
        PropertyGrid.SetPadding(7f);
        PropertyGrid.PaddingTop -= 3;
        PropertyGrid.PaddingRight -= 1;
        PropertyGrid.SetInnerPixels(160f * 2f + 4 + 21f, 250f);
        PropertyGrid.Join(Window);

        Window.SetPadding(0);
        Window.SetInnerPixels(PropertyGrid.Width.Pixels, PropertyGrid.BottomPixels());

        Window.HAlign = Window.VAlign = 0.5f;
        Window.Join(this);
        #endregion

        #region 控制器的开关按钮
        ControllerSwitch = new SUIImage(ModAsset.Luck2.Value);
        ControllerSwitch.SetPosPixels(SwitchPosition);
        ControllerSwitch.SetSizePixels(ControllerSwitch.Texture.Size());
        ControllerSwitch.OnUpdate +=
            (_) =>
            {
                if (ControllerSwitch.GetPosPixel() != SwitchPosition)
                {
                    ControllerSwitch.SetPosPixels(SwitchPosition);
                    Recalculate();
                }

                if (ControllerSwitch.IsMouseHovering)
                {
                    ControllerSwitch.Texture = ModAsset.Luck3.Value;
                }
                else
                {
                    ControllerSwitch.Texture = ModAsset.Luck2.Value;
                }
            };
        ControllerSwitch.OnLeftMouseDown +=
            (_, _) =>
            {
                if (HasChild(Window))
                {
                    RemoveChild(Window);
                }
                else
                {
                    Window.Join(this);
                }
            };
        ControllerSwitch.Join(this);
        #endregion
    }

    /// <summary>
    /// 去除重复的和未收藏的
    /// </summary>
    private void Body_OnUpdate(UIElement body)
    {
        List<UIElement> list = body.Children.ToList();
        HashSet<Miximixi> appeared = new HashSet<Miximixi>();

        for (int i = 0; i < list.Count; i++)
        {
            var uie = list[i];

            if (uie is PropertyCard card)
            {
                // 去重 mixi
                if (appeared.Contains(card.Miximixi))
                {
                    uie.Remove();
                    break;
                }

                appeared.Add(card.Miximixi);

                // 去掉未收藏
                if (!card.Miximixi.Favorite)
                {
                    uie.Remove();
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        bool b = false;

        foreach (var item in Children)
        {
            if (item != Body && (item.IsMouseHovering || (item is View view && view.KeepPressed)))
            {
                b = true;
                break;
            }
        }

        foreach (var item in Body.Children)
        {
            if (item.IsMouseHovering || (item is View view && view.KeepPressed))
            {
                b = true;
                break;
            }
        }

        if (b)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerProperty GUI");
            Main.LocalPlayer.mouseInterface = true;
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// 创建展示用的属性卡片
    /// </summary>
    public static PropertyCard CreatePropertyCardForDisplay(View view, Miximixi miximixi)
    {
        PropertyCard card = miximixi.CreateCard(out _, out _, out _);
        miximixi.AppendPropertys(card);

        if (miximixi.UIPosition is Vector2 pos)
        {
            card.SetPosPixels(pos);
        }
        else if (view.Children.Last() is UIElement uie)
        {
            card.SetPosPixels(uie.Right() + 4f, uie.Top.Pixels);
        }

        card.Draggable = true;
        card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);

        return card;
    }

    /// <summary>
    /// 创建控制用的卡片
    /// </summary>
    public static PropertyCard CreateCardForControl(View view, Miximixi miximixi)
    {
        PropertyCard card = miximixi.CreateCard(out TimerView titleView, out _, out _);
        miximixi.AppendPropertysForControl(view, card);

        card.OnUpdate += (_) =>
        {
            if (card.Miximixi.Favorite)
                card.BorderColor = UIColor.ItemSlotBorderFav;
            else
                card.BorderColor = UIColor.PanelBorder;
        };

        titleView.OnUpdate += (_) =>
        {
            titleView.Border = titleView.HoverTimer.Lerp(0, 2);
            titleView.BorderColor = titleView.HoverTimer.Lerp(UIColor.PanelBorder, UIColor.ItemSlotBorderFav);
        };

        titleView.OnLeftMouseDown += (_, _) =>
        {
            miximixi.Favorite = !miximixi.Favorite;

            if (miximixi.Favorite)
            {
                // 正常版的创建
                PropertyCard innerCard = CreatePropertyCardForDisplay(view, miximixi);
                innerCard.Join(view);
            }
        };

        card.Relative = RelativeMode.Vertical;
        card.Spacing = new Vector2(4f);
        card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);

        return card;
    }
}