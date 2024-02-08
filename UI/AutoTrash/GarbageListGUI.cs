using ImproveGame.Common.Configs;
using ImproveGame.Common.Players;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.AutoTrash;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Auto Trash")]
public class GarbageListGUI : BaseBody
{
    public static GarbageListGUI Instace { get; private set; }
    public GarbageListGUI() => Instace = this;

    #region 抽象实现
    public override bool Enabled
    {
        get => UIConfigs.Instance.QoLAutoTrash && Main.playerInventory && ShowWindow;
        set { }
    }

    public override bool CanSetFocusTarget(UIElement target)
    {
        return Window.IsMouseHovering;
    }
    #endregion

    /// <summary>
    /// 显示窗口
    /// </summary>
    public static bool ShowWindow { get; set; }

    public SUIPanel Window;
    public View TitleView;
    public SUITitle Title;
    public SUICross Cross;
    public GarbageListGrid GarbageListGrid;

    public override void OnInitialize()
    {
        if (Main.LocalPlayer is not null &&
            Main.LocalPlayer.TryGetModPlayer(out AutoTrashPlayer atPlayer))
        {
            // 窗口
            Window = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
            {
                Shaded = true,
                Draggable = true,
                FinallyDrawBorder = true,
                IsAdaptiveHeight = true
            };

            Window.SetPadding(0);
            Window.Width.Pixels = (44f + 4f) * 4f - 4f + 16f + 7f + 6f + 5f;
            Window.SetPosPixels(492f - Window.Width.Pixels, 306f);
            Window.JoinParent(this);

            // 标题
            TitleView = new View
            {
                DragIgnore = true,
                BgColor = Color.Lerp(Color.Black, UIStyle.TitleBg, 0.5f) * 0.5f,
                Border = 1.5f,
                BorderColor = Color.Transparent,
                Rounded = new Vector4(12f, 12f, 0f, 0f),
            };
            TitleView.SetPadding(0);
            TitleView.Width.Precent = 1f;
            TitleView.Height.Pixels = 44f;
            TitleView.JoinParent(Window);

            Title = new SUITitle("自动丢弃列表", 0.42f)
            {
                VAlign = 0.5f
            };
            Title.SetPadding(15f, 0f, 10f, 0f);
            Title.SetInnerPixels(Title.TextSize);
            Title.JoinParent(TitleView);

            Cross = new SUICross
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Rounded = new Vector4(0f, 12f, 0f, 0f),
                CrossSize = 20f,
                CrossRounded = 4.5f * 0.85f,
                Border = 1.5f,
                BorderColor = Color.Transparent,
                BgColor = Color.Transparent,
            };
            Cross.CrossOffset.X = 1f;
            Cross.Width.Pixels = 46f;
            Cross.Height.Set(0f, 1f);
            Cross.OnUpdate += (_) =>
            {
                Cross.BgColor = Cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
            };
            Cross.OnLeftMouseDown += (_, _) =>
            {
                ShowWindow = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            };
            Cross.JoinParent(TitleView);

            GarbageListGrid = new GarbageListGrid
            {
                RelativeMode = RelativeMode.Vertical,
            };
            GarbageListGrid.SetPadding(7f, 4f, 6f, 8f);
            GarbageListGrid.Width.Percent = 1f;
            GarbageListGrid.SetInnerPixels(0f, (44f + 4f) * 4f - 4f);
            GarbageListGrid.JoinParent(Window);
        }
    }

    /// <summary>
    /// 如果两个值不同，就把第一个值设置成第二个
    /// </summary>
    public static bool Different(ref float value1, float value2)
    {
        if (value1 != value2)
        {
            value1 = value2;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 如果两个值不同，就把第一个值设置成第二个
    /// </summary>
    public static bool Different(ref Vector2 value1, Vector2 value2)
    {
        if (value1 != value2)
        {
            value1 = value2;
            return true;
        }

        return false;
    }

    public override void Update(GameTime gameTime)
    {
        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Garbage List GUI");
        }

        base.Update(gameTime);
    }
}
