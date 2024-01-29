using Terraria.GameInput;

namespace ImproveGame.Interface;

public enum MouseButtonType { Left, Middle, Right }
public enum MouseEventType { Down, Up }
public struct MouseStateMinor(bool leftButton, bool middleButton, bool rightButton)
{
    public bool LeftButton = leftButton, MiddleButton = middleButton, RightButton = rightButton;

    public void SetState(bool leftButton, bool middleButton, bool rightButton)
    {
        LeftButton = leftButton; MiddleButton = middleButton; RightButton = rightButton;
    }

    public readonly bool this[MouseButtonType button] => button switch
    {
        MouseButtonType.Left => LeftButton,
        MouseButtonType.Middle => MiddleButton,
        MouseButtonType.Right => RightButton,
        _ => throw new NotImplementedException()
    };
}

/// <summary>
/// 事件触发器，用于取代 <see cref="UserInterface"/><br/>
/// 添加一个很早就执行的 <see cref="View.PreUpdate(GameTime)"/><br/>
/// 因为用不到 鼠标侧键 和 双击 事件，所以都未实现。
/// </summary>
public class EventTrigger(string layerName, string name)
{
    public string LayerName { get; init; } = layerName;
    public string Name { get; init; } = name;

    public BaseBody RootBody { get; protected set; }

    public UIElement PreviousHoverTarget { get; protected set; }

    public IReadOnlyDictionary<MouseButtonType, UIElement> PreviousMouseTargets => _previousMouseTargets;
    protected readonly Dictionary<MouseButtonType, UIElement> _previousMouseTargets = new()
    {
        { MouseButtonType.Left, null },  { MouseButtonType.Middle, null },  { MouseButtonType.Right, null }
    };

    protected MouseStateMinor _mouseState;
    protected MouseStateMinor _previousMouseState;

    /// <summary>
    /// 注册到 <see cref="EventTriggerManager"/>
    /// </summary>
    public EventTrigger Register() => EventTriggerManager.Register(this);

    public void SetRootBody(BaseBody rootBody)
    {
        if (RootBody != rootBody)
        {
            RootBody = rootBody;

            if (RootBody != null)
            {
                RootBody.Activate();
                RootBody.Recalculate();
            }
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        if (RootBody is null or { Enabled: false })
        {
            return;
        }

        Vector2 focus = EventTriggerManager.MouseScreen;

        _previousMouseState = _mouseState;
        _mouseState.SetState(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            UIElement target = EventTriggerManager.FocusHasUIElement ? null : RootBody.GetElementAt(focus);

            RootBody.PreUpdate(gameTime);

            if (target != PreviousHoverTarget)
            {
                HandleChangeTarget(target, PreviousHoverTarget, new UIMouseEvent(target, focus));
            }

            // 禁用鼠标
            if (RootBody.CanSetFocusTarget(target))
            {
                EventTriggerManager.FocusUIElement = target;
            }

            // 遍历三种鼠标按键：左键、右键和中键
            foreach (MouseButtonType mouseButton in Enum.GetValues(typeof(MouseButtonType)))
            {
                // 判断当前按键是否被按下
                if (_mouseState[mouseButton] && !_previousMouseState[mouseButton])
                {
                    HandleMouseEvent(MouseEventType.Down, target, focus, mouseButton);
                }
                else if (!_mouseState[mouseButton] && _previousMouseState[mouseButton])
                {
                    HandleMouseEvent(MouseEventType.Up, target, focus, mouseButton);
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                target?.ScrollWheel(new UIScrollWheelEvent(target, focus, PlayerInput.ScrollWheelDeltaForUI));
            }

            RootBody.Update(gameTime);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }

    private void HandleChangeTarget(UIElement target, UIElement previousHoverTarget, UIMouseEvent e)
    {
        previousHoverTarget?.MouseOut(e);
        target?.MouseOver(e);

        PreviousHoverTarget = target;
    }

    private void HandleMouseEvent(MouseEventType eventType, UIElement target, Vector2 focus, MouseButtonType mouseButton)
    {
        // 根据按键类型触发对应的事件
        switch (mouseButton)
        {
            case MouseButtonType.Left:
                if (eventType is MouseEventType.Down)
                {
                    target?.LeftMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    _previousMouseTargets[mouseButton]?.LeftMouseUp(new UIMouseEvent(target, focus));
                }

                break;
            case MouseButtonType.Middle:
                if (eventType is MouseEventType.Down)
                {
                    target?.MiddleMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    _previousMouseTargets[mouseButton]?.MiddleMouseUp(new UIMouseEvent(target, focus));
                }

                break;
            case MouseButtonType.Right:
                if (eventType is MouseEventType.Down)
                {
                    target?.RightMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    _previousMouseTargets[mouseButton]?.RightMouseUp(new UIMouseEvent(target, focus));
                }

                break;
        }

        // 如果目标元素存在且可以被优先处理，则将视图置于顶层
        if (eventType is MouseEventType.Down && target is not null && EventTriggerManager.FocusUIElement == target)
        {
            EventTriggerManager.SetHeadEventTigger(this);
        }

        _previousMouseTargets[mouseButton] = target;
    }

    public virtual bool Draw(bool drawToGame = true)
    {
        if (!RootBody?.Enabled ?? true)
        {
            return true;
        }

        // 类云母，UI本身不透明，背景是游戏画面高斯模糊处理后的结果
        // drawToGame 表示这是拿来绘制到游戏上的，不是拿来造玻璃或者干啥的
        if (drawToGame && GlassVfxType is GlassType.MicaLike)
        {
            var layers = EventTriggerManager.EventTriggerInstances["Radial Hotbars"];
            int index = layers.IndexOf(this);
            if (index is -1)
            {
                RootBody?.Draw(Main.spriteBatch);
                return true;
            }

            Main.spriteBatch.ReBegin(null, Matrix.Identity);
            Main.spriteBatch.Draw(GlassmorphismVfx.GlassCovers[index], Vector2.Zero, Color.White);
            Main.spriteBatch.ReBegin(null, Main.UIScaleMatrix);
            RootBody?.Draw(Main.spriteBatch);
            return true;
        }

        RootBody?.Draw(Main.spriteBatch);

        return true;
    }
}