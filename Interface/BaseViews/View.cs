using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.BaseViews;

public enum RelativeMode { None, Horizontal, Vertical };

/// <summary>
/// 相对定位，用于可变大小的 UI 更方便计算位置。
/// </summary>
public class View : UIElement
{
    /// <summary>
    /// 用于记录先前的子元素数量
    /// </summary>
    public int PreviousChildCount { get; protected set; }

    /// <summary>
    /// 子元素相较于先前是否有变化
    /// </summary>
    public bool HasChildCountChanges => Children.Count() != PreviousChildCount;

    /// <summary>
    /// 该类的大小会根据子元素位置大小自适应，其 <see cref="UIElement.Width"/> <see cref="UIElement.Height"/> 属性皆不生效 <br/>
    /// 使用的时候要注意，只有继承自 <see cref="View"/> 的子元素才有效 <br/>
    /// 并且需要注意子元素的 <see cref="UIElement.Width"/> <see cref="UIElement.Height"/> Percent 属性不要设置，因为获取到的会是无限大
    /// </summary>
    public bool IsAdaptiveWidth;
    public bool IsAdaptiveHeight;

    /// <summary>
    /// 隐藏完全溢出元素
    /// </summary>
    public bool HideFullyOverflowedElements;

    /// <summary>
    /// 使自身相对前一个 <see cref="View"/> 排列，横向或纵向 <br/>
    /// 启用会使 <see cref="UIElement.Left"/> <see cref="UIElement.Top"/>
    /// <see cref="UIElement.HAlign"/> <see cref="UIElement.VAlign"/> 失效
    /// </summary>
    public RelativeMode RelativeMode;

    /// <summary>
    /// 间距，与 <see cref="RelativeMode"/> 搭配使用
    /// </summary>
    public Vector2 Spacing;

    /// <summary>
    /// 防止溢出 (越界换行)，与 <see cref="RelativeMode"/> 搭配使用
    /// </summary>
    public bool PreventOverflow;

    /// <summary>
    /// 设置 true 横向时不同步与前一个元素的 Top，纵向时不同步 Left<br/>
    /// 在大背包中用于一排 Button 的时候，第一个 Button 前面有一个 Switch<br/>
    /// 与 <see cref="RelativeMode"/> 搭配使用
    /// </summary>
    public bool ResetAnotherPosition;

    /// <summary>
    /// 拖动忽略，默认为 <see langword="false"/> 不会影响长辈中可拖动元素拖动
    /// </summary>
    public bool DragIgnore;

    /// <summary>
    /// 不透明度 [0,1]
    /// </summary>
    public readonly Opacity Opacity;

    public bool IsLeftMousePressed;

    public Vector4 Rounded;
    public float Border;
    public Color BgColor, BorderColor;

    public View()
    {
        Opacity = new Opacity(this);
    }

    /// <summary>
    /// 设置圆角矩形的基本属性
    /// </summary>
    public void SetRoundedRectProperties(Color bgColor, float border, Color borderColor, Vector4 rounded)
    {
        BgColor = bgColor;
        Border = border;
        BorderColor = borderColor;
        Rounded = rounded;
    }

    /// <summary>
    /// 设置圆角矩形的基本属性
    /// </summary>
    public void SetRoundedRectProperties(Color bgColor, float border, Color borderColor, float rounded)
    {
        BgColor = bgColor;
        Border = border;
        BorderColor = borderColor;
        Rounded = new Vector4(rounded);
    }

    #region Override Method
    public override void Recalculate()
    {
        RecalculateFromView();
        OrigianlRecalculate();

        RecalculateChildren();

        if (IsAdaptiveWidth || IsAdaptiveHeight)
        {
            Vector2 minPos = GetInnerDimensions().Position();
            Vector2 maxPos = GetInnerDimensions().Position();

            foreach (UIElement child in Children)
            {
                CalculatedStyle dimension = child.GetDimensions();

                if (IsAdaptiveWidth)
                {
                    maxPos.X = Math.Max(maxPos.X, dimension.X + dimension.Width);
                }

                if (IsAdaptiveHeight)
                {
                    maxPos.Y = Math.Max(maxPos.Y, dimension.Y + dimension.Height);
                }
            }

            if (IsAdaptiveWidth)
            {
                Width = default;
                Width.Pixels = MathF.Round(maxPos.X - minPos.X, 2) + HPadding;
                MaxWidth = Width;
            }

            if (IsAdaptiveHeight)
            {
                Height = default;
                Height.Pixels = MathF.Round(maxPos.Y - minPos.Y, 2) + VPadding;
                MaxHeight = Height;
            }

            RecalculateFromView();
            OrigianlRecalculate();

            if (HAlign != 0 || VAlign != 0)
            {
                RecalculateChildren();
            }
        }
    }

