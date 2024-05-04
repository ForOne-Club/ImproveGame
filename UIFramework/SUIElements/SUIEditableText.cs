using ImproveGame.Common;
using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria.GameInput;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.SUIElements;

public class SUIEditableText : TimerView
{
    #region 专门做的可以显示指针的SUI文本元素

    public class SUITextWithTicker : SUIText
    {
        public float CursorOpacity;
        public int Cursor;

        public SUITextWithTicker()
        {
            UseKey = false;
            IsWrapped = true;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawSDFRectangle();

            int cursor = Cursor;
            cursor = Math.Min(OriginalString.Length, cursor);

            var cursorColor = Color.White;
            // 算是个小彩蛋？
            if (Main.LocalPlayer.hasRainbowCursor)
                cursorColor = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.25f % 1f, 1f, 0.5f);

            cursorColor *= CursorOpacity;

            LastString = OriginalString.Insert(cursor, CursorSnippet.GenerateTag(cursorColor));

            FinalTextSnippets = TextSnippetHelper
                .ConvertNormalSnippets(TextSnippetHelper.ParseMessageWithCursorCheck(LastString, TextColor)).ToArray();
            if (_isWrapped)
                FinalTextSnippets = TextSnippetHelper.WordwrapString(FinalTextSnippets, TextColor, Font,
                    (int)(GetInnerDimensions().Width / TextScale), out _, MaxCharacterCount, MaxLines);

            TextSize = ChatManager.GetStringSize(Font, FinalTextSnippets, new Vector2(1f));

            var inner = GetInnerDimensions();

            Vector2 innerSize = inner.Size();
            Vector2 innerPos = inner.Position();

            Vector2 textSize = TextSize;
            if (textSize.Y < Font.LineSpacing)
                textSize.Y = Font.LineSpacing;
            Vector2 textPos = innerPos + TextOffset;
            textPos += TextPercentOffset * innerSize;
            textPos += TextAlign * (innerSize - textSize * TextScale);
            textPos -= TextOrigin * TextSize * TextScale;
            textPos.Y += TextScale *
                         (_isLarge ? UIConfigs.Instance.BigFontOffsetY : UIConfigs.Instance.GeneralFontOffsetY);

            DrawColorCodedStringShadow(spriteBatch, Font, FinalTextSnippets,
                textPos, TextBorderColor, 0f, Vector2.Zero, new Vector2(TextScale), -1f, TextBorder * TextScale);

