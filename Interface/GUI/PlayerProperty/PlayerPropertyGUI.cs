using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;

namespace ImproveGame.Interface.GUI.PlayerProperty;

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

        foreach (var item in PlayerPropertySystem.Instance.PropertyCategories)
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

        var Title = new SUITitle(GetText("UI.PlayerProperty.Control"), 0.42f)
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

        foreach (var item in PlayerPropertySystem.Instance.PropertyCategories)
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
        #endregion

        #region 控制器的开关按钮
        ControllerSwitch = new PropertyToggle();
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
    /// 进入游戏时调用，加载收藏列表
    /// </summary>
    public void LoadAndSetupFavorites()
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out UIPlayerSetting playerSetting))
            return;

        var proCats = PlayerPropertySystem.Instance.PropertyCategories;

        foreach (var item in proCats)
        {
            if (playerSetting.ProCatsPos.TryGet(item.Key, out Vector2 pos))
            {
                item.Value.UIPosition = pos;
            }

            if (playerSetting.ProCatsFav.TryGet(item.Key, out bool fav))
            {
                item.Value.Favorite = fav;
            }

            if (playerSetting.ProFavs.TryGet(item.Key, out TagCompound tags))
            {
                foreach (var pro in item.Value.BaseProperties)
                {
                    if (tags.TryGet(pro.Name, out bool proFav))
                    {
                        pro.Favorite = proFav;
                    }
                }
            }
        }

        // 添加到展示列表
        foreach (var item in PlayerPropertySystem.Instance.PropertyCategories)
        {
            if (item.Value.Favorite)
            {
                PropertyCard card = CreatePropertyCardForDisplay(Body, item.Value);
                card.Join(Body);
            }
        }
    }

    /// <summary>
    /// 去除重复的和未收藏的
    /// </summary>
    private void Body_OnUpdate(UIElement body)
    {
        List<UIElement> list = body.Children.ToList();
        HashSet<BasePropertyCategory> appeared = new HashSet<BasePropertyCategory>();

        for (int i = 0; i < list.Count; i++)
        {
            var uie = list[i];

            if (uie is PropertyCard card)
            {
                // 去重 proCat
                if (appeared.Contains(card.PropertyCategory))
                {
                    uie.Remove();
                    break;
                }

                appeared.Add(card.PropertyCategory);

                // 去掉未收藏
                if (!card.PropertyCategory.Favorite)
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (ControllerSwitch.IsMouseHovering)
        {
            UICommon.TooltipMouseText(GetText("UI.PlayerProperty.Introduction"));
        }
    }

    /// <summary>
    /// 创建展示用的属性卡片
    /// </summary>
    public static PropertyCard CreatePropertyCardForDisplay(View view, BasePropertyCategory proCat)
    {
        PropertyCard card = proCat.CreateCard(out _, out _);
        proCat.AppendProperties(card);

        if (proCat.UIPosition is Vector2 pos)
        {
            card.SetPosPixels(pos);
        }
        else
        {
            if (view.Children.Any())
            {
                var uie = view.Children.Last();
                card.SetPosPixels(uie.Right() + 4f, uie.Top.Pixels);
            }
            else
            {
                card.SetPosPixels(620f, 20f);
            }
        }

        card.Draggable = true;
        card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);

        return card;
    }

    /// <summary>
    /// 创建控制用的卡片
    /// </summary>
    public static PropertyCard CreateCardForControl(View view, BasePropertyCategory proCat)
    {
        PropertyCard card = proCat.CreateCard(out _, out _);
        proCat.AppendPropertiesForControl(view, card);

        card.OnUpdate += (_) =>
        {
            if (card.PropertyCategory.Favorite)
            {
                card.BorderColor = UIColor.ItemSlotBorderFav;
            }
            else
            {
                card.BorderColor = UIColor.PanelBorder;
            }
        };

        card.TitleView.OnUpdate += (_) =>
        {
            card.TitleView.Border = card.TitleView.HoverTimer.Lerp(0, 2);
            card.TitleView.BorderColor = card.TitleView.HoverTimer.Lerp(UIColor.PanelBorder, UIColor.ItemSlotBorderFav);
        };

        card.TitleView.OnLeftMouseDown += (_, _) =>
        {
            proCat.Favorite = !proCat.Favorite;

            if (proCat.Favorite)
            {
                // 正常版的创建
                PropertyCard innerCard = CreatePropertyCardForDisplay(view, proCat);
                innerCard.Join(view);
            }
        };

        card.Relative = RelativeMode.Vertical;
        card.Spacing = new Vector2(4f);
        card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);

        return card;
    }
}