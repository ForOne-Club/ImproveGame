using Terraria.GameInput;

namespace ImproveGame.Interface
{
    /*
        *********************************************
                           _ooOoo_
                          o8888888o
                          88" . "88
                          (| -_- |)
                          O\  =  /O
                       ____/`---'\____
                     .'  \\|     |//  `.
                    /  \\|||  :  |||//  \
                   /  _||||| -:- |||||-  \
                   |   | \\\  -  /// |   |
                   | \_|  ''\---/''  |   |
                   \  .-\__  `-`  ___/-. /
                 ___`. .'  /--.--\  `. . __
              ."" '<  `.___\_<|>_/___.'  >'"".
             | | :  `- \`.;`\ _ /`;.`/ - ` : | |
             \  \ `-.   \_ __\ /__ _/   .-` /  /
        ======`-.____`-.___\_____/___.-`____.-'======
                           `=---='
        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                   佛祖保佑       永无BUG
     */
    /// <summary>
    /// 事件触发器，用于取代原版的 UserInterface <br/>
    /// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click <br/>
    /// 鼠标侧键方法 & 双击方法都取消了，因为用不到。 <br/>
    /// (全凭感觉设计) <br/>
    /// (越写越乱，不确定哪里写的有没有问题，使用遇到问题了随时告诉我) <br/>
    /// </summary>
    public class EventTrigger
    {
        private static readonly List<string> LayersPriority;
        private static readonly Dictionary<string, List<EventTrigger>> LayersDictionary;

        private static bool _occupyCursor;
        private static EventTrigger _currentEventTrigger;
        private static EventTrigger _primaryEventTrigger;

        /// <summary>
        /// 写了跟没写似的
        /// </summary>
        static EventTrigger()
        {
            LayersPriority = new List<string>();
            LayersDictionary = new Dictionary<string, List<EventTrigger>>();

            _occupyCursor = false;
            _currentEventTrigger = null;
            _primaryEventTrigger = null;
        }

        /// <summary>
        /// 锁定接下来布局的 Update() 的非点按操作事件。
        /// </summary>
        private static void OccupyCursor()
        {
            _occupyCursor = true;
        }

        /// <summary>
        /// 将当前 UI 的 Update 与 Draw 设置到最顶层
        /// </summary>
        private static void ToPrimaryElements()
        {
            _primaryEventTrigger = _currentEventTrigger;
        }

        public static void UpdateUI(GameTime gameTime)
        {
            foreach (var layerName in LayersPriority.Where(layerName => LayersDictionary.ContainsKey(layerName)))
            {
                // 执行顶层 EventTrigger
                if (LayersDictionary[layerName].Contains(_primaryEventTrigger))
                {
                    _primaryEventTrigger?.Update(gameTime);
                }

                foreach (EventTrigger eventTrigger in LayersDictionary[layerName]
                             .Where((trigger => trigger != _primaryEventTrigger)))
                {
                    eventTrigger?.Update(gameTime);
                }
            }

            _occupyCursor = false;
        }

        /// <summary>
        /// 嗯 嗯 啊啊啊啊
        /// </summary>
        public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            Dictionary<string, int> indexDictionary = new Dictionary<string, int>();
            foreach (KeyValuePair<string, List<EventTrigger>> valuePair in LayersDictionary)
            {
                layers.FindVanilla(valuePair.Key, index =>
                {
                    indexDictionary.Add(valuePair.Key, index);
                    if (valuePair.Value.Contains(_primaryEventTrigger))
                    {
                        layers.Insert(index, _primaryEventTrigger);
                    }

                    foreach (EventTrigger eventTrigger in valuePair.Value.Where(eventTrigger =>
                                 eventTrigger != _primaryEventTrigger))
                    {
                        layers.Insert(index, eventTrigger);
                    }
                });
            }

            LayersPriority.Sort(((a, b) => -indexDictionary[a].CompareTo(indexDictionary[b])));
        }

        /// <summary>
        /// 用于判断此 UI 是否执行 Update() 与 Draw()
        /// </summary>
        public Func<bool> CanRunFunc;

        public readonly string Name;
        private readonly int _priority;

        private UIState _state;
        private Vector2 _mousePosition;

        /// <summary>
        /// 我在这里提醒一下需要为 CanRunFunc 赋值否则 UI 将永不生效<br/>
        /// CanRunFunc 用于判断此 UI 是否执行 Update() 与 Draw() <br/>
        /// priority 是 Update() 和 Draw() 执行的优先级，值越大越先执行，且越晚绘制。 <br/>
        /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
        /// </summary>
        /// <param name="layersName">114514</param>
        /// <param name="name">114514</param>
        /// <param name="priority">Update Draw 的优先级, 写到后面感觉没啥用了</param>
        public EventTrigger(string layersName, string name, int priority)
        {
            Name = name;
            _priority = priority;

            if (!LayersPriority.Contains(layersName))
            {
                LayersPriority.Add(layersName);
            }

            if (!LayersDictionary.ContainsKey(layersName))
            {
                LayersDictionary.Add(layersName, new List<EventTrigger>());
            }

            LayersDictionary[layersName].Add(this);

            PrioritySort(layersName);
        }

