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
    #endregion

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

    public View Body { get; set; }
    public SUIPanel Window { get; set; }
    public PropertyGrid PropertyGrid { get; set; }

    public override void OnInitialize()
    {
        Body = new()
        {
            Width = new StyleDimension(0, 1f),
            Height = new StyleDimension(0, 1f),
        };
        Body.OnUpdate += Body_OnUpdate;
        Body.Join(this);

        // 开关控制器的按钮
        SUIImage button = new SUIImage(ModAsset.Luck.Value);
        button.SetPosPixels(572, 20);
        button.SetSizePixels(button.Texture.Size());
        button.OnUpdate +=
            (_) => { button.ImageColor = Color.White * button.HoverTimer.Lerp(0.5f, 1f); };
        button.OnLeftMouseDown +=
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
        button.Join(this);

        // 所有的属性面板
        foreach (var item in PlayerPropertySystem.Instance.Miximixis)
        {
            if (item.Value.Favorite)
            {
                PropertyCard card = item.Value.CreateCard(out _, out _, out _);
                item.Value.AppendPropertys(card, null);

                if (item.Value.UIPosition is Vector2 pos)
                {
                    card.SetPosPixels(pos);
                }
                else if (Body.Children.Last() != null)
                {
                    card.SetPosPixels(Body.Children.Last().Right() + 4f, Body.Children.Last().Top.Pixels);
                }

                card.Draggable = true;
                card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);
                card.Join(Body);
            }
        }

        // 控制窗口
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
            PropertyCard card = item.Value.CreateCard(out TimerView titleView, out _, out _);
            item.Value.AppendPropertys(card, (bala, bar) =>
                {
                    bar.OnUpdate += (_) =>
                    {
                        bar.BorderColor = bala.Favorite ? Color.Transparent : (Color.Red * 0.85f);
                        bar.Border = bala.Favorite ? 0 : 2;
                    };

                    bar.OnLeftMouseDown += (_, _) =>
                    {
                        bala.Favorite = !bala.Favorite;

                        foreach (var item in Body.Children)
                        {
                            if (item is PropertyCard innerCard && innerCard.Miximixi == card.Miximixi)
                            {
                                var first = innerCard.Children.First();
                                innerCard.RemoveAllChildren();
                                innerCard.Append(first);
                                innerCard.Miximixi.AppendPropertys(innerCard, null);
                            }
                        }
                    };

                    return true;
                });

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
                item.Value.Favorite = !item.Value.Favorite;

                if (item.Value.Favorite)
                {
                    // 正常版的创建
                    PropertyCard innerCard = item.Value.CreateCard(out _, out _, out _);
                    item.Value.AppendPropertys(innerCard, null);

                    if (item.Value.UIPosition is Vector2 pos)
                    {
                        innerCard.SetPosPixels(pos);
                    }
                    else if (Body.Children.Count() > 0)
                    {
                        innerCard.SetPosPixels(Body.Children.Last().Right() + 4f, Body.Children.Last().Top.Pixels);
                    }

                    innerCard.Draggable = true;
                    innerCard.SetInnerPixels(160, (30 + 2) * innerCard.Children.Count() - 2);
                    innerCard.Join(Body);
                }
            };

            card.Relative = RelativeMode.Vertical;
            card.Spacing.Y = 4f;
            card.SetInnerPixels(160, (30 + 2) * card.Children.Count() - 2);

            float height1 = PropertyGrid.ListView.Children.Count() > 0 ? PropertyGrid.ListView.Children.Last().Bottom() : 0f;
            float height2 = PropertyGrid.ListView2.Children.Count() > 0 ? PropertyGrid.ListView2.Children.Last().Bottom() : 0f;

            card.Join(height1 > height2 ? PropertyGrid.ListView : PropertyGrid.ListView2);
        }

        PropertyGrid.Top.Pixels = TitleView.BottomPixels();
        PropertyGrid.SetPadding(7f);
        PropertyGrid.PaddingTop -= 3;
        PropertyGrid.PaddingRight -= 1;
        PropertyGrid.SetInnerPixels(160f * 2f + 4 + 21f, 250f);
        PropertyGrid.Join(Window);

        Window.SetPadding(0);
        Window.SetInnerPixels(PropertyGrid.Width.Pixels, PropertyGrid.BottomPixels());

        Window.HAlign = 0.5f;
        Window.Join(this);
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
                if (appeared.Contains(card.Miximixi))
                {
                    appeared.Remove(card.Miximixi);
                    break;
                }

                appeared.Add(card.Miximixi);

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
}