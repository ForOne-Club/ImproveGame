using ImproveGame.Common.Animations;
using ImproveGame.QolUISystem.UIEnums;
using ImproveGame.QolUISystem.UIStruct;
using System.Xml.Linq;
using UwUPnP;

namespace ImproveGame.QolUISystem.UIElements;

public class XUIElement
{
    protected static readonly GraphicsDevice GraphicsDevice = Main.graphics.GraphicsDevice;
    protected static readonly SpriteBatch SpriteBatch = Main.spriteBatch;

    public delegate void MouseEvent(List<XUIElement> targets);

    private static int _idCounter;
    public readonly int UniqueId;

    public XUIElement Parent;

    public List<XUIElement> Children { get; protected set; } = new List<XUIElement>();

    /// <summary>
    /// 计算前位置信息
    /// </summary>
    public UIPosition Position;

    /// <summary>
    /// 计算前大小信息
    /// </summary>
    public UISize Size;

    /// <summary>
    /// 内边距
    /// </summary>
    public Spacing Padding;

    /// <summary>
    /// 外边距
    /// </summary>
    public Spacing Margin;

    /// <summary>
    /// 鼠标按下的状态
    /// </summary>
    public MouseKey MouseKey;

    /// <summary>
    /// 鼠标是否悬浮与它之上
    /// </summary>
    public bool MouseHover;

    /// <summary>
    /// 计算后外矩形位置
    /// </summary>
    public RectangleF OuterRectangle { get; protected set; }

    /// <summary>
    /// 计算后中间矩形位置
    /// </summary>
    public RectangleF OwnRectangle { get; protected set; }

    /// <summary>
    /// 计算后内容矩形位置
    /// </summary>
    public RectangleF ContentRectangle { get; protected set; }

    /// <summary>
    /// 忽略鼠标交互
    /// 在 GetCursorTargets 中不会计算该元素
    /// </summary>
    public bool IgnoresMouseInteraction;

    /// <summary>
    /// 内容溢出处理方式
    /// </summary>
    public UIOverflow Overflow;

    /// <summary>
    /// 开启隐藏溢出后整体渲染颜色
    /// </summary>
    public (Color, Color) ChildColor = (Color.White, Color.Red);

    /// <summary>
    /// 阻止长辈元素拖动
    /// </summary>
    public bool PreventDragging = true;

    /// <summary>
    /// 动画计时器 - 悬浮
    /// </summary>
    public AnimationTimer HoverTimer = new AnimationTimer(3);

    public XUIElement()
    {
        UniqueId = _idCounter++;
    }

    /// <summary>
    /// 初始化时候调用
    /// </summary>
    public virtual void OnInitialize() { }

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Initialize()
    {
        OnInitialize();

        foreach (var child in Children)
        {
            child.Initialize();
        }

        Recalculate();
    }

    /// <summary>
    /// 更新
    /// </summary>
    public virtual void Update()
    {
        UpdateChilden();
    }

    public virtual void UpdateChilden()
    {
        foreach (XUIElement child in Children)
        {
            child.Update();
        }
    }

    public event MouseEvent OnCursorEnter;
    public event MouseEvent OnCursorLeave;

