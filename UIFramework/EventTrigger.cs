using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using Terraria.GameInput;

namespace ImproveGame.UIFramework;

#region enum and struct
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

public struct MouseTarget()
{
    public UIElement LeftButton = null, MiddleButton = null, RightButton = null;

    public UIElement this[MouseButtonType button]
    {
        readonly get
        {
            return button switch
            {
                MouseButtonType.Left => LeftButton,
                MouseButtonType.Middle => MiddleButton,
                MouseButtonType.Right => RightButton,
                _ => throw new NotImplementedException()
            };
        }
        set
        {
            switch (button)
            {
                case MouseButtonType.Left:
                    LeftButton = value;
                    break;
                case MouseButtonType.Right:
                    RightButton = value;
                    break;
                case MouseButtonType.Middle:
                    MiddleButton = value;
                    break;
            }
        }
    }
}
#endregion

/// <summary>
/// 事件触发器，用于取代 <see cref="UserInterface"/><br/>
/// 未实现 侧键 和 双击 事件。
/// </summary>
public class EventTrigger(string layerName, string name) : IComparable<EventTrigger>
{
    /// <summary>
    /// 优先级, 不常用
    /// </summary>
    public virtual int Priority { get; set; }

    public int CompareTo(EventTrigger other) => -Priority.CompareTo(other.Priority);

    public string LayerName { get; init; } = layerName;
    public string Name { get; init; } = name;

    public BaseBody RootBody { get; protected set; }

    public Vector2 MouseFocus { get; protected set; }
    public UIElement CurrentHoverTarget { get; protected set; }
    public UIElement PreviousHoverTarget { get; protected set; }

    public MouseTarget PreviousMouseTargets => _previousMouseTargets;
    protected MouseTarget _previousMouseTargets = new();

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

    /// <summary>
    /// 更新鼠标位置
    /// </summary>
    public virtual void UpdateMouseFocus()
    {
        MouseFocus = EventTriggerManager.MouseScreen;
    }

    /// <summary>
    /// 更新鼠标悬停目标
    /// </summary>
    public virtual void UpdateHoverTarget()
    {
        PreviousHoverTarget = CurrentHoverTarget;
        CurrentHoverTarget = (EventTriggerManager.FocusHasUIElement || RootBody.IsNotSelectable) ? null : RootBody.GetElementAt(MouseFocus);
    }

    public virtual void Update(GameTime gameTime)
    {
        if (RootBody is null or { Enabled: false })
        {
            return;
        }

        UpdateMouseFocus();

        _previousMouseState = _mouseState;
        _mouseState.SetState(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            UpdateHoverTarget();

            if (RootBody.CanSetFocusTarget(CurrentHoverTarget))
            {
                EventTriggerManager.FocusUIElement = CurrentHoverTarget;
            }

            if (CurrentHoverTarget != PreviousHoverTarget)
            {
                var mouseOverOutEvent = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
                PreviousHoverTarget?.MouseOut(mouseOverOutEvent);
                CurrentHoverTarget?.MouseOver(mouseOverOutEvent);
            }

            // 遍历三种鼠标按键：左键、右键和中键
            foreach (MouseButtonType mouseButton in Enum.GetValues(typeof(MouseButtonType)))
            {
                // 判断当前按键是否被按下
                if (_mouseState[mouseButton] && !_previousMouseState[mouseButton])
                {
                    // 如果目标元素存在且可以被优先处理，则将视图置于顶层
                    if (CurrentHoverTarget is not null && EventTriggerManager.FocusUIElement == CurrentHoverTarget)
                    {
                        EventTriggerManager.SetHeadEventTigger(this);
                    }

                    HandleMouseEvent(MouseEventType.Down, mouseButton);
                }
                else if (!_mouseState[mouseButton] && _previousMouseState[mouseButton])
                {
                    HandleMouseEvent(MouseEventType.Up, mouseButton);
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                CurrentHoverTarget?.ScrollWheel(new UIScrollWheelEvent(CurrentHoverTarget, MouseFocus, PlayerInput.ScrollWheelDeltaForUI));
            }

            RootBody.Update(gameTime);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }

    public static Action<UIMouseEvent> GetMouseDownEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseDown,
            MouseButtonType.Right => element.RightMouseDown,
            MouseButtonType.Middle => element.MiddleMouseDown,
            _ => null,
        };
    }

    public static Action<UIMouseEvent> GetMouseUpEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseUp,
            MouseButtonType.Right => element.RightMouseUp,
            MouseButtonType.Middle => element.MiddleMouseUp,
            _ => null,
        };
    }

    public static Action<UIMouseEvent> GetClickEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftClick,
            MouseButtonType.Right => element.RightClick,
            MouseButtonType.Middle => element.MiddleClick,
            _ => null,
        };
    }

    private void HandleMouseEvent(MouseEventType eventType, MouseButtonType mouseButton)
    {
        UIMouseEvent evt;

        if (eventType is MouseEventType.Down)
        {
            evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
            GetMouseDownEvent(mouseButton, CurrentHoverTarget)?.Invoke(evt);
        }
        else
        {
            evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
            GetMouseUpEvent(mouseButton, _previousMouseTargets[mouseButton])?.Invoke(evt);

            if (_previousMouseTargets[mouseButton] == CurrentHoverTarget)
            {
                evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
                GetClickEvent(mouseButton, _previousMouseTargets[mouseButton])?.Invoke(evt);
            }
        }

        _previousMouseTargets[mouseButton] = CurrentHoverTarget;
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