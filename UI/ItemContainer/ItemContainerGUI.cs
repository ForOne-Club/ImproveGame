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

    public IItemContainer Container;

    public SUIPanel Window = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
    {
        HAlign = 0.5f,
        VAlign = 0.5f,
        Shaded = true,
        Draggable = true,
        FinallyDrawBorder = true,
        IsAdaptiveWidth = true,
        IsAdaptiveHeight = true,
    };

    // 标题
    public readonly View TitlePanel = ViewHelper.CreateHead(UIStyle.TitleBg2, 50f, 12f);
    public readonly SUIText Title = new SUIText
    {
        IsLarge = true,
        TextScale = 0.5f,
        TextAlign = new Vector2(0, 0.5f),
        DragIgnore = true,
    };
    public readonly SUICross CloseButton = new SUICross
    {
        HAlign = 1f,
        VAlign = 0.5f,
        Rounded = new Vector4(0f, 12f, 0f, 0f),
        CrossSize = 24f,
        CrossRounded = 4.5f * 0.95f,
        Border = 1.5f,
        BorderColor = Color.Transparent,
        BgColor = Color.Transparent,
    };

    // 开关按钮
    public readonly View SwitchPanel = new View
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(4f),
        IsAdaptiveWidth = true,
        IsAdaptiveHeight = true,
        PaddingLeft = 10f,
    };
    public SUISwitch AutoStorageSwitch, AutoSortSwitch;

    public readonly ItemContainerGrid ItemContainerGrid = new ItemContainerGrid
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(4),
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

        CloseButton.CrossOffset += Vector2.One;
        CloseButton.Width.Pixels = 55f;
        CloseButton.Height.Set(0f, 1f);
        CloseButton.OnUpdate += (_) =>
        {
            CloseButton.BgColor = CloseButton.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        CloseButton.OnLeftMouseDown += (_, _) => Close();
        CloseButton.JoinParent(TitlePanel);
        #endregion

        SwitchPanel.JoinParent(Window);
        AutoStorageSwitch =
            new SUISwitch(() => Container.AutoStorage, state => Container.AutoStorage = state, GetText("PackageGUI.AutoStorage"), 0.8f)
            {
                RelativeMode = RelativeMode.Horizontal, Spacing = new Vector2(8),
            };
        AutoStorageSwitch.JoinParent(SwitchPanel);

        AutoSortSwitch =
            new SUISwitch(() => Container.AutoSort, state => Container.AutoSort = state, GetText("PackageGUI.AutoSort"), 0.8f)
            {
                RelativeMode = RelativeMode.Horizontal, Spacing = new Vector2(8),
            };
        AutoSortSwitch.JoinParent(SwitchPanel);

        ItemContainerGrid.SetPadding(10f, 0f, 9f, 9f).SetInnerPixels(ItemContainerGrid.Width.Pixels, ItemContainerGrid.Height.Pixels);
        ItemContainerGrid.OnLeftMouseDown += (_, _) =>
        {
            if (!Main.mouseItem.IsAir && !Main.LocalPlayer.ItemAnimationActive && Container.MeetEntryCriteria(Main.mouseItem))
            {
                Container.PutInPackage(ref Main.mouseItem);
            }
        };
        ItemContainerGrid.JoinParent(Window);
    }

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
        ItemContainerGrid.Scrollbar.BarTop = 0;
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