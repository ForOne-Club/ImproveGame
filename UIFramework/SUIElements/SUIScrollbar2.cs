using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.UIFramework.SUIElements;

public class SUIScrollbar2 : TimerView
{
    #region Basic Fields
    public virtual Vector2 CurrentScrollPosition
    {
        get => Vector2.Clamp(_currentScrollPosition, Vector2.Zero, GetScrollRange());
        set => _currentScrollPosition = Vector2.Clamp(value, Vector2.Zero, GetScrollRange());
    }
    protected Vector2 _currentScrollPosition;

    public Vector2 TargetScrollPosition
    {
        get => Vector2.Clamp(_targetScrollPosition, Vector2.Zero, GetScrollRange());
        set => _targetScrollPosition = Vector2.Clamp(value, Vector2.Zero, GetScrollRange());
    }
    public Vector2 _targetScrollPosition;

    public Vector2 OriginalScrollPosition;
    public Vector2 PreviousTargetScrollPosition;

    public Color BarColor = Color.Black * 0.6f;
    public Color BarHoverColor = Color.Black * 0.7f;

    public bool IsBarSizeLimited = true;

    public Vector2 ScrollViewSize
    {
        get => Vector2.Max(Vector2.One, _scrollViewSize);
        set => _scrollViewSize = Vector2.Max(Vector2.One, value);
    }
    public Vector2 _scrollViewSize = Vector2.One;

    public Vector2 ScrollableContentSize
    {
        get => Vector2.Max(Vector2.One, _scrollableContentSize);
        set => _scrollableContentSize = Vector2.Max(Vector2.One, value);
    }
    public Vector2 _scrollableContentSize = Vector2.One;

    public bool AutomaticallyDisabled = true;
    public bool IsBeUsableH => !AutomaticallyDisabled || ScrollableContentSize.X > ScrollViewSize.X;
    public bool IsBeUsableV => !AutomaticallyDisabled || ScrollableContentSize.Y > ScrollViewSize.Y;
    #endregion

    #region Base Method

    public void SetWindowAndContentSize(Vector2 windowSize, Vector2 contentSize)
    {
        ScrollViewSize = windowSize;
        ScrollableContentSize = contentSize;
    }

    public Vector2 GetScrollRange() => Vector2.Max(Vector2.Zero, ScrollableContentSize - ScrollViewSize);

    public Vector2 BarPosition => CurrentScrollPosition / ScrollableContentSize * GetInnerDimensionsSize();

    public Vector2 GetBarSize()
    {
        float innerWidth = GetInnerDimensions().Width;
        float innerHeight = GetInnerDimensions().Height;

        Vector2 barSize = GetInnerDimensionsSize() * (ScrollViewSize / ScrollableContentSize);

        if (IsBarSizeLimited)
        {
            Vector2 min = new Vector2(Math.Min(innerWidth, innerHeight));
            barSize = Vector2.Clamp(barSize, min, GetInnerDimensionsSize());
        }

        return barSize;
    }

    public Vector2 GetBarScreenPosition() => GetInnerDimensions().Position() + BarPosition;

    public void SetScrollPositionDirectly(Vector2 position)
    {
        position = Vector2.Clamp(position, Vector2.Zero, GetScrollRange());

        CurrentScrollPosition = position;
        OriginalScrollPosition = position;
        TargetScrollPosition = position;
        PreviousTargetScrollPosition = position;
    }

    public void SetBarPositionDirectly(Vector2 barPosition) =>
        SetScrollPositionDirectly(ScrollableContentSize * barPosition / GetInnerDimensionsSize());

    public bool IsMouseOverScrollbar()
    {
        Vector2 focus = Main.MouseScreen;
        Vector2 barPos = GetBarScreenPosition();
        Vector2 barSize = GetBarSize();

        return focus.X > barPos.X && focus.Y > barPos.Y && focus.X < barPos.X + barSize.X && focus.Y < barPos.Y + barSize.Y;
    }
    #endregion

    #region private static Fields
    protected bool _isScrollbarDragging; // 滚动条拖动中
    protected Vector2 _scrollbarDragOffset; // 滚动条拖动偏移

    protected readonly AnimationTimer ScrollTimer = new(5); // 动画
    #endregion

    #region Override Method
    public override bool ContainsPoint(Vector2 point)
    {
        return (IsBeUsableH || IsBeUsableV) && base.ContainsPoint(point);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if ((IsBeUsableH || IsBeUsableV) && IsMouseOverScrollbar())
        {
            _isScrollbarDragging = true;
            _scrollbarDragOffset = Main.MouseScreen - GetBarScreenPosition();
        }
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);

        _isScrollbarDragging = false;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsBeUsableH || IsBeUsableV)
        {
            if (_isScrollbarDragging)
            {
                SetBarPositionDirectly(Main.MouseScreen - _scrollbarDragOffset - GetInnerDimensions().Position());
            }

            UpdateScrollPosition();

            ScrollTimer.Update();

            base.DrawSelf(spriteBatch);

            DrawScrollbar();
        }
    }
    #endregion

    protected virtual void UpdateScrollPosition()
    {
        if (PreviousTargetScrollPosition != TargetScrollPosition)
        {
            PreviousTargetScrollPosition = TargetScrollPosition;
            OriginalScrollPosition = CurrentScrollPosition;
            ScrollTimer.OpenAndResetTimer();
        }

        if (CurrentScrollPosition != TargetScrollPosition)
        {
            CurrentScrollPosition = ScrollTimer.Lerp(OriginalScrollPosition, TargetScrollPosition);
        }
    }

    protected virtual void DrawScrollbar()
    {
        Vector2 barSize = GetBarSize();
        Vector2 barPos = GetInnerDimensions().Position() + BarPosition;

        if (barSize.X > 0 && barSize.Y > 0)
        {
            Color barBgColor = IsMouseOverScrollbar() || _isScrollbarDragging ? BarHoverColor : BarColor;
            SDFRectangle.NoBorder(barPos, barSize, new Vector4(Math.Min(barSize.X, barSize.Y) / 2f), barBgColor * Opacity.Value);
        }
    }
}
