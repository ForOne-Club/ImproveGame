using Terraria.GameInput;

namespace ImproveGame.Interface
{
    /// <summary>
    /// 事件触发器，用于取代原版的 UserInterface
    /// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click
    /// 鼠标侧键方法 & 双击方法都取消了，因为用不到。
    /// （全凭感觉设计）
    /// （越写越乱，不确定哪里写的有没有问题，使用遇到问题了随时告诉我）
    /// </summary>
    public class EventTrigger
    {
        private static readonly List<EventTrigger> UpdateList = new List<EventTrigger>();

        private static readonly Dictionary<string, List<EventTrigger>> DrawDictionary =
            new Dictionary<string, List<EventTrigger>>();

        private static bool _lockCursor;
        private static EventTrigger _currentEventTrigger;
        private static EventTrigger _primaryEventTrigger;

        /// <summary>
        /// 锁定接下来布局的 Update() 的非点按操作事件。
        /// </summary>
        public static void LockCursor()
        {
            _lockCursor = true;
        }

        // 将当前 UI 的 Update 与 Draw 设置到最顶层
        public static void SetPrimaryElements()
        {
            _primaryEventTrigger = _currentEventTrigger;
        }

        public static void UpdateUI(GameTime gameTime)
        {
            // 执行顶层 EventTrigger
            _primaryEventTrigger?.Update(gameTime);

            // 执行普通的 EventTrigger
            foreach (var eventTrigger in UpdateList.Where(eventTrigger => eventTrigger != _primaryEventTrigger))
            {
                eventTrigger.Update(gameTime);
            }

            _lockCursor = false;
        }

        /// <summary>
        /// 这个是根据 layersName 遍历 DrawDictionary
        /// </summary>
        /// <param name="layersName"></param>
        /// <returns>没啥用~</returns>
        public static bool DrawAllUI(string layersName)
        {
            if (!DrawDictionary.ContainsKey(layersName))
            {
                return true;
            }

            List<EventTrigger> eventTriggers = DrawDictionary[layersName];
            foreach (var eventTrigger in eventTriggers.Where(eventTrigger => eventTrigger != _primaryEventTrigger))
            {
                eventTrigger.Draw();
            }

            if (DrawDictionary[layersName].Contains(_primaryEventTrigger))
            {
                _primaryEventTrigger.Draw();
            }

            return true;
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
        /// priority 是 Update() 和 Draw() 执行的优先级，值越大越先执行，且越晚绘制。 <br/>
        /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
        /// </summary>
        /// <param name="layersName"></param>
        /// <param name="priority">Update() 优先级</param>
        public EventTrigger(string layersName, int priority)
        {
            _priority = priority;
            UpdateList.Add(this);
            if (!DrawDictionary.ContainsKey(layersName))
            {
                DrawDictionary.Add(layersName, new List<EventTrigger>());
            }

            DrawDictionary[layersName].Add(this);

            PrioritySort(layersName);
        }

        // 对 EventTriggers 和 name 指定的 DrawDictionary 进行排序
        private static void PrioritySort(string name)
        {
            UpdateList.Sort((a, b) => -a._priority.CompareTo(b._priority));

            if (!DrawDictionary.ContainsKey(name))
            {
                return;
            }

            DrawDictionary[name].Sort((a, b) => a._priority.CompareTo(b._priority));
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
        private bool _executedElementDown;
        private bool _executedElementRightDown;
        private bool _executedElementMiddleDown;

        // CanRunFuc 用于此处，来判断 Update 是否继续向下执行
        // 并且设置 _canRun 用于在 Draw 判定是否继续执行
        private void Update(GameTime gameTime)
        {
            _currentEventTrigger = this;
            if (!CanRunFunc?.Invoke() ?? true)
            {
                _canRun = false;
                return;
            }

            _canRun = true;

            if (_state is null)
                return;

            if (!Main.hasFocus)
            {
                return;
            }

            _mousePosition = new Vector2(Main.mouseX, Main.mouseY);
            // 鼠标目标元素
            UIElement target = _lockCursor ? null : _state.GetElementAt(_mousePosition);
            var targetMouseEvent = new UIMouseEvent(target, _mousePosition);
            bool lockMouseLeft = Main.mouseLeft && !_lockCursor;
            bool lockMouseRight = Main.mouseRight && !_lockCursor;
            bool lockMouseMiddle = Main.mouseMiddle && !_lockCursor;

            try
            {
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
                    case true when !_executedElementDown:
                        // 按下事件
                        target?.MouseDown(targetMouseEvent);
                        _lastElementDown = target;
                        break;
                    case false when _executedElementDown && _lastElementDown != null:
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
                    case true when !_executedElementRightDown:
                        target?.RightMouseDown(targetMouseEvent);
                        _lastElementRightDown = target;
                        break;
                    case false when _executedElementRightDown && _lastElementRightDown != null:
                        {
                            UIElement lastElementRightDown = _lastElementRightDown;
                            // 点击事件
                            if (lastElementRightDown.ContainsPoint(_mousePosition))
                            {
                                lastElementRightDown.RightClick(new UIMouseEvent(lastElementRightDown,
                                    _mousePosition));
                            }

                            // 鼠标离开事件
                            lastElementRightDown.RightMouseUp(new UIMouseEvent(lastElementRightDown, _mousePosition));
                            _lastElementRightDown = null;
                            break;
                        }
                }

                switch (Main.mouseMiddle)
                {
                    case true when !_executedElementMiddleDown:
                        target?.MiddleMouseDown(targetMouseEvent);
                        _lastElementMiddleDown = target;
                        break;
                    case false when _executedElementMiddleDown && _lastElementMiddleDown != null:
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

                _state.Update(gameTime);
            } finally
            {
                _executedElementDown = lockMouseLeft;
                _executedElementRightDown = lockMouseRight;
                _executedElementMiddleDown = lockMouseMiddle;
            }
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