using ImproveGame.Interface.Common;
using Terraria.GameInput;

namespace ImproveGame.Interface
{
    /// <summary>
    /// 事件触发器，用于取代原版的 UserInterface <br/>
    /// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click <br/>
    /// 鼠标侧键方法 & 双击方法都取消了，因为用不到。 <br/>
    /// (全凭感觉设计) <br/>
    /// (越写越乱，不确定哪里写的有没有问题，使用遇到问题了随时告诉我) <br/>
    /// </summary>
    public class EventTrigger
    {
        private static bool DisableMouse { get; set; }
        private static EventTrigger CurrentEventTrigger { get; set; }

        private static readonly List<string> LayersPriority;
        private static readonly Dictionary<string, List<EventTrigger>> LayersDictionary;

        static EventTrigger()
        {
            LayersPriority = new List<string>();
            LayersDictionary = new Dictionary<string, List<EventTrigger>>();

            DisableMouse = false;
        }

        /// <summary>
        /// 首次调用时 CurrentEventTrigger 已经被赋值了<br/>
        /// 所以不用判断是不是 null
        /// </summary>
        private static void MakePriority()
        {
            string layerName = CurrentEventTrigger._layerName;
            if (LayersDictionary[layerName][0] == CurrentEventTrigger)
            {
                return;
            }

            LayersDictionary[layerName].Remove(CurrentEventTrigger);
            LayersDictionary[layerName].Insert(0, CurrentEventTrigger);
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="gameTime"></param>
        public static void UpdateUI(GameTime gameTime)
        {
            foreach (string layerName in LayersPriority.Where(LayersDictionary.ContainsKey))
            {
                List<EventTrigger> triggers = LayersDictionary[layerName];
                for (int i = 0; i < triggers.Count; i++)
                {
                    CurrentEventTrigger = triggers[i];
                    CurrentEventTrigger.Update(gameTime);
                }
            }

            DisableMouse = false;
        }

        /// <summary>
        /// 嗯 嗯 啊啊啊啊
        /// </summary>
        public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // 防止进入世界时UI闪一下
            if (!UIPlayer.ShouldShowUI)
                return;
            
            var layerIndex = new Dictionary<string, int>();

            // 插入到绘制层
            foreach (KeyValuePair<string, List<EventTrigger>> keyValuePair in LayersDictionary)
            {
                layers.FindVanilla(keyValuePair.Key, index =>
                {
                    layerIndex.Add(keyValuePair.Key, index);
                    foreach (EventTrigger trigger in keyValuePair.Value)
                    {
                        layers.Insert(index + 1, new LegacyGameInterfaceLayer($"ImproveGame: {trigger._name}",
                            () => trigger.Draw(), InterfaceScaleType.UI));
                    }
                });
            }

            LayersPriority.Sort(((a, b) => -layerIndex[a].CompareTo(layerIndex[b])));
        }

        private readonly string _name, _layerName;

        public ViewBody ViewBody { get => _body; protected set => _body = value; }
        private ViewBody _body;
        private Vector2 _mouse;
        private readonly UIElement[] _last;
        private readonly bool[] _pressed;

        public ViewBody Body => _body;

        /// <summary>
        /// 我在这里提醒一下需要为 CanRunFunc 赋值否则 UI 将永不生效<br/>
        /// CanRunFunc 用于判断此 UI 是否执行 Update() 与 Draw() <br/>
        /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
        /// </summary>
        /// <param name="layerName">114514</param>
        /// <param name="name">114514</param>
        public EventTrigger(string layerName, string name)
        {
            _last = new UIElement[4];
            _pressed = new bool[3];

            _layerName = layerName;
            _name = name;

            if (!LayersPriority.Contains(layerName))
                LayersPriority.Add(layerName);
            if (!LayersDictionary.ContainsKey(layerName))
                LayersDictionary.Add(layerName, new List<EventTrigger>());

            LayersDictionary[layerName].Add(this);
        }

        public void SetCarrier(ViewBody viewBody)
        {
            if (_body == viewBody) return;

            _body = viewBody;
            if (viewBody is null)
            {
                return;
            }

            _body.Activate();
            _body.Recalculate();
        }

        protected virtual void Update(GameTime gameTime)
        {
            if (!_body?.Display ?? true)
            {
                return;
            }

            _mouse = new Vector2(Main.mouseX, Main.mouseY);
            bool mouse = DisableMouse;
            bool[] down = { Main.mouseLeft, Main.mouseRight, Main.mouseMiddle };
            // 鼠标目标元素
            UIElement target = mouse ? null : _body.GetElementAt(_mouse);
            var targetMouseEvent = new UIMouseEvent(target, _mouse);

            try
            {
                // 当前目标元素不是上一个目标元素
                if (_last[^1] != target)
                {
                    // 鼠标离开元素
                    _last[^1]?.MouseOut(new UIMouseEvent(_last[^1], _mouse));
                    _last[^1] = target;
                    // 鼠标进入元素
                    target?.MouseOver(targetMouseEvent);
                }

                // 禁用鼠标
                if (_body.CanDisableMouse(target))
                {
                    DisableMouse = true;
                }

                for (int i = 0; i < 3; i++)
                {
                    switch (down[i])
                    {
                        case true when !_pressed[i]:
                            _last[i] = target;
                            // 按下事件
                            switch (i)
                            {
                                case 0:
                                    target?.LeftMouseDown(new UIMouseEvent(target, _mouse));
                                    break;
                                case 1:
                                    target?.RightMouseDown(new UIMouseEvent(target, _mouse));
                                    break;
                                default:
                                    target?.MiddleMouseDown(new UIMouseEvent(target, _mouse));
                                    break;
                            }

                            // 视图置于顶层
                            if (target != null && _body.CanPriority(target))
                            {
                                MakePriority();
                            }

                            break;
                        case false when _pressed[i] && _last[i] != null:
                            {
                                if (_last[i].ContainsPoint(_mouse))
                                {
                                    // 左键点击事件
                                    switch (i)
                                    {
                                        case 0:
                                            _last[i]?.LeftClick(new UIMouseEvent(target, _mouse));
                                            break;
                                        case 1:
                                            _last[i]?.RightClick(new UIMouseEvent(target, _mouse));
                                            break;
                                        case 2:
                                            _last[i]?.MiddleClick(new UIMouseEvent(target, _mouse));
                                            break;
                                    }
                                }

                                // 鼠标离开事件
                                switch (i)
                                {
                                    case 0:
                                        _last[i]?.LeftMouseUp(new UIMouseEvent(target, _mouse));
                                        break;
                                    case 1:
                                        _last[i]?.RightMouseUp(new UIMouseEvent(target, _mouse));
                                        break;
                                    case 2:
                                        _last[i]?.MiddleMouseUp(new UIMouseEvent(target, _mouse));
                                        break;
                                }

                                _last[i] = null;
                                break;
                            }
                    }
                }

                if (PlayerInput.ScrollWheelDeltaForUI != 0)
                {
                    target?.ScrollWheel(new UIScrollWheelEvent(target, _mouse,
                        PlayerInput.ScrollWheelDeltaForUI));
                }

                _body.Update(gameTime);
            } finally
            {
                _pressed[0] = down[0] && !mouse;
                _pressed[1] = down[1] && !mouse;
                _pressed[2] = down[2] && !mouse;
            }
        }
        
        protected virtual bool Draw()
        {
            if (!_body?.Display ?? true)
            {
                return true;
            }

            _body?.Draw(Main.spriteBatch);
            return true;
        }
    }
}