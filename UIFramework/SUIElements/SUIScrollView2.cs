using ImproveGame.Helpers.Extensions;
using ImproveGame.UIFramework.BaseViews;
using System.Reflection;

namespace ImproveGame.UIFramework.SUIElements;

/// <summary>
/// 方位, 朝向
/// </summary>
public enum Orientation
{
    /// <summary> 横向 </summary>
    Horizontal,
    /// <summary> 纵向 </summary>
    Vertical
}

/// <summary>
/// 为了更方便的使用滚动列表而设计，支持横向滚动和纵向滚动 <br/>
/// 了解工作逻辑请查看 <see cref="FixedSize"/> <see cref="Update(GameTime)"/> 两处代码
/// </summary>
public class SUIScrollView2 : TimerView
{
    /// <summary>
    /// 滚动朝向
    /// </summary>
    public readonly Orientation ScrollOrientation;

    /// <summary>
    /// 固定大小
    /// </summary>
    public bool FixedSize
    {
        get => _fixedSize;
        set
        {
            _fixedSize = value;

            if (_fixedSize)
            {
                if (ScrollOrientation == Orientation.Horizontal)
                {
                    IsAdaptiveHeight = false;

                    MaskView.IsAdaptiveHeight = false;
                    MaskView.Width.Percent = 1f;

                    ListView.IsAdaptiveHeight = false;
                    ListView.Width.Percent = 1f;
                }
                else if (ScrollOrientation == Orientation.Vertical)
                {
                    IsAdaptiveWidth = false;

                    MaskView.IsAdaptiveWidth = false;
                    MaskView.Width.Percent = 1f;

                    ListView.IsAdaptiveWidth = false;
                    ListView.Width.Percent = 1f;
                }
            }
            else
            {
                if (ScrollOrientation == Orientation.Horizontal)
                {
                    IsAdaptiveHeight = true;

                    MaskView.IsAdaptiveHeight = true;
                    MaskView.Width.Percent = 0f;

                    ListView.IsAdaptiveHeight = true;
                    ListView.Width.Percent = 0f;
                }
                else if (ScrollOrientation == Orientation.Vertical)
                {
                    IsAdaptiveWidth = true;

                    MaskView.IsAdaptiveWidth = true;
                    MaskView.Width.Percent = 0f;

                    ListView.IsAdaptiveWidth = true;
                    ListView.Width.Percent = 0f;
                }
            }

            Recalculate();
        }
    }
    protected bool _fixedSize;

    /// <summary>
    /// 滚动条是否常驻
    /// </summary>
    public bool ScrollbarPermanent;

    /// <summary>
    /// 蒙版
    /// </summary>
    public TimerView MaskView { get; init; } = new();
    /// <summary>
    /// 列表
    /// </summary>
    public TimerView ListView { get; init; } = new();
    /// <summary>
    /// 滚动条
    /// </summary>
    public SUIScrollbar2 ScrollBar { get; init; } = new();

    /// <param name="scrollOrientation">滚动朝向</param>
    /// <param name="fixedSize">固定大小</param>
    public SUIScrollView2(Orientation scrollOrientation, bool fixedSize = true)
    {
        #region Mask ListView 蒙版 列表
        ScrollOrientation = scrollOrientation;
        FixedSize = fixedSize;

        DragIgnore = MaskView.DragIgnore = ListView.DragIgnore = true;

        // MaskView.BgColor = Color.White * 0.5f; // 测试用
        MaskView.OverflowHidden = true;
        MaskView.SetSizePercent(1f);
        MaskView.JoinParent(this);

        if (ScrollOrientation is Orientation.Vertical)
        {
            ListView.Width.Percent = 1f;
            ListView.IsAdaptiveHeight = true;
        }
        else if (ScrollOrientation is Orientation.Horizontal)
        {
            ListView.IsAdaptiveWidth = true;
            ListView.Height.Percent = 1f;
        }

        ListView.HideFullyOverflowedElements = true;
        ListView.SetPadding(1f);
        ListView.JoinParent(MaskView);
        #endregion

        #region Scrollbar 滚动条
        ScrollBar.Spacing = new Vector2(4f);
        ScrollBar.Rounded = new Vector4(4f);
        ScrollBar.BgColor = Color.Black * 0.25f;
        ScrollBar.BorderColor = Color.Transparent;

        switch (ScrollOrientation)
        {
            case Orientation.Horizontal:
                ScrollBar.SetSize(0f, 8f, 1f, 0f);
                ScrollBar.RelativeMode = RelativeMode.Vertical;

                ScrollBar.OnUpdate += (_) =>
                {
                    var maskSize = new Vector2(MaskView.GetInnerDimensions().Width, 1f);
                    var targetSize = new Vector2(ListView.Children.Any() ? ListView.Width.Pixels : 0, 1f);

                    ScrollBar.SetSizeForMaskAndTarget(maskSize, targetSize);
                };
                break;
            case Orientation.Vertical or _:
                ScrollBar.SetSize(8f, 0f, 0f, 1f);
                ScrollBar.RelativeMode = RelativeMode.Horizontal;

                ScrollBar.OnUpdate += (_) =>
                {
                    var maskSize = new Vector2(1f, MaskView.GetInnerDimensions().Height);
                    var targetSize = new Vector2(1f, ListView.Children.Any() ? ListView.Height.Pixels : 0);

                    ScrollBar.SetSizeForMaskAndTarget(maskSize, targetSize);
                };
                break;
        }

        ScrollBar.JoinParent(this);
        #endregion
    }