            DrawColorCodedString(spriteBatch, Font, FinalTextSnippets,
                textPos, TextColor, 0f, Vector2.Zero, new Vector2(TextScale), out var _, -1f);
        }
    }

    #endregion

    /// <summary>
    /// 用于显示的文本元素
    /// </summary>
    public SUITextWithTicker InnerText;

    /// <summary>
    /// 指针位置
    /// </summary>
    public int Cursor
    {
        get => _cursor;
        set => _cursor = value;
    }

    private int _cursor;

    /// <summary>
    /// 最大文本长度
    /// </summary>
    public int MaxLength = 100;

    /// <summary>
    /// 文本
    /// </summary>
    public string Text
    {
        get => InnerText.TextOrKey;
        set
        {
            var text = value ?? "";

            if (text.Length > MaxLength)
                text = text[..MaxLength] ?? "";

            if (InnerText.TextOrKey != text)
                ContentsChanged?.Invoke(text);

            if (string.IsNullOrEmpty(text))
                Cursor = 0;

            InnerText.TextOrKey = text;

            Cursor = Math.Min(text.Length, Cursor);
        }
    }

    /// <summary>
    /// 是否正在输入
    /// </summary>
    public bool IsWritingText;

    /// <summary>
    /// 是否显示输入指针，为否则永远不显示
    /// </summary>
    public bool ShowInputTicker = true;

    /// <summary>
    /// 用于检测长按Del键，向左向右键的计时器
    /// </summary>
    private int[] _keyHoldTimer = new int[127];

    /// <summary>
    /// 用于周期性闪烁指针的计时器
    /// </summary>
    private static int _cursorBlinkCount;

    /// <summary>
    /// 是否绘制输入法框，一般建议由父元素绘制，而将此值设为false，这样的话不会被其他元素遮挡
    /// </summary>
    public bool ShowImePanel = false;

    /// <summary>
    /// 此帧按下的键
    /// </summary>
    private static List<Keys> _pressedKeys = [];

    /// <summary>
    /// 上一帧按下的键
    /// </summary>
    private static List<Keys> _oldPressedKeys = [];

    /// <summary>
    /// 当输入内容更改时触发
    /// </summary>
    public event Action<string> ContentsChanged;

    /// <summary>
    /// 当开始输入操作时触发
    /// </summary>
    public event Action StartTakingInput;

    /// <summary>
    /// 当结束输入操作时触发
    /// </summary>
    public event Action EndTakingInput;

    /// <summary>
    /// 当取消输入操作时触发（输入时按下Esc键）
    /// </summary>
    public event Action CanceledTakingInput;

    public SUIEditableText(string initialText = "")
    {
        InnerText = new SUITextWithTicker
        {
            TextOrKey = initialText,
            Height = {Percent = 1f},
            Width = {Percent = 1f},
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        InnerText.JoinParent(this);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        ToggleTakingText();
        SoundEngine.PlaySound(SoundID.MenuTick);

        base.LeftMouseDown(evt);
    }

    public override void Update(GameTime gameTime)
    {
        UISystem.IsHoveringOnEditableText = true;

        _oldPressedKeys = _pressedKeys;
        _pressedKeys = PlayerInput.GetPressedKeys();

        _cursorBlinkCount++;
        _cursorBlinkCount %= 40;

        base.Update(gameTime);

        // UISystem里的不是你，但是你还在写？？？
        // 这一般是哪里代码有问题，就出错了，这里加判断是为了保险
        if (UISystem.FocusedEditableText != this && IsWritingText)
            ToggleTakingText();

        // 正在编辑文本
        if (!IsWritingText)
            return;

        // 可以占用键盘
        PlayerInput.WritingText = true;
        // 当前输入文本接管者覆盖
        Main.CurrentInputTextTakerOverride = this;

        // 按住Shift, Ctrl, Alt使用快捷键
        HandleShortcutOperation(out bool usingShortcut);
        if (usingShortcut)
            return;

        // 非快捷键操作，能算进原版的keyInt数组的（退格、Enter、Esc）
        HandleRegularOperation();

        // 非快捷键操作，不能算进原版的keyInt数组的（删除、左右移动）
        HandleSpecialOperation();

        // 正常输入
        string inputText = GetInputText();

        // 清除输入缓冲区
        Main.keyCount = 0;

        if (string.IsNullOrEmpty(inputText))
            return;

        // 写入文本
        Write(inputText);
    }

    public void ToggleTakingText()
    {
        IsWritingText = !IsWritingText;

        if (UISystem.FocusedEditableText == this && !IsWritingText)
            UISystem.FocusedEditableText = null;
        if (IsWritingText)
            UISystem.FocusedEditableText = this;

        Cursor = Text.Length;
        _cursorBlinkCount = 0;
        Main.clrInput();
        if (IsWritingText)
            this.StartTakingInput?.Invoke();
        else
            this.EndTakingInput?.Invoke();
    }

    #region 基本操作方法

    public void Write(string text)
    {
        SetText(Text.Insert(Cursor, text));
        Cursor += text.Length;
    }

    public void SetText(string text)
    {
        Text = text;
    }

    public void Delete()
    {
        if (Cursor != Text.Length)
            Text = Text[..(Cursor)] + Text[(Cursor + 1)..];
    }

    public void Backspace()
    {
        if (Cursor == 0)
            return;

        int curCursor = Cursor;
        Cursor--;
        Text = Text[..(curCursor - 1)] + Text[curCursor..];
    }

    public void CursorLeftest()
    {
        Cursor = 0;
        _cursorBlinkCount = 0;
    }

    public void CursorLeft()
    {
        if (Cursor != 0)
            Cursor--;
        _cursorBlinkCount = 0;
    }

    public void CursorRightest()
    {
        Cursor = Text.Length;
        _cursorBlinkCount = 0;
    }

    public void CursorRight()
    {
        if (Cursor < Text.Length)
            Cursor++;
        _cursorBlinkCount = 0;
    }

    #endregion

    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        InnerText.Cursor = _cursor;

        // 没在写字，或者强制不显示指针
        if (!IsWritingText || !ShowInputTicker)
        {
            InnerText.CursorOpacity = 0f;
        }
        // 下面是一个带淡入淡出的鼠标指针
        else
        {
            // Opacity: 1        |   Time: 0 -> 14
            if (_cursorBlinkCount is > 0 and < 14)
                InnerText.CursorOpacity = 1f;

            // Opacity: 1 -> 0   |   Time: 14 -> 20
            if (_cursorBlinkCount is >= 14 and <= 20)
                InnerText.CursorOpacity = TrUtils.GetLerpValue(20, 14, _cursorBlinkCount);

            // Opacity: 0        |   Time: 20 -> 34
            if (_cursorBlinkCount is > 20 and < 34)
                InnerText.CursorOpacity = 0f;

            // Opacity: 0 -> 1   |   Time: 34 -> 40(0)
            if (_cursorBlinkCount is >= 37 and <= 40)
                InnerText.CursorOpacity = TrUtils.GetLerpValue(34, 40, _cursorBlinkCount);
        }

        base.DrawChildren(spriteBatch);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (!IsWritingText)
            return;
        PlayerInput.WritingText = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (!IsWritingText)
            return;

        Main.instance.HandleIME();
        if (!ShowImePanel)
            return;

        Vector2 position = InnerText.GetDimensions().ToRectangle().Bottom();
        position.Y += 32f;
        Main.instance.DrawWindowsIMEPanel(position, 0.5f);
    }

    private void HandleShortcutOperation(out bool usingShortcut)
    {
        usingShortcut = false;

        if ((_pressedKeys.Any(k => k is Keys.LeftControl or Keys.RightControl) &&
             _pressedKeys.All(k => k is not Keys.LeftAlt and not Keys.RightAlt)))
        {
            usingShortcut = true;
            if (Pressed(Keys.Z))
            {
                Text = "";
            }
            else if (Pressed(Keys.X))
            {
                Platform.Get<IClipboard>().Value = Text;
                Text = "";
            }
            else if (Pressed(Keys.C))
            {
                Platform.Get<IClipboard>().Value = Text;
            }
            else if (Pressed(Keys.V))
            {
                Write(GetPasteText(true));
            }
        }
        else if (_pressedKeys.Any(k => k is Keys.LeftShift or Keys.RightShift))
        {
            if (Pressed(Keys.Delete))
            {
                Platform.Get<IClipboard>().Value = Text;
                Text = "";
            }

            if (Pressed(Keys.Insert))
                Write(GetPasteText(true));
        }
    }

    private void HandleRegularOperation()
    {
        for (int i = 0; i < Main.keyCount; i++)
        {
            int input = Main.keyInt[i];
            switch (input)
            {
                case (int) Keys.Enter:
                    ToggleTakingText();
                    break;
                case (int) Keys.Escape:
                    ToggleTakingText();
                    CanceledTakingInput?.Invoke();
                    break;
                case (int) Keys.Back:
                    Backspace();
                    break;
            }
        }
    }

    private void HandleSpecialOperation()
    {
        HandleKeyHoldFor(Keys.Left, CursorLeft);
        HandleKeyHoldFor(Keys.Right, CursorRight);
        HandleKeyHoldFor(Keys.Delete, Delete);
        if (Pressed(Keys.Up)) CursorLeftest();
        if (Pressed(Keys.Down)) CursorRightest();

        return;

        void HandleKeyHoldFor(Keys key, Action action)
        {
            if (!_pressedKeys.Contains(key))
                return;

            int keyInt = (int)key;
            ref int timer = ref _keyHoldTimer[keyInt];
            // 按住了
            timer++;
            // 这帧刚按下
            if (Pressed(key))
                timer = 0;

            int supposedHoldTime = 30; // 按下持续多长时间才算作长按
            bool longPress = timer > supposedHoldTime && timer % 2 == 0;
            if (timer == 0 || longPress)
                action.Invoke();
        }
    }

    public string GetInputText()
    {
        string finalText = "";

        for (int i = 0; i < Main.keyCount; i++)
        {
            int num = Main.keyInt[i];
            string key = Main.keyString[i];
            switch (num)
            {
                case >= (int) Keys.Space when num != (int) Keys.F16:
                    finalText += key;
                    break;
            }
        }

        return finalText;
    }

    private static string GetPasteText(bool allowMultiLine) => allowMultiLine
        ? Platform.Get<IClipboard>().MultiLineValue
        : Platform.Get<IClipboard>().Value;

    private static bool Pressed(Keys key) => _pressedKeys.Contains(key) && !_oldPressedKeys.Contains(key);
}