        // 对 EventTriggers 和 name 指定的 DrawDictionary 进行排序
        private static void PrioritySort(string name)
        {
            if (!LayersDictionary.ContainsKey(name))
            {
                return;
            }

            LayersDictionary[name].Sort((a, b) => -a._priority.CompareTo(b._priority));
        }

        public void SetState<T>(T uiState) where T : UIState, IUseEventTrigger
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
                return;
            }

            if (_state is null)
                return;

            if (!Main.hasFocus)
            {
                return;
            }

            _mousePosition = new Vector2(Main.mouseX, Main.mouseY);
            // 鼠标目标元素
            UIElement target = _occupyCursor ? null : _state.GetElementAt(_mousePosition);
            IUseEventTrigger useEventTrigger = _state as IUseEventTrigger;
            var targetMouseEvent = new UIMouseEvent(target, _mousePosition);
            bool occupyCursor = _occupyCursor;
            bool lockMouseLeft = Main.mouseLeft && !occupyCursor;
            bool lockMouseRight = Main.mouseRight && !occupyCursor;
            bool lockMouseMiddle = Main.mouseMiddle && !occupyCursor;

            try
            {
                // 当前目标元素不是上一个目标元素
                if (_lastElementHover != target)
                {
                    // 鼠标移出元素
                    _lastElementHover?.MouseOut(new UIMouseEvent(_lastElementHover, _mousePosition));
                    _lastElementHover = target;
                    // 鼠标移入元素
                    target?.MouseOver(targetMouseEvent);
                }

                // 占用此帧光标
                if (target is not null && useEventTrigger!.CanOccupyCursor(target))
                    OccupyCursor();

                switch (Main.mouseLeft)
                {
                    case true when !occupyCursor && !_executedElementDown:
                        _lastElementDown = target;
                        // 按下事件
                        target?.MouseDown(targetMouseEvent);
                        // 置于顶层
                        if (useEventTrigger!.ToPrimary(target))
                            ToPrimaryElements();
                        break;
                    case false when _executedElementDown && _lastElementDown != null:
                        {
                            // 左键点击事件
                            if (_lastElementDown.ContainsPoint(_mousePosition))
                            {
                                _lastElementDown.Click(new UIMouseEvent(_lastElementDown, _mousePosition));
                            }

                            // 鼠标离开事件
                            _lastElementDown.MouseUp(new UIMouseEvent(_lastElementDown, _mousePosition));
                            _lastElementDown = null;
                            break;
                        }
                }

                switch (Main.mouseRight)
                {
                    case true when !occupyCursor && !_executedElementRightDown:
                        _lastElementRightDown = target;
                        target?.RightMouseDown(targetMouseEvent);
                        // 置于顶层
                        if (useEventTrigger!.ToPrimary(target))
                            ToPrimaryElements();
                        break;
                    case false when _executedElementRightDown && _lastElementRightDown != null:
                        {
                            // 点击事件
                            if (_lastElementRightDown.ContainsPoint(_mousePosition))
                            {
                                _lastElementRightDown.RightClick(new UIMouseEvent(_lastElementRightDown,
                                    _mousePosition));
                            }

                            // 鼠标离开事件
                            _lastElementRightDown.RightMouseUp(new UIMouseEvent(_lastElementRightDown, _mousePosition));
                            _lastElementRightDown = null;
                            break;
                        }
                }

                switch (Main.mouseMiddle)
                {
                    case true when !occupyCursor && !_executedElementMiddleDown:
                        _lastElementMiddleDown = target;
                        // 中键按下
                        target?.MiddleMouseDown(targetMouseEvent);
                        // 置于顶层
                        if (useEventTrigger!.ToPrimary(target))
                            ToPrimaryElements();
                        break;
                    case false when _executedElementMiddleDown && _lastElementMiddleDown != null:
                        {
                            // 点击事件
                            if (_lastElementMiddleDown.ContainsPoint(_mousePosition))
                            {
                                _lastElementMiddleDown.MiddleClick(new UIMouseEvent(_lastElementMiddleDown,
                                    _mousePosition));
                            }

                            // 鼠标离开事件
                            _lastElementMiddleDown.MiddleMouseUp(new UIMouseEvent(_lastElementMiddleDown,
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
            if (!CanRunFunc?.Invoke() ?? true)
            {
                return;
            }

            _state?.Draw(Main.spriteBatch);
        }
    }
}