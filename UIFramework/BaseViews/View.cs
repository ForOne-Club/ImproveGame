using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UIFramework.BaseViews;

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
    /// 元素大小会根据子元素位置大小变化，自身的 <see cref="UIElement.Width"/> 属性会在每次统计子元素后变化 <br/>
    /// 同时会使 <see cref="UIElement.MaxWidth"/> 强制等于统计子元素后 <see cref="UIElement.Width"/> <br/>
    /// 使用的时候要注意，其子元素只有继承自 <see cref="View"/> 才会被统计 <br/>
    /// 如果父元素开启此属性 元素自身的 <see cref="UIElement.MaxWidth"/> 会被强制设为 <see cref="float.MaxValue"/>
    /// </summary>
    public bool IsAdaptiveWidth;

    /// <summary>
    /// 元素大小会根据子元素位置大小变化，自身的 <see cref="UIElement.Height"/> 属性会在每次统计子元素后变化 <br/>
    /// 同时会使 <see cref="UIElement.MaxHeight"/> 强制等于统计子元素后 <see cref="UIElement.Height"/> <br/>
    /// 使用的时候要注意，其子元素只有继承自 <see cref="View"/> 才会被统计 <br/>
    /// 如果父元素开启此属性 元素自身的 <see cref="UIElement.MaxHeight"/> 会被强制设为 <see cref="float.MaxValue"/>
    /// </summary>
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
    /// 防止溢出 (越界换行)，与 <see cref="RelativeMode"/> 搭配使用 <br/>
    /// 只有设置了 <see cref="StyleDimension.Pixels"/> 属性才有效
    /// </summary>
    public bool PreventOverflow;

    /// <summary>
    /// 直接换行, 不考虑 <see cref="PreventOverflow"/>
    /// </summary>
    public bool DirectLineBreak;

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

    public bool IsLeftMousePressed;

    public bool IsRightMousePressed;

    public Vector4 Rounded;
    public float Border;
    public Color BgColor, BorderColor;

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
            Vector2 maxPos = minPos;

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
                Width.Percent = 0f;
                Width.Pixels = maxPos.X - minPos.X + HPadding;
                MaxWidth = Width;
            }

            if (IsAdaptiveHeight)
            {
                Height.Percent = 0f;
                Height.Pixels = maxPos.Y - minPos.Y + VPadding;
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

        View parent = Parent as View;

        if (Parent is UIList || (parent != null && parent.IsAdaptiveWidth))
        {
            MaxWidth = new StyleDimension(float.MaxValue, 0f);
        }

        if (Parent is UIList || (parent != null && parent.IsAdaptiveHeight))
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
        if (RelativeMode is RelativeMode.Horizontal or RelativeMode.Vertical)
        {
            // 这边VAlign和HAlign相比局长写的反过来了，暂没有发现问题，先保持这样
            if (RelativeMode is RelativeMode.Vertical)
            {
                VAlign = 0f;
            }

            if (RelativeMode is RelativeMode.Horizontal)
            {
                HAlign = 0f;
            }

            Left = Top = new StyleDimension();

            if (Parent is View parent && parent.Children is IList<UIElement> parentChildren &&
                parentChildren.IndexOf(this) is int index && index >= 1)
            {
                View previousView = null;

                for (int i = index - 1; i >= 0; i--)
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

                            if (DirectLineBreak || PreventOverflow && RightPixels > parentInnerSize.X)
                            {
                                SetPosPixels(0f, previousView.Top.Pixels + previousViewOuterSize.Y + Spacing.Y);
                            }

                            break;
                        case RelativeMode.Vertical:
                            SetPosPixels(
                                ResetAnotherPosition ? 0 : previousView.Left.Pixels,
                                previousView.Top.Pixels + previousViewOuterSize.Y + Spacing.Y);

                            if (DirectLineBreak || PreventOverflow && BottomPixels > parentInnerSize.Y)
                            {
                                SetPosPixels(previousView.Left.Pixels + previousViewOuterSize.X + Spacing.X, 0f);
                            }

                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 禁用可能的文本编辑操作，一般在按下除编辑框以外的UI元素时执行
    /// </summary>
    public void DisablePossibleTextEditing()
    {
        if (this is not SUIEditableText && UISystem.FocusedEditableText is {IsWritingText: true} && !UISystem.FocusedEditableText.IsMouseHovering)
        {
            UISystem.FocusedEditableText.ToggleTakingText();
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
        // 文本框处理
        DisablePossibleTextEditing();

        IsLeftMousePressed = true;

        base.LeftMouseDown(evt);
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        // 文本框处理
        DisablePossibleTextEditing();

        IsRightMousePressed = true;

        base.RightMouseDown(evt);
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

    /// <summary>
    /// 其他一切绘制都结束之后再绘制边框
    /// </summary>
    public bool FinallyDrawBorder;

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        DrawSDFRectangle();
        base.DrawSelf(spriteBatch);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (FinallyDrawBorder && Border > 0f && BorderColor != Color.Transparent)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            SDFRectangle.HasBorder(pos, size, Rounded, Color.Transparent, Border, BorderColor);
        }
    }

    public void DrawSDFRectangle()
    {
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();

        if (Border > 0)
        {
            if (BorderColor == Color.Transparent || FinallyDrawBorder)
            {
                if (BgColor != Color.Transparent)
                    SDFRectangle.NoBorder(pos + new Vector2(Border), size - new Vector2(Border * 2f),
                        Rounded - new Vector4(Border), BgColor);
            }
            else
            {
                SDFRectangle.HasBorder(pos, size, Rounded, BgColor, Border, BorderColor);
            }
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(pos, size, Rounded, BgColor);
        }
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

    public View SetSizePercent(float percent)
    {
        Width.Percent = percent;
        Height.Percent = percent;
        return this;
    }

    public View SetSizePercent(float width, float height)
    {
        Width.Percent = width;
        Height.Percent = height;
        return this;
    }

    public View SetSizePixels(Vector2 size)
    {
        Width.Pixels = size.X;
        Height.Pixels = size.Y;
        return this;
    }

    /// <summary>
    /// <see cref="UIElement.SetPadding(float)"/>
    /// </summary>
    public float Padding
    {
        set => SetPadding(value);
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

    public Vector2 GetOuterDimensionsSize()
    {
        return new Vector2(_outerDimensions.Width, _outerDimensions.Height);
    }

    public Vector2 GetInnerDimensionsRight()
    {
        return new Vector2(_innerDimensions.Width + _innerDimensions.X, _innerDimensions.Height + _innerDimensions.Y);
    }

    public Vector2 GetInnerDimensionsSize()
    {
        return new Vector2(_innerDimensions.Width, _innerDimensions.Height);
    }

    public Vector2 GetDimensionsRight()
    {
        return new Vector2(_dimensions.Width + _dimensions.X, _dimensions.Height + _dimensions.Y);
    }

    public Vector2 GetDimensionsSize()
    {
        return new Vector2(_dimensions.Width, _dimensions.Height);
    }

    public Vector2 GetDimensionsCenter()
    {
        return new Vector2(_dimensions.X + _dimensions.Width / 2f, _dimensions.Y + _dimensions.Height / 2f);
    }

    #endregion

    #endregion
}