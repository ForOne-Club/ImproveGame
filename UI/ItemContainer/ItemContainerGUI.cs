using ImproveGame.UI.ItemContainer.Elements;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.ItemContainer;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Item Container GUI")]
public class ItemContainerGUI : BaseBody
{
    #region Base
    public static ItemContainerGUI Instace { get; private set; }
    public ItemContainerGUI() => Instace = this;

    public override bool Enabled
    {
        get
        {
            if (!Main.playerInventory)
                _enabled = false;
            return _enabled;
        }
        set => _enabled = value;
    }
    private static bool _enabled;
    #endregion

    public IItemContainer Container { get; private set; }

    // 窗口
    public SUIPanel Window = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
    {
        Shaded = true,
        Draggable = true,
        FinallyDrawBorder = true,
        HAlign = 0.5f, VAlign = 0.5f,
        IsAdaptiveWidth = true, IsAdaptiveHeight = true,
    };

    // 标题
    public readonly View TitlePanel = ViewHelper.CreateHead(UIStyle.TitleBg2, 45f, 12f);
    public readonly SUIText Title = new SUIText
    {
        DragIgnore = true,
        IsLarge = true,
        TextScale = 0.5f,
        TextAlign = new Vector2(0, 0.5f),
    };
    public readonly SUICross Cross = new SUICross
    {
        HAlign = 1f, VAlign = 0.5f,
        Rounded = new Vector4(0f, 12f, 0f, 0f),
        CrossSize = 22f, CrossRounded = 4.5f * 0.85f,
        BgColor = Color.Transparent,
        Border = 0f, BorderColor = Color.Transparent,
    };

    // 开关按钮
    public readonly View SwitchView = new View
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(6f),
        IsAdaptiveWidth = true,
        Height = new StyleDimension(20f, 0f),
        PaddingLeft = 12f,
    };

    public readonly ItemContainerGrid ItemContainerGrid = new ItemContainerGrid
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(6f),
    };

    public override void OnInitialize()
    {
        Window.SetPadding(0);
        Window.JoinParent(this);

        #region 标题组件
        TitlePanel.SetPadding(0);
        TitlePanel.JoinParent(Window);

        Title.SetPadding(20f, 0f, 10f, 0f);
        Title.Height.Percent = 1f;
        Title.Width.Pixels = 200f;
        Title.JoinParent(TitlePanel);

        Cross.CrossOffset += Vector2.One;
        Cross.Width.Pixels = 50f;
        Cross.Height.Set(0f, 1f);
        Cross.OnUpdate += (_) =>
        {
            Cross.BgColor = Cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        Cross.OnLeftMouseDown += (_, _) => Close();
        Cross.JoinParent(TitlePanel);
        #endregion

        #region Switch
        SwitchView.JoinParent(Window);

        View view = SUIToggleSwitch.CreateTextSwitch(out var toggleSwitch, out var text);
        view.VAlign = 0.5f;
        view.IsAdaptiveWidth = view.IsAdaptiveHeight = true;
        view.RelativeMode = RelativeMode.Horizontal;
        view.Spacing = new Vector2(4);

        toggleSwitch.Status += () => Container.AutoStorage;
        toggleSwitch.Switch += () => Container.AutoStorage = !Container.AutoStorage;
        toggleSwitch.Rounded = new Vector4(10f);
        toggleSwitch.SetSizePixels(32f, 20f);

        text.TextScale = 0.8f;
        text.TextOrKey = GetText("PackageGUI.AutoStorage");
        text.SetSizePixels(text.TextSize * text.TextScale);
        view.JoinParent(SwitchView);

        View view2 = SUIToggleSwitch.CreateTextSwitch(out var toggleSwitch2, out var text2);
        view2.VAlign = 0.5f;
        view2.IsAdaptiveWidth = view2.IsAdaptiveHeight = true;
        view2.RelativeMode = RelativeMode.Horizontal;
        view2.Spacing = new Vector2(8);

        toggleSwitch2.Status += () => Container.AutoSort;
        toggleSwitch2.Switch += () => Container.AutoSort = !Container.AutoSort;
        toggleSwitch2.Rounded = new Vector4(10f);
        toggleSwitch2.SetSizePixels(32f, 20f);

        text2.TextScale = 0.8f;
        text2.TextOrKey = GetText("PackageGUI.AutoSort");
        text2.SetSizePixels(text2.TextSize * text2.TextScale);
        view2.JoinParent(SwitchView);
        #endregion

        ItemContainerGrid.SetPadding(8f);
        ItemContainerGrid.PaddingTop = 0f;
        ItemContainerGrid.SetSizePixels(0, 220f);
        ItemContainerGrid.OnLeftMouseDown += (_, _) =>
        {
            if (!Main.mouseItem.IsAir && !Main.LocalPlayer.ItemAnimationActive && Container.MeetEntryCriteria(Main.mouseItem))
            {
                Container.ItemIntoContainer(Main.mouseItem);
            }
        };
        ItemContainerGrid.JoinParent(Window);
    }

    public bool Ya;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
        }
    }

    public void Open(string title, IItemContainer container)
    {
        Enabled = true;

        SoundEngine.PlaySound(SoundID.MenuOpen);
        Main.playerInventory = true;
        ItemContainerGrid.SetInventory(container.ItemContainer);
        Title.TextOrKey = title;
        Title.SetInnerPixels(Title.TextSize);
        Container = container;
        Recalculate();
    }

    public void Close()
    {
        Enabled = false;

        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override bool CanSetFocusTarget(UIElement target)
    {
        return target != this && Window.IsMouseHovering || Window.IsLeftMousePressed;
    }
}