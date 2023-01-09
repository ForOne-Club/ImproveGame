namespace ImproveGame.Interface
{
    /// <summary>
    /// 事件触发器，用于取代原版的 UserInterface
    /// </summary>
    public class EventTrigger
    {
        private UIState _state;
        private Vector2 MousePosition;

        public EventTrigger()
        {
            UserInterface ui = new UserInterface();
            ui.SetState(null);
        }

        public void SetState(UIState uiState)
        {
            if (uiState is null || _state == uiState)
            {
                return;
            }

            _state.Activate();
            _state.Recalculate();
            _state = uiState;
        }

        private void RefreshMousePosition()
        {
            MousePosition = new Vector2(Main.mouseX, Main.mouseY);
        }

        private UIElement _lastElementHover;
        private UIElement _lastElementDown;

        public void Update(GameTime gameTime)
        {
            if (_state is null)
                return;
            RefreshMousePosition();
            if (Main.hasFocus)
            {
                // 鼠标目标元素
                UIElement target = _state.GetElementAt(MousePosition);
                // 当前目标元素不是上一个目标元素
                if (_lastElementHover != target)
                {
                    // 鼠标移出元素
                    _lastElementHover?.MouseOut(new UIMouseEvent(_lastElementHover, MousePosition));
                    // 鼠标移入元素
                    target?.MouseOver(new UIMouseEvent(target, MousePosition));
                    _lastElementHover = target;
                }

                if (Main.mouseLeft)
                {
                    target?.MouseDown(new UIMouseEvent(target, MousePosition));
                    _lastElementDown = target;
                }
            }

            _state.Update(gameTime);
        }
    }
}