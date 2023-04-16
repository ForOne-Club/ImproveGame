using ImproveGame.QolUISystem.UIElements;
using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.QolUISystem;

public class UIController
{
    public UIManager UIManager;

    public Vector2 MousePosition;

    public List<XUIElement> PastMouseLeftTargets = new List<XUIElement>();
    public List<XUIElement> PastMouseRightTargets = new List<XUIElement>();
    public List<XUIElement> PastMouseMiddleTargets = new List<XUIElement>();
    public List<XUIElement> PastHoverTargets = new List<XUIElement>();
    public MouseKey PastMouseKey;

    /// <summary>
    /// 鼠标事件处理
    /// 检测是否触发鼠标的基本事件
    /// </summary>
    public void MouseEventHandler(GameTime gameTime)
    {
        // 清除临时数据
        PastHoverTargets.ForEach(target =>
        {
            target.MouseHover = false;
        });

        bool mouseLeft = Main.mouseLeft;
        bool mouseRight = Main.mouseRight;
        bool mouseMiddle = Main.mouseMiddle;

        // 获取鼠标光标的目标，仅获取最顶成的元素以及他的父元素们
        List<XUIElement> cursorTargets = UIManager.GetCursorTargets(MousePosition);
        List<XUIElement> mouseLeftTargets = PastMouseKey.Left ? new List<XUIElement>() : new List<XUIElement>(cursorTargets);
        List<XUIElement> mouseRightTargets = PastMouseKey.Right ? new List<XUIElement>() : new List<XUIElement>(cursorTargets);
        List<XUIElement> mouseMiddleTargets = PastMouseKey.Middle ? new List<XUIElement>() : new List<XUIElement>(cursorTargets);

        #region 鼠标光标悬浮事件处理
        // 遍历上一帧的悬浮目标
        // 如果没有在新的一帧存在，就代表鼠标移出
        // 执行 View.CursorLeave()
        for (int i = 0; i < PastHoverTargets.Count; i++)
        {
            XUIElement target = PastHoverTargets[i];

            if (!target.MouseHover)
            {
                target.CursorLeave(PastHoverTargets);
                PastHoverTargets.RemoveAt(i--);
            }
        }

        // 进入
        for (int i = 0; i < cursorTargets.Count; i++)
        {
            XUIElement target = cursorTargets[i];
            target.CursorEnter(cursorTargets);
            PastHoverTargets.Add(target);
        }
        #endregion

        #region 左键事件处理
        for (int i = 0; i < PastMouseLeftTargets.Count; i++)
        {
            XUIElement target = PastMouseLeftTargets[i];

            if (!mouseLeft && target.MouseKey.Left)
            {
                target.MouseLeftUp(PastMouseLeftTargets);
                target.MouseKey.Left = false;

                if (target.Contains(MousePosition))
                {
                    target.MouseLeftClick(PastMouseLeftTargets);
                }

                PastMouseLeftTargets.RemoveAt(i--);
            }
        }

        for (int i = 0; i < mouseLeftTargets.Count; i++)
        {
            XUIElement target = mouseLeftTargets[i];

            if (mouseLeft && !target.MouseKey.Left)
            {
                target.MouseLeftDown(mouseLeftTargets);
                target.MouseKey.Left = true;

                PastMouseLeftTargets.Add(target);
            }
        }
        #endregion

        #region 右键事件处理
        for (int i = 0; i < PastMouseRightTargets.Count; i++)
        {
            XUIElement target = PastMouseRightTargets[i];

            if (!mouseRight && target.MouseKey.Left)
            {
                target.MouseRightUp(PastMouseRightTargets);
                target.MouseKey.Left = false;

                if (target.Contains(MousePosition))
                {
                    target.MouseRightClick(PastMouseLeftTargets);
                }

                PastMouseRightTargets.RemoveAt(i--);
            }
        }

        for (int i = 0; i < mouseRightTargets.Count; i++)
        {
            XUIElement target = mouseRightTargets[i];

            if (mouseRight && !target.MouseKey.Left)
            {
                target.MouseRightDown(mouseRightTargets);
                target.MouseKey.Left = true;

                PastMouseRightTargets.Add(target);
            }
        }
        #endregion

        #region 中键事件处理
        for (int i = 0; i < PastMouseMiddleTargets.Count; i++)
        {
            XUIElement target = PastMouseMiddleTargets[i];

            if (!mouseMiddle && target.MouseKey.Left)
            {
                target.MouseMiddleUp(PastMouseMiddleTargets);
                target.MouseKey.Left = false;

                if (target.Contains(MousePosition))
                {
                    target.MouseMiddleClick(PastMouseLeftTargets);
                }

                PastMouseMiddleTargets.RemoveAt(i--);
            }
        }

        for (int i = 0; i < mouseMiddleTargets.Count; i++)
        {
            XUIElement target = mouseMiddleTargets[i];

            if (mouseMiddle && !target.MouseKey.Left)
            {
                target.MouseMiddleDown(mouseMiddleTargets);
                target.MouseKey.Left = true;

                PastMouseMiddleTargets.Add(target);
            }
        }
        #endregion

        // 记录鼠标按下状态, 防止错误触发事件
        PastMouseKey.SetValue(Main.mouseLeft, Main.mouseRight, Main.mouseMiddle);
    }

    public void UpdateUI(GameTime gameTime)
    {
        MousePosition = new Vector2(Main.mouseX, Main.mouseY);
        MouseEventHandler(gameTime);
        UIManager.Update();
    }
}
