using Terraria.GameInput;

namespace ImproveGame.Interface;

public enum MouseButton { Left, Middle, Right }
public enum MouseEvent { Down, Up }
public struct MouseStateMinor(bool leftButton, bool middleButton, bool rightButton)
{
    public bool LeftButton { get; set; } = leftButton;
    public bool MiddleButton { get; set; } = middleButton;
    public bool RightButton { get; set; } = rightButton;

    public readonly bool this[MouseButton button] => button switch
    {
        MouseButton.Left => LeftButton,
        MouseButton.Middle => MiddleButton,
        MouseButton.Right => RightButton,
        _ => throw new NotImplementedException()
    };
}

/// <summary>
/// 事件触发器，用于取代 <see cref="UserInterface"/><br/>
/// 添加一个很早就执行的 <see cref="View.PreUpdate(GameTime)"/><br/>
/// 因为完全 鼠标侧键 和 双击 事件，所以都未实现。
/// </summary>
/// <param name="layerName"></param>
/// <param name="name"></param>
public class EventTrigger(string layerName, string name)
{
    public string LayerName { get; protected set; } = layerName;
    public string Name { get; protected set; } = name;

    public BaseBody BaseBody { get; protected set; }

    protected UIElement PreviousHoverTarget;
    protected readonly Dictionary<MouseButton, UIElement> PreviousMouseTargets = new()
    {
        { MouseButton.Left, null },
        { MouseButton.Middle, null },
        { MouseButton.Right, null }
    };

    protected MouseStateMinor MouseState;
    protected MouseStateMinor PreviousMouseState;

    public EventTrigger Register()
    {
        if (!EventTriggerManager.LayersPriority.Contains(LayerName))
        {
            EventTriggerManager.LayersPriority.Add(LayerName);
        }

        if (!EventTriggerManager.EventTriggerInstances.ContainsKey(LayerName))
        {
            EventTriggerManager.EventTriggerInstances.Add(LayerName, []);
        }

        var targgers = EventTriggerManager.EventTriggerInstances[LayerName];

        if (!targgers.Contains(this))
        {
            targgers.Add(this);
        }

        return this;
    }

    public void SetBaseBody(BaseBody baseBody)
    {
        if (BaseBody == baseBody)
        {
            return;
        }

        BaseBody = baseBody;

        if (BaseBody != null)
        {
            BaseBody.Activate();
            BaseBody.Recalculate();
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        if (!BaseBody?.Enabled ?? true)
        {
            return;
        }

        Vector2 focus = EventTriggerManager.MouseScreen;

        PreviousMouseState = MouseState;
        MouseState = new MouseStateMinor(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            UIElement target = EventTriggerManager.FocusHasUIElement ? null : BaseBody.GetElementAt(focus);

            BaseBody.PreUpdate(gameTime);

            if (target != PreviousHoverTarget)
            {
                HandleChangeTarget(target, PreviousHoverTarget, new UIMouseEvent(target, focus));
            }

            // 禁用鼠标
            if (BaseBody.CanSetFocusUIElement(target))
            {
                EventTriggerManager.FocusUIElement = target;
            }

            // 遍历三种鼠标按键：左键、右键和中键
            foreach (MouseButton mouseButton in Enum.GetValues(typeof(MouseButton)))
            {
                // 判断当前按键是否被按下
                if (MouseState[mouseButton])
                {
                    if (!PreviousMouseState[mouseButton])
                    {
                        HandleMouseEvent(MouseEvent.Down, target, focus, mouseButton);
                    }
                }
                else
                {
                    if (PreviousMouseState[mouseButton])
                    {
                        HandleMouseEvent(MouseEvent.Up, target, focus, mouseButton);
                    }
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                target?.ScrollWheel(new UIScrollWheelEvent(target, focus, PlayerInput.ScrollWheelDeltaForUI));
            }

            BaseBody.Update(gameTime);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }

    private void HandleChangeTarget(UIElement target, UIElement originalTarget, UIMouseEvent @event)
    {
        originalTarget?.MouseOut(@event);
        target?.MouseOver(@event);

        PreviousHoverTarget = target;
    }

    private void HandleMouseEvent(MouseEvent eventType, UIElement target, Vector2 focus, MouseButton mouseButton)
    {
        // 根据按键类型触发对应的事件
        switch (mouseButton)
        {
            case MouseButton.Left:
                if (eventType is MouseEvent.Down)
                {
                    target?.LeftMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    PreviousMouseTargets[mouseButton]?.LeftMouseUp(new UIMouseEvent(target, focus));
                }

                break;
            case MouseButton.Middle:
                if (eventType is MouseEvent.Down)
                {
                    target?.MiddleMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    PreviousMouseTargets[mouseButton]?.MiddleMouseUp(new UIMouseEvent(target, focus));
                }

                break;
            case MouseButton.Right:
                if (eventType is MouseEvent.Down)
                {
                    target?.RightMouseDown(new UIMouseEvent(target, focus));
                }
                else
                {
                    PreviousMouseTargets[mouseButton]?.RightMouseUp(new UIMouseEvent(target, focus));
                }

                break;
        }

        // 如果目标元素存在且可以被优先处理，则将视图置于顶层
        if (eventType is MouseEvent.Down && target != null && BaseBody.CanPriority(target))
        {
            EventTriggerManager.SetHeadEventTigger(this);
        }

        PreviousMouseTargets[mouseButton] = target;
    }

    public virtual bool Draw(bool drawToGame = true)
    {
        if (!BaseBody?.Enabled ?? true)
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
                BaseBody?.Draw(Main.spriteBatch);
                return true;
            }

            Main.spriteBatch.ReBegin(null, Matrix.Identity);
            Main.spriteBatch.Draw(GlassmorphismVfx.GlassCovers[index], Vector2.Zero, Color.White);
            Main.spriteBatch.ReBegin(null, Main.UIScaleMatrix);
            BaseBody?.Draw(Main.spriteBatch);
            return true;
        }

        BaseBody?.Draw(Main.spriteBatch);

        return true;
    }
}