    /// <summary>
    /// 鼠标光标进入
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void CursorEnter(List<XUIElement> targets)
    {
        HoverTimer.Open();
        OnCursorEnter?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标光标离开
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void CursorLeave(List<XUIElement> targets)
    {
        HoverTimer.Close();
        OnCursorLeave?.Invoke(targets);
    }

    public event MouseEvent OnMouseLeftDown;
    public event MouseEvent OnMouseRightDown;
    public event MouseEvent OnMouseMiddleDown;

    /// <summary>
    /// 鼠标左键按下
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseLeftDown(List<XUIElement> targets)
    {
        OnMouseLeftDown?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标右键按下
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseRightDown(List<XUIElement> targets)
    {
        OnMouseRightDown?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标中键按下
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseMiddleDown(List<XUIElement> targets)
    {
        OnMouseMiddleDown?.Invoke(targets);
    }

    public event MouseEvent OnMouseLeftUp;
    public event MouseEvent OnMouseRightUp;
    public event MouseEvent OnMouseMiddleUp;

    /// <summary>
    /// 鼠标左键松开
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseLeftUp(List<XUIElement> targets)
    {
        OnMouseLeftUp?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标右键松开
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseRightUp(List<XUIElement> targets)
    {
        OnMouseRightUp?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标中键松开
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseMiddleUp(List<XUIElement> targets)
    {
        OnMouseMiddleUp?.Invoke(targets);
    }

    public event MouseEvent OnMouseLeftClick;
    public event MouseEvent OnMouseRightClick;
    public event MouseEvent OnMouseMiddleClick;

    /// <summary>
    /// 鼠标左键点击
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseLeftClick(List<XUIElement> targets)
    {
        OnMouseLeftClick?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标左键点击
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseRightClick(List<XUIElement> targets)
    {
        OnMouseRightClick?.Invoke(targets);
    }

    /// <summary>
    /// 鼠标左键点击
    /// </summary>
    /// <param name="gameTime"></param>
    public virtual void MouseMiddleClick(List<XUIElement> targets)
    {
        OnMouseMiddleClick?.Invoke(targets);
    }


    /// <summary>
    /// 为自己附加一个子元素
    /// </summary>
    /// <param name="child"></param>
    public void Append(XUIElement child)
    {
        if (Children.Contains(child))
        {
            return;
        }

        child.Recalculate();
        Children.Add(child);
        child.Parent = this;
    }

    /// <summary>
    /// 加入到父元素当中去
    /// </summary>
    /// <param name="parentView"></param>
    public void Join(XUIElement parentView)
    {
        if (parentView.Children.Contains(this))
        {
            return;
        }

        Recalculate();
        parentView.Children.Add(this);
        Parent = parentView;
    }

    public virtual bool Contains(Vector2 position)
    {
        return OwnRectangle.Contains(position);
    }

    public virtual List<XUIElement> GetCursorTargets(Vector2 mousePosition)
    {
        List<XUIElement> cursorTargets = new List<XUIElement>();

        if (Contains(mousePosition))
        {
            if (!IgnoresMouseInteraction)
            {
                MouseHover = true;
                cursorTargets.Insert(0, this);
            }

            if (ContentRectangle.Contains(mousePosition))
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    XUIElement child = Children[i];
                    List<XUIElement> childCursorTargets = child.GetCursorTargets(mousePosition);
                    if (!child.IgnoresMouseInteraction && childCursorTargets.Count > 0)
                    {
                        cursorTargets.InsertRange(0, childCursorTargets);
                        return cursorTargets;
                    }
                }
            }
        }

        return cursorTargets;
    }

    /// <summary>
    /// 获取父元素绘制位置，如果没有父元素就以屏幕大小设置
    /// </summary>
    /// <returns></returns>
    protected virtual RectangleF GetParentContentRectangle()
    {
        return Parent is null ?
            new RectangleF(0, 0, Main.screenWidth, Main.screenHeight) :
            Parent.ContentRectangle;
    }

    /// <summary>
    /// 重新计算大小和位置
    /// </summary>
    public virtual void Recalculate()
    {
        OuterRectangle = new RectangleF(GetParentContentRectangle(), Position, Size);

        OwnRectangle = new RectangleF(
            OuterRectangle.X + Margin.Left,
            OuterRectangle.Y + Margin.Top,
            OuterRectangle.Width - Margin.Left - Margin.Right,
            OuterRectangle.Height - Margin.Top - Margin.Bottom);

        ContentRectangle = new RectangleF(
            OwnRectangle.X + Padding.Left,
            OwnRectangle.Y + Padding.Top,
            OwnRectangle.Width - Padding.Left - Padding.Right,
            OwnRectangle.Height - Padding.Top - Padding.Bottom);

        RecalculateChildren();
    }

    public virtual void RecalculateChildren()
    {
        foreach (XUIElement child in Children)
        {
            child.Recalculate();
        }
    }

    /// <summary>
    /// 我用来更新动画或者拖拽位置的
    /// </summary>
    public virtual void PreDraw()
    {
        HoverTimer.Update();
        PreDrawChildren();
    }

    public virtual void PreDrawChildren()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].PreDraw();
        }
    }

    protected static void Begin_Immediate(SamplerState samplerState)
    {
        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    samplerState, DepthStencilState.Default,
                    RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
    }

    public virtual void Draw(DrawArgs drawArgs)
    {
        DrawArgs ownDrawArgs = drawArgs;

        switch (Overflow)
        {
            default:
            case UIOverflow.Visible:
                break;
            case UIOverflow.Hidden:
                ownDrawArgs.DrawOffset = -ContentRectangle.Position;
                break;
        }

        DrawSelf(drawArgs);

        switch (Overflow)
        {
            default:
            case UIOverflow.Visible:
                DrawChildren(ownDrawArgs);
                break;
            case UIOverflow.Hidden:
                Vector2 position = ContentRectangle.Position + drawArgs.DrawOffset;
                Vector2 size = ContentRectangle.Size;

                int left = (int)MathF.Round(position.X * Main.UIScale);
                int top = (int)MathF.Round(position.Y * Main.UIScale);
                int right = (int)MathF.Round((position.X + size.X) * Main.UIScale);
                int bottom = (int)MathF.Round((position.Y + size.Y) * Main.UIScale);

                int rt2dWidth = right - left;
                int rt2dHeight = bottom - top;

                // 借走 RT2D（记得还哦~）
                if (ImproveGame.Instance.RenderTarget2DPool.TryBorrow(rt2dWidth, rt2dHeight, out RenderTarget2D childRT2D))
                {
                    // 记录原来的 RenderTarget2D
                    RenderTargetBinding[] renderTargetBindings = GraphicsDevice.GetRenderTargets();
                    // 记录原来的 Usage
                    RenderTargetUsage origUsage = GraphicsDevice.PresentationParameters.RenderTargetUsage;
                    // 设置为 PreserveContents
                    GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

                    SpriteBatch.End();

                    GraphicsDevice.SetRenderTarget(childRT2D);
                    GraphicsDevice.Clear(Color.Transparent);
                    Begin_Immediate(SamplerState.LinearClamp);

                    string text = $"width: {rt2dWidth} height: {rt2dHeight}";
                    DrawString(new Vector2(), text, Color.White, Color.Black);
                    DrawChildren(ownDrawArgs);

                    SpriteBatch.End();
                    GraphicsDevice.SetRenderTargets(renderTargetBindings);
                    Begin_Immediate(SamplerState.LinearClamp);

                    Color childColor = HoverTimer.Lerp(ChildColor.Item1, ChildColor.Item2);
                    SpriteBatch.Draw(childRT2D, position + size / 2f, null, childColor, 0f, new Vector2(rt2dWidth, rt2dHeight) / 2f,
                        new Vector2(size.X / rt2dWidth, size.Y / rt2dHeight), 0, 0f);

                    SpriteBatch.End();
                    Begin_Immediate(SamplerState.LinearClamp);

                    GraphicsDevice.PresentationParameters.RenderTargetUsage = origUsage;
                    // 归还 RT2D（记住了！）
                    ImproveGame.Instance.RenderTarget2DPool.TryReturn(childRT2D);
                }
                break;
        }
    }

    public virtual void DrawSelf(DrawArgs drawArgs) { }

    public virtual void DrawChildren(DrawArgs drawArgs)
    {
        foreach (XUIElement child in Children)
        {
            child.Draw(drawArgs);
        }
    }
}