    public virtual void OrigianlRecalculate()
    {
        CalculatedStyle parentDimensions =
            Parent?.GetInnerDimensions() ?? UserInterface.ActiveInstance.GetDimensions();

        View view = Parent as View;

        if (Parent is UIList || (view?.IsAdaptiveWidth ?? false))
        {
            MaxWidth = new StyleDimension(float.MaxValue, 0f);
        }

        if (Parent is UIList || (view?.IsAdaptiveHeight ?? false))
        {
            MaxHeight = new StyleDimension(float.MaxValue, 0f);
        }

        _outerDimensions = GetDimensionsBasedOnParentDimensions(parentDimensions);

        _dimensions = _outerDimensions;
        _dimensions.X += MarginLeft;
        _dimensions.Y += MarginTop;
        _dimensions.Width -= MarginLeft + MarginRight;
        _dimensions.Height -= MarginTop + MarginBottom;

        _innerDimensions = _dimensions;
        _innerDimensions.X += PaddingLeft;
        _innerDimensions.Y += PaddingTop;
        _innerDimensions.Width -= HPadding;
        _innerDimensions.Height -= VPadding;
    }

    public virtual void RecalculateFromView()
    {
        Opacity.Recalculate();

        if (RelativeMode is RelativeMode.Horizontal or RelativeMode.Vertical)
        {
            if (RelativeMode is RelativeMode.Vertical)
            {
                HAlign = 0f;
            }

            if (RelativeMode is RelativeMode.Horizontal)
            {
                VAlign = 0f;
            }

            Left = Top = new StyleDimension();

            if (Parent is View parent && parent.Children is List<UIElement> parentChildren &&
                parentChildren.IndexOf(this) is int index && index >= 1)
            {
                View previousView = null;

                for (int i = index - 1; i > -1; i--)
                {
                    if (parentChildren[i] is View view)
                    {
                        previousView = view;
                        break;
                    }
                }

                if (previousView != null)
                {
                    Vector2 previousViewOuterSize = previousView.GetOuterDimensionsSize();
                    Vector2 parentInnerSize = parent.GetInnerDimensionsSize();

                    switch (RelativeMode)
                    {
                        case RelativeMode.Horizontal:
                            SetPosPixels(
                                previousView.Left.Pixels + previousViewOuterSize.X + Spacing.X,
                                ResetAnotherPosition ? 0 : previousView.Top.Pixels);

                            if (PreventOverflow && RightPixels > parentInnerSize.X)
                            {
                                SetPosPixels(0f, previousView.Top.Pixels + previousViewOuterSize.Y + Spacing.Y);
                            }
                            break;
                        case RelativeMode.Vertical:
                            SetPosPixels(
                                ResetAnotherPosition ? 0 : previousView.Left.Pixels,
                                previousView.Top.Pixels + previousViewOuterSize.Y + Spacing.Y);

                            if (PreventOverflow && BottomPixels > parentInnerSize.Y)
                            {
                                SetPosPixels(previousView.Left.Pixels + previousViewOuterSize.X + Spacing.X, 0f);
                            }
                            break;
                    }
                }
            }
        }
    }

