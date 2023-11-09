using ImproveGame.Common.Configs;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using ImproveGame.Interface.UIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class GarbageListGUI : ViewBody
{
    #region 抽象实现
    public override bool Display
    {
        get => UIConfigs.Instance.QoLAutoTrash && Main.playerInventory && ShowWindow;
        set { }
    }

    public override bool CanDisableMouse(UIElement target)
    {
        return Window.IsMouseHovering;
    }

    public override bool CanPriority(UIElement target)
    {
        return Window.IsMouseHovering;
    }
    #endregion

    /// <summary>
    /// 显示窗口
    /// </summary>
    public static bool ShowWindow;

    public SUIPanel Window;
    public View TitleView;
    public SUITitle Title;
    public SUICross Cross;
    public View GarbagesView;
    public GarbageListGrid GarbageListGrid;

    public override void OnInitialize()
    {
        if (Main.LocalPlayer is not null &&
            Main.LocalPlayer.TryGetModPlayer(out AutoTrashPlayer atPlayer))
        {
            Vector2 gridSize = ScrollView.GridSize(44f, 4f, 4, 4) + new Vector2(20f, 0f);

            // 窗口
            Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                Shaded = true,
                Draggable = true
            };
            Window.SetPosPixels(20f, 306f);
            Window.SetPadding(0);
            Window.SetSizePixels(gridSize.X + 14f, 42f + gridSize.Y + 11f);
            Window.Join(this);

            // 标题
            TitleView = new View
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

            Title = new SUITitle("自动丢弃列表", 0.42f)
            {
                VAlign = 0.5f
            };
            Title.SetPadding(15f, 0f, 10f, 0f);
            Title.SetInnerPixels(Title.TextSize);
            Title.Join(TitleView);

            Cross = new SUICross
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
                ShowWindow = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            };

            GarbagesView = new View();
            GarbagesView.SetPadding(7f, 4f, 6f, 7f);
            GarbagesView.Top.Pixels = TitleView.BottomPixels();
            GarbagesView.Width.Percent = 1f;
            GarbagesView.Height.Pixels = Window.Height.Pixels - 42f;
            GarbagesView.Join(Window);

            GarbageListGrid = new GarbageListGrid();
            GarbageListGrid.Width.Percent = GarbageListGrid.Height.Percent = 1f;
            GarbageListGrid.Join(GarbagesView);
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
