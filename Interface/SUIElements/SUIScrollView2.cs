using Microsoft.Xna.Framework.Input;

namespace ImproveGame.Interface.SUIElements;

public enum ScrollType { TwoWay, Horizontal, Vertical }

public enum Direction { Left, Top, Right, Bottom }

public class SUIScrollView2 : TimerView
{
    public readonly ScrollType ScrollType;

    public Vector2 ContentAndScrollbarSpacing = new(4f);

    public View OverflowHiddenView { get; init; } = new();
    public TimerView AdaptiveView { get; init; } = new();
    public SUIScrollbar2 HScrollBar { get; init; } = new();
    public SUIScrollbar2 VScrollBar { get; init; } = new();

    public SUIScrollView2(ScrollType scrollType)
    {
        ScrollType = scrollType;

        OverflowHiddenView.OverflowHidden = true;
        OverflowHiddenView.Width.Percent = OverflowHiddenView.Height.Percent = 1f;
        OverflowHiddenView.JoinParent(this);

        DragIgnore = true;
        OverflowHiddenView.DragIgnore = true;
        AdaptiveView.DragIgnore = true;

        if (ScrollType is ScrollType.TwoWay or ScrollType.Horizontal)
        {
            AdaptiveView.IsAdaptiveWidth = true;
            AdaptiveView.Height.Percent = 1f;
        }

        if (ScrollType is ScrollType.TwoWay or ScrollType.Vertical)
        {
            AdaptiveView.Width.Percent = 1f;
            AdaptiveView.IsAdaptiveHeight = true;
        }

        AdaptiveView.HideFullyOverflowedElements = true;
        AdaptiveView.SetPadding(1f);
        AdaptiveView.JoinParent(OverflowHiddenView);


        if (ScrollType is ScrollType.TwoWay or ScrollType.Horizontal)
        {
            HScrollBar.Width.Percent = 1f;
            HScrollBar.Height.Pixels = 8f;
            HScrollBar.VAlign = 1f;
            HScrollBar.BarColor = Color.Black * 0.6f;
            HScrollBar.BarHoverColor = Color.Black * 0.7f;
            HScrollBar.BgColor = Color.Transparent;
            HScrollBar.BorderColor = Color.Transparent;
            HScrollBar.OnUpdate += (_) =>
            {
                Vector2 windowSize = new Vector2(OverflowHiddenView.GetInnerDimensions().Width, 1f);
                Vector2 contentSize = new Vector2(AdaptiveView.Children.Any() ? AdaptiveView.Width.Pixels : 0, 1f);
                HScrollBar.SetWindowAndContentSize(windowSize, contentSize);
            };
            HScrollBar.JoinParent(this);

            OverflowHiddenView.Height.Pixels = -ContentAndScrollbarSpacing.Y - HScrollBar.Height.Pixels;
        }

        if (ScrollType is ScrollType.TwoWay or ScrollType.Vertical)
        {
            VScrollBar.Width.Pixels = 8f;
            VScrollBar.Height.Percent = 1f;
            VScrollBar.HAlign = 1f;

            VScrollBar.BarColor = Color.Black * 0.6f;
            VScrollBar.BarHoverColor = Color.Black * 0.7f;
            VScrollBar.BgColor = Color.Transparent;
            VScrollBar.BorderColor = Color.Transparent;
            VScrollBar.OnUpdate += (_) =>
            {
                Vector2 windowSize = new Vector2(1f, OverflowHiddenView.GetInnerDimensions().Height);
                Vector2 contentSize = new Vector2(1f, AdaptiveView.Children.Any() ? AdaptiveView.Height.Pixels : 0);
                VScrollBar.SetWindowAndContentSize(windowSize, contentSize);
            };
            VScrollBar.JoinParent(this);

            OverflowHiddenView.Width.Pixels = -ContentAndScrollbarSpacing.X - VScrollBar.Width.Pixels;
        }
    }

    public void MoveScrollbar(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                OverflowHiddenView.HAlign = 1f;
                VScrollBar.HAlign = 0f;
                break;
            case Direction.Top:
                OverflowHiddenView.VAlign = 1f;
                HScrollBar.VAlign = 0f;
                break;
            case Direction.Right:
                OverflowHiddenView.HAlign = 0f;
                VScrollBar.HAlign = 1f;
                break;
            case Direction.Bottom:
                OverflowHiddenView.VAlign = 0f;
                HScrollBar.VAlign = 1f;
                break;
        }
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        KeyboardState state = Keyboard.GetState();

        switch (ScrollType)
        {
            case ScrollType.TwoWay:
                if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
                {
                    HScrollBar.TargetScrollPosition -= new Vector2(evt.ScrollWheelValue, 0f);
                }
                else
                {
                    VScrollBar.TargetScrollPosition -= new Vector2(0f, evt.ScrollWheelValue);
                }
                break;
            case ScrollType.Horizontal:
                HScrollBar.TargetScrollPosition -= new Vector2(evt.ScrollWheelValue, 0f);
                break;
            case ScrollType.Vertical:
                VScrollBar.TargetScrollPosition -= new Vector2(0f, evt.ScrollWheelValue);
                break;
            default:
                break;
        }

        base.ScrollWheel(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        bool recalculate = false;

        if (ScrollType is ScrollType.TwoWay or ScrollType.Horizontal)
        {
            if (AdaptiveView.Left.Pixels != -HScrollBar.CurrentScrollPosition.X)
            {
                AdaptiveView.Left.Pixels = -HScrollBar.CurrentScrollPosition.X;
                recalculate = true;
            }

            if (HScrollBar.IsBeUsableH)
            {
                if (OverflowHiddenView.Height.Pixels != -ContentAndScrollbarSpacing.Y - HScrollBar.Height.Pixels)
                {
                    OverflowHiddenView.Height.Pixels = -ContentAndScrollbarSpacing.Y - HScrollBar.Height.Pixels;
                    recalculate = true;
                }
            }
            else if (OverflowHiddenView.Height.Pixels != 0f)
            {
                OverflowHiddenView.Height.Pixels = 0f;
                recalculate = true;
            }
        }

        if (ScrollType is ScrollType.TwoWay or ScrollType.Vertical)
        {
            if (AdaptiveView.Top.Pixels != -VScrollBar.CurrentScrollPosition.Y)
            {
                AdaptiveView.Top.Pixels = -VScrollBar.CurrentScrollPosition.Y;
                recalculate = true;
            }

            if (VScrollBar.IsBeUsableV)
            {
                if (OverflowHiddenView.Width.Pixels != -ContentAndScrollbarSpacing.X - VScrollBar.Width.Pixels)
                {
                    OverflowHiddenView.Width.Pixels = -ContentAndScrollbarSpacing.X - VScrollBar.Width.Pixels;
                    recalculate = true;
                }
            }
            else if (OverflowHiddenView.Width.Pixels != 0f)
            {
                OverflowHiddenView.Width.Pixels = 0f;
                recalculate = true;
            }
        }

        if (recalculate)
        {
            Recalculate();
        }
    }
}