    public virtual void PreUpdate(GameTime gameTime)
    {
        foreach (UIElement child in Children)
        {
            if (child is View view)
            {
                view.PreUpdate(gameTime);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // 记录先前的子元素数量
        PreviousChildCount = Children.Count();
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        IsLeftMousePressed = true;

        base.LeftMouseDown(evt);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        IsLeftMousePressed = false;

        base.LeftMouseUp(evt);
    }

    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        if (OverflowHidden && HideFullyOverflowedElements)
        {
            Vector2 pos = Parent.GetDimensions().Position();
            Vector2 size = Parent.GetDimensions().Size();
            foreach (UIElement uie in from uie in Elements
                                      let dimensions2 = uie.GetDimensions()
                                      let position2 = dimensions2.Position()
                                      let size2 = dimensions2.Size()
                                      where Collision.CheckAABBvAABBCollision(pos, size, position2, size2)
                                      select uie)
            {
                uie.Draw(spriteBatch);
            }
        }
        else
        {
            foreach (UIElement element in Elements)
            {
                element.Draw(spriteBatch);
            }
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();

        if (Border > 0 && (BgColor != Color.Transparent || BorderColor != Color.Transparent))
        {
            SDFRectangle.HasBorder(pos, size, Rounded, BgColor * Opacity.Value, Border, BorderColor * Opacity.Value);
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(pos, size, Rounded, BgColor * Opacity.Value);
        }

        base.DrawSelf(spriteBatch);
    }
    #endregion

    #region 拓展一些 UIElement 原来没有的方法
    public void JoinParent(UIElement parent)
    {
        parent.Append(this);
    }

    public View SetPosPixels(float left, float top)
    {
        Left.Pixels = left;
        Top.Pixels = top;
        return this;
    }

    public View SetPosPixels(Vector2 size)
    {
        Left.Pixels = size.X;
        Top.Pixels = size.Y;
        return this;
    }

    public View SetMaxInnerPixels(float width, float height)
    {
        MaxWidth.Pixels = width + HPadding;
        MaxHeight.Pixels = height + VPadding;
        return this;
    }

    public View SetMaxInnerPixels(Vector2 size)
    {
        MaxWidth.Pixels = size.X + HPadding;
        MaxHeight.Pixels = size.Y + VPadding;
        return this;
    }

    public View SetInnerPixels(float width, float height)
    {
        Width.Pixels = width + HPadding;
        Height.Pixels = height + VPadding;
        return this;
    }

    public View SetInnerPixels(Vector2 size)
    {
        Width.Pixels = size.X + HPadding;
        Height.Pixels = size.Y + VPadding;
        return this;
    }

    public View SetMaxSizePixels(float width, float height)
    {
        MaxWidth.Pixels = width;
        MaxWidth.Pixels = height;
        return this;
    }

    public View SetMaxSizePixels(Vector2 size)
    {
        MaxWidth.Pixels = size.X;
        MaxHeight.Pixels = size.Y;
        return this;
    }

    public View SetSizePixels(float width, float height)
    {
        Width.Pixels = width;
        Height.Pixels = height;
        return this;
    }

    public View SetSizePixels(Vector2 size)
    {
        Width.Pixels = size.X;
        Height.Pixels = size.Y;
        return this;
    }

    public View SetPadding(float left, float top, float right, float bottom)
    {
        PaddingLeft = left;
        PaddingTop = top;
        PaddingRight = right;
        PaddingBottom = bottom;
        return this;
    }

    public View SetPadding(float h, float v)
    {
        PaddingLeft = PaddingRight = h;
        PaddingTop = PaddingBottom = v;
        return this;
    }

    #region 获取一些属性
    public Vector2 PositionPixels => new Vector2(Left.Pixels, Top.Pixels);

    public float HMargin => MarginLeft + MarginRight;
    public float VMargin => MarginTop + MarginBottom;

    public float HPadding => PaddingLeft + PaddingRight;
    public float VPadding => PaddingTop + PaddingBottom;

    public float RightPixels => Left.Pixels + Width.Pixels + HMargin;
    public float BottomPixels => Top.Pixels + Height.Pixels + VMargin;

    public Vector2 GetOuterDimensionsRight()
    {
        return new Vector2(_outerDimensions.Width + _outerDimensions.X, _outerDimensions.Height + _outerDimensions.Y);
    }

    public Vector2 GetDimensionsRight()
    {
        return new Vector2(_dimensions.Width + _dimensions.X, _dimensions.Height + _dimensions.Y);
    }

    public Vector2 GetInnerDimensionsRight()
    {
        return new Vector2(_innerDimensions.Width + _innerDimensions.X, _innerDimensions.Height + _innerDimensions.Y);
    }

    public Vector2 GetOuterDimensionsSize()
    {
        return new Vector2(_outerDimensions.Width, _outerDimensions.Height);
    }

    public Vector2 GetDimensionsSize()
    {
        return new Vector2(_dimensions.Width, _dimensions.Height);
    }

    public Vector2 GetInnerDimensionsSize()
    {
        return new Vector2(_innerDimensions.Width, _innerDimensions.Height);
    }
    #endregion
    #endregion
}
