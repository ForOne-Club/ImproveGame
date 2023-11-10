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

    public override bool CanPriority(UIElement target) => target != this;

    public override bool CanDisableMouse(UIElement target)
    {
        if (target != this)
        {
            foreach (var item in Children)
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

    public PropertyGrid PropertyGrid { get; set; }
    public SUIPanel Window { get; set; }

    public override void OnInitialize()
    {
        SUIImage button = new SUIImage(ModAsset.Flying.Value);
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

        // 所有的
        foreach (var item in PlayerPropertySystem.Instance.Miximixis)
        {
            var card = item.Value.CreateCard(out _, out _, out _);
            item.Value.AppendPropertys(card);
            card.Draggable = true;
            card.Join(this);
        }

        // 控制窗口
        Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg, 10, 2, true);

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

        // 列表
        PropertyGrid = new PropertyGrid();

        foreach (var item in PlayerPropertySystem.Instance.Miximixis)
        {
            var card = item.Value.CreateCard(out TimerView titleView, out _, out _);
            item.Value.AppendPropertys(card);
            card.Relative = RelativeMode.Vertical;
            card.Spacing.Y = 4f;
            card.Join(PropertyGrid.ListView);
        }

        PropertyGrid.Top.Pixels = TitleView.BottomPixels();
        PropertyGrid.SetPadding(7f);
        PropertyGrid.PaddingTop -= 3;
        PropertyGrid.PaddingRight -= 1;
        PropertyGrid.SetInnerPixels(160f + 21f, 250f);
        PropertyGrid.Join(Window);

        Window.SetPadding(0);
        Window.SetInnerPixels(PropertyGrid.Width.Pixels, PropertyGrid.BottomPixels());

        Window.HAlign = 0.5f;
        Window.Join(this);

        // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
        // new PlyInfoCard(GetHJSON("WingTimeMax"),
        //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var card in Children)
        {
            if (card.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerProperty GUI");
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        base.Update(gameTime);
    }
}