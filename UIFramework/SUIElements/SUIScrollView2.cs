using ImproveGame.UIFramework.BaseViews;

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
        ScrollBar.BgColor = Color.Transparent;
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

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        switch (ScrollOrientation)
        {
            case Orientation.Horizontal:
                ScrollBar.TargetScrollPosition -= new Vector2(evt.ScrollWheelValue, 0f);
                break;
            case Orientation.Vertical or _:
                ScrollBar.TargetScrollPosition -= new Vector2(0f, evt.ScrollWheelValue);
                break;
        }

        base.ScrollWheel(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        WidthTimer.UpdateHighFps();
        HeightTimer.UpdateHighFps();

        base.Draw(spriteBatch);
    }

    public AnimationTimer WidthTimer = new AnimationTimer(3);
    public AnimationTimer HeightTimer = new AnimationTimer(3);
}