    static FieldInfo scrollWhellValueField;
    public override void ScrollWheel(UIScrollWheelEvent evt)
    {

#if false
        Main.NewText("123");
        MaskView.BgColor = Color.Red * 0.25f;
        ListView.BgColor = Color.Black * 0.25f;
#endif

        float delta = 0;

        var maxRange = ScrollBar.GetScrollRange();
        switch (ScrollOrientation)
        {
            case Orientation.Horizontal:
                float origX = ScrollBar.TargetScrollPosition.X;
                ScrollBar.TargetScrollPosition -= new Vector2(evt.ScrollWheelValue, 0f);
                delta = ScrollBar.TargetScrollPosition.X - origX;
                if (Math.Abs(delta) > 0 && (ScrollBar.TargetScrollPosition.X == 0 || ScrollBar.TargetScrollPosition.X == maxRange.X))
                    delta = -evt.ScrollWheelValue;
                break;
            case Orientation.Vertical or _:
                float origY = ScrollBar.TargetScrollPosition.Y;
                ScrollBar.TargetScrollPosition -= new Vector2(0f, evt.ScrollWheelValue);
                delta = ScrollBar.TargetScrollPosition.Y - origY;
                if (Math.Abs(delta) > 0 && (ScrollBar.TargetScrollPosition.Y == 0 || ScrollBar.TargetScrollPosition.Y == maxRange.Y))
                    delta = -evt.ScrollWheelValue;
                break;
        }

        scrollWhellValueField ??= typeof(UIScrollWheelEvent).GetField("ScrollWheelValue", BindingFlags.Public | BindingFlags.Instance);
        scrollWhellValueField.SetValue(evt, evt.ScrollWheelValue + (int)delta);

        base.ScrollWheel(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        WidthTimer.UpdateHighFps();
        HeightTimer.UpdateHighFps();

        base.Draw(spriteBatch);

        RecalculatePosition();

        //var d = GetDimensions();
        //SDFGraphics.HasBorderBox(d.Position(), default, d.Size(), Color.Purple * .5f, 4f, Main.DiscoColor, GetMatrix(true));
        //spriteBatch.DrawString(FontAssets.MouseText.Value, "芝士中心", d.Center(), Main.DiscoColor * .5f);
    }

    private void RecalculatePosition()
    {
        bool recalculate = false;

        switch (ScrollOrientation)
        {
            case Orientation.Horizontal:
                // 滚动条位置对准
                if (ListView.Left.Pixels != -ScrollBar.CurrentScrollPosition.X)
                {
                    ListView.Left.Pixels = -ScrollBar.CurrentScrollPosition.X;
                    recalculate = true;
                }
                break;
            case Orientation.Vertical or _:
                // 滚动条位置对准
                if (ListView.Top.Pixels != -ScrollBar.CurrentScrollPosition.Y)
                {
                    ListView.Top.Pixels = -ScrollBar.CurrentScrollPosition.Y;
                    recalculate = true;
                }
                break;
        }

        if (FixedSize)
        {
            switch (ScrollOrientation)
            {
                case Orientation.Horizontal:
                    // 设置正确的 蒙版 高度
                    if (ScrollBar.IsBeUsableH)
                    {
                        if (MaskView.Height.Pixels != -(ScrollBar.Height.Pixels + ScrollBar.Spacing.Y))
                        {
                            MaskView.Height.Pixels = -ScrollBar.Height.Pixels - ScrollBar.Spacing.Y;
                            recalculate = true;
                        }
                    }
                    else if (MaskView.Height.Pixels != 0)
                    {
                        MaskView.Height.Pixels = 0f;
                        recalculate = true;
                    }
                    break;
                case Orientation.Vertical or _:
                    // 设置正确的 蒙版 宽度
                    if (ScrollBar.IsBeUsableV)
                    {
                        if (MaskView.Width.Pixels != -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X))
                        {
                            WidthTimer.Open();

                            MaskView.Width.Pixels =
                                WidthTimer.Lerp(0, -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X));
                            recalculate = true;
                        }
                    }
                    else if (MaskView.Width.Pixels != 0)
                    {
                        WidthTimer.Close();

                        MaskView.Width.Pixels =
                            WidthTimer.Lerp(0, -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X));
                        recalculate = true;
                    }
                    break;
            }
        }

        if (recalculate)
            Recalculate();
    }

    public AnimationTimer WidthTimer = new AnimationTimer(3);
    public AnimationTimer HeightTimer = new AnimationTimer(3);
}
