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
            if (_enabled && !Main.playerInventory)
            {
                _enabled = false;
                StartTimer.Close();
            }

            return StartTimer.Closing || _enabled;
        }
        set
        {
            _enabled = value;
            if (_enabled)
                StartTimer.Open();
            else
                StartTimer.Close();
        }
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
        Width = { Pixels = 300f },
        IsAdaptiveWidth = false, IsAdaptiveHeight = true,
    };

    // 标题
    public readonly View TitlePanel = ViewHelper.CreateHead(UIStyle.TitleBg2 * 0.75f, 45f, 12f);
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
    public readonly View SwitchView = new View();

    public readonly ItemContainerGridLayout ItemContainerGrid = new ItemContainerGridLayout
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(6f),
    };

    public override void OnInitialize()
    {
        StartTimer.State = AnimationState.Closed;

        Window.SetPadding(0f);
        Window.JoinParent(this);

        #region 标题组件
        TitlePanel.SetPadding(0);
        TitlePanel.JoinParent(Window);

        Title.SetPadding(12f, 0f, 0f, 0f);
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
        SwitchView.RelativeMode = RelativeMode.Vertical;
        SwitchView.Spacing = new Vector2(6f);
        SwitchView.Width = new StyleDimension { Pixels = 0, Percent = 1f };
        SwitchView.IsAdaptiveHeight = true;
        SwitchView.SetPadding(12f, 0f);
        SwitchView.JoinParent(Window);

        View switchView1 = SUIToggleSwitch.CreateTextSwitch(out var toggleSwitch, out var text);
        switchView1.Width.Percent = 1f;
        switchView1.IsAdaptiveHeight = true;
        switchView1.RelativeMode = RelativeMode.Vertical;
        switchView1.Spacing = new Vector2(4);

        text.Height.Pixels = 20f;
        text.TextScale = 0.8f;
        text.TextOrKey = GetText("PackageGUI.AutoStorage");
        text.SetInnerPixels(text.TextSize.X, 20f);
        text.SetSizePixels(text.TextSize * text.TextScale);

        toggleSwitch.Status += () => Container.AutoStorage;
        toggleSwitch.Switch += () => Container.AutoStorage = !Container.AutoStorage;
        toggleSwitch.Rounded = new Vector4(10f);
        toggleSwitch.OnUpdate += _ =>
        {
            toggleSwitch.BorderColor =
                toggleSwitch.SwitchTimer.Lerp(UIStyle.ScrollBarBorder, UIStyle.ItemSlotBorderFav);
            toggleSwitch.ToggleCircleColor =
                toggleSwitch.SwitchTimer.Lerp(UIStyle.ScrollBarBorder, UIStyle.ItemSlotBorderFav);
        };
        toggleSwitch.SetSizePixels(32f, 20f);

        switchView1.JoinParent(SwitchView);

        View switchView2 = SUIToggleSwitch.CreateTextSwitch(out var toggleSwitch2, out var text2);
        switchView2.Width.Percent = 1f;
        switchView2.IsAdaptiveHeight = true;
        switchView2.RelativeMode = RelativeMode.Vertical;
        switchView2.Spacing = new Vector2(8);

        text2.TextScale = 0.8f;
        text2.TextOrKey = GetText("PackageGUI.AutoSort");
        text2.SetInnerPixels(text2.TextSize.X, 20f);
        text2.SetSizePixels(text2.TextSize * text2.TextScale);

        toggleSwitch2.Status += () => Container.AutoSort;
        toggleSwitch2.Switch += () => Container.AutoSort = !Container.AutoSort;
        toggleSwitch2.Rounded = new Vector4(10f);
        toggleSwitch2.OnUpdate += _ =>
        {
            toggleSwitch2.BorderColor =
                toggleSwitch2.SwitchTimer.Lerp(UIStyle.ScrollBarBorder, UIStyle.ItemSlotBorderFav);
            toggleSwitch2.ToggleCircleColor =
                toggleSwitch2.SwitchTimer.Lerp(UIStyle.ScrollBarBorder, UIStyle.ItemSlotBorderFav);
        };
        toggleSwitch2.SetSizePixels(32f, 20f);

        switchView2.JoinParent(SwitchView);
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

    public override void Update(GameTime gameTime)
    {
        StartTimer.Update();
        base.Update(gameTime);

        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
        }
    }

    public void Open(IItemContainer container)
    {
        Enabled = true;

        SoundEngine.PlaySound(SoundID.MenuOpen);
        OperateInventory(true);
        ItemContainerGrid.SetInventory(container.ItemContainer);
        Title.TextOrKey = container.Name;
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

    public AnimationTimer StartTimer = new(3);

    public override bool RenderTarget2DDraw => !StartTimer.Opened;
    public override Vector2 RenderTarget2DScale => new Vector2(0.95f + StartTimer * 0.05f);
    public override float RenderTarget2DOpacity => StartTimer.Schedule;
    public override Vector2 RenderTarget2DPosition => Window.GetDimensionsCenter();
    public override Vector2 RenderTarget2DOrigin => Window.GetDimensionsCenter();
}