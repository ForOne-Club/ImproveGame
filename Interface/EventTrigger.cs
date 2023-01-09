using Terraria.GameInput;

namespace ImproveGame.Interface
{
    /// <summary>
    /// 事件触发器，用于取代原版的 UserInterface
    /// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click
    /// 鼠标侧键方法 & 双击方法都取消了，因为用不到。
    /// </summary>
    public class EventTrigger
    {
        private static readonly List<EventTrigger> EventTriggers = new List<EventTrigger>();

        private static void PrioritySort()
        {
            EventTriggers.Sort((a, b) => -a._priority.CompareTo(b._priority));
        }

        public static void UpdateUI(GameTime gameTime)
        {
            foreach (EventTrigger value in EventTriggers)
            {
                value.Update(gameTime);
            }
        }

        /// <summary>
        /// 用于判断此 UI 是否执行 Update() 与 Draw()
        /// </summary>
        public Func<bool> CanRunFunc;

        private bool _canRun;

        private readonly int _priority;

        private UIState _state;
        private Vector2 _mousePosition;

        /// <summary>
        /// 我在这里提醒一下需要为 CanRunFunc 赋值否则 UI 将永不生效<br/>
        /// CanRunFunc 用于判断此 UI 是否执行 Update() 与 Draw() <br/>
        /// priority 是 Update() 执行的优先级，值越大越先执行。 <br/>
        /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
        /// </summary>
        /// <param name="priority">Update() 优先级</param>
        public EventTrigger(int priority)
        {
            _priority = priority;
            EventTriggers.Add(this);
            PrioritySort();
        }

        public void SetState(UIState uiState)
        {
            if (uiState is null || _state == uiState)
            {
                return;
            }

            _state = uiState;
            _state.Activate();
            _state.Recalculate();
        }

        private UIElement _lastElementHover;
        private UIElement _lastElementDown;
        private UIElement _lastElementRightDown;
        private UIElement _lastElementMiddleDown;
        private bool _wasElementDown;
        private bool _wasElementRightDown;
        private bool _wasElementMiddleDown;

        private void Update(GameTime gameTime)
        {
            if (CanRunFunc is null)
            {
                _canRun = false;
                return ;
            }

            if (_state is null)
                return;

            _mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            if (Main.hasFocus)
            {
                try
                {
                    // 鼠标目标元素
                    UIElement target = _state.GetElementAt(_mousePosition);
                    var targetMouseEvent = new UIMouseEvent(target, _mousePosition);
                    // 当前目标元素不是上一个目标元素
                    if (_lastElementHover != target)
                    {
                        // 鼠标移出元素
                        _lastElementHover?.MouseOut(new UIMouseEvent(_lastElementHover, _mousePosition));
                        // 鼠标移入元素
                        target?.MouseOver(targetMouseEvent);
                        _lastElementHover = target;
                    }

                    switch (Main.mouseLeft)
                    {
                        case true when !_wasElementDown:
                            // 按下事件
                            target?.MouseDown(targetMouseEvent);
                            _lastElementDown = target;
                            break;
                        case false when _wasElementDown && _lastElementDown != null:
                            {
                                UIElement lastElementDown = _lastElementDown;
                                // 左键点击事件
                                if (lastElementDown.ContainsPoint(_mousePosition))
                                {
                                    lastElementDown.Click(new UIMouseEvent(lastElementDown, _mousePosition));
                                }

                                // 鼠标离开事件
                                lastElementDown.MouseUp(new UIMouseEvent(lastElementDown, _mousePosition));
                                _lastElementDown = null;
                                break;
                            }
                    }

                    switch (Main.mouseRight)
                    {
                        case true when !_wasElementRightDown:
                            target?.RightMouseDown(targetMouseEvent);
                            _lastElementRightDown = target;
                            break;
                        case false when _wasElementRightDown && _lastElementRightDown != null:
                            {
                                UIElement lastElementRightDown = _lastElementRightDown;
                                // 点击事件
                                if (lastElementRightDown.ContainsPoint(_mousePosition))
                                {
                                    lastElementRightDown.RightClick(new UIMouseEvent(lastElementRightDown,
                                        _mousePosition));
                                }

                                // 鼠标离开事件
                                lastElementRightDown.RightMouseUp(
                                    new UIMouseEvent(lastElementRightDown, _mousePosition));
                                _lastElementRightDown = null;
                                break;
                            }
                    }

                    switch (Main.mouseMiddle)
                    {
                        case true when !_wasElementMiddleDown:
                            target?.MiddleMouseDown(targetMouseEvent);
                            _lastElementMiddleDown = target;
                            break;
                        case false when _wasElementMiddleDown && _lastElementMiddleDown != null:
                            {
                                UIElement lastElementMiddleDown = _lastElementMiddleDown;
                                // 点击事件
                                if (lastElementMiddleDown.ContainsPoint(_mousePosition))
                                {
                                    lastElementMiddleDown.MiddleClick(new UIMouseEvent(lastElementMiddleDown,
                                        _mousePosition));
                                }

                                // 鼠标离开事件
                                lastElementMiddleDown.MiddleMouseUp(new UIMouseEvent(lastElementMiddleDown,
                                    _mousePosition));
                                _lastElementRightDown = null;
                                break;
                            }
                    }

                    if (PlayerInput.ScrollWheelDeltaForUI != 0)
                    {
                        target?.ScrollWheel(new UIScrollWheelEvent(target, _mousePosition,
                            PlayerInput.ScrollWheelDeltaForUI));
                    }
                } finally
                {
                    _wasElementDown = Main.mouseLeft;
                    _wasElementRightDown = Main.mouseRight;
                    _wasElementMiddleDown = Main.mouseMiddle;
                }
            }

            _state.Update(gameTime);
        }

        public void Draw()
        {
            if (!_canRun)
            {
                return;
            }

            _state?.Draw(Main.spriteBatch);
        }
    }
}