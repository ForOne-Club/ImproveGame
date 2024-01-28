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
/// 事件触发器，用于取代原版的 UserInterface <br/>
/// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click <br/>
/// 鼠标侧键方法 & 双击方法都取消了，因为用不到。 <br/>
/// (全凭感觉设计) <br/>
/// (越写越乱，不确定哪里写的有没有问题，使用遇到问题了随时告诉我) <br/>
/// </summary>
public class EventTrigger
{
    public string Name { get; protected set; }
    public string LayerName { get; protected set; }
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

    /// <summary>
    /// 我在这里提醒一下需要为 CanRunFunc 赋值否则 UI 将永不生效<br/>
    /// CanRunFunc 用于判断此 UI 是否执行 Update() 与 Draw() <br/>
    /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
    /// </summary>
    /// <param name="layerName">114514</param>
    /// <param name="name">114514</param>
    public EventTrigger(string layerName, string name)
    {
        LayerName = layerName;
        Name = name;

        if (!EventTriggerManager.LayersPriority.Contains(layerName))
        {
            EventTriggerManager.LayersPriority.Add(layerName);
        }

        if (!EventTriggerManager.LayersDictionary.ContainsKey(layerName))
        {
            EventTriggerManager.LayersDictionary.Add(layerName, []);
        }

        EventTriggerManager.LayersDictionary[layerName].Add(this);
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
            UIElement target = EventTriggerManager.DisableMouse ? null : BaseBody.GetElementAt(focus);

            BaseBody.PreUpdate(gameTime);

            if (target != PreviousHoverTarget)
            {
                HandleChangeTarget(target, PreviousHoverTarget, new UIMouseEvent(target, focus));
            }

            // 禁用鼠标
            if (BaseBody.CanDisableMouse(target))
            {
                EventTriggerManager.DisableMouse = true;
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
            EventTriggerManager.MakePriority();
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
            var layers = EventTriggerManager.LayersDictionary["Radial Hotbars"];
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