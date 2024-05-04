using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.UIFramework.SUIElements;

public class SUISearchBar : View
{
    public class CustomSearchBar : UIElement
    {
        public readonly LocalizedText _textToShowWhenEmpty;
        public UITextBox _text;
        public string actualContents;
        public float _textScale;
        public bool isWritingText;
        public bool ShowImePanel = true;

        public bool HasContents => !string.IsNullOrWhiteSpace(this.actualContents);

        public bool IsWritingText => this.isWritingText;

        public event Action<string> OnContentsChanged;

        public event Action OnStartTakingInput;

        public event Action OnEndTakingInput;

        public event Action OnCanceledTakingInput;

        public event Action OnNeedingVirtualKeyboard;

        public CustomSearchBar(LocalizedText emptyContentText, float scale)
        {
            this._textToShowWhenEmpty = emptyContentText;
            this._textScale = scale;
            UITextBox uiTextBox = new UITextBox("", scale);
            uiTextBox.HAlign = 0.0f;
            uiTextBox.VAlign = 0.5f;
            uiTextBox.BackgroundColor = Color.Transparent;
            uiTextBox.BorderColor = Color.Transparent;
            uiTextBox.Width = new StyleDimension(0.0f, 1f);
            uiTextBox.Height = new StyleDimension(0.0f, 1f);
            uiTextBox.TextHAlign = 0.0f;
            uiTextBox.ShowInputTicker = false;
            UITextBox element = uiTextBox;
            element.SetTextMaxLength(50);
            this.Append((UIElement)element);
            this._text = element;
        }

        public void SetContents(string contents, bool forced = false)
        {
            if (!(!(this.actualContents == contents) | forced))
                return;
            this.actualContents = contents;
            if (string.IsNullOrEmpty(this.actualContents))
            {
                this._text.TextColor = Color.Gray;
                this._text.SetText(this._textToShowWhenEmpty.Value, this._textScale, false);
            }
            else
            {
                this._text.TextColor = Color.White;
                this._text.SetText(this.actualContents);
                this.actualContents = this._text.Text;
            }

            this.TrimDisplayIfOverElementDimensions(0);
            if (this.OnContentsChanged == null)
                return;
            this.OnContentsChanged(contents);
        }

        public void TrimDisplayIfOverElementDimensions(int padding)
        {
            CalculatedStyle dimensions1 = this.GetDimensions();
            if ((double)dimensions1.Width == 0.0 && (double)dimensions1.Height == 0.0)
                return;
            Point point1 = new Point((int)dimensions1.X, (int)dimensions1.Y);
            Point point2 = new Point(point1.X + (int)dimensions1.Width, point1.Y + (int)dimensions1.Height);
            Rectangle rectangle1 = new Rectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y);
            CalculatedStyle dimensions2 = this._text.GetDimensions();
            Point point3 = new Point((int)dimensions2.X, (int)dimensions2.Y);
            Point point4 = new Point(point3.X + (int)this._text.MinWidth.Pixels,
                point3.Y + (int)this._text.MinHeight.Pixels);
            Rectangle rectangle2 = new Rectangle(point3.X, point3.Y, point4.X - point3.X, point4.Y - point3.Y);
            int num = 0;
            while (rectangle2.Right > rectangle1.Right - padding && this._text.Text.Length > 0)
            {
                this._text.SetText(this._text.Text.Substring(0, this._text.Text.Length - 1));
                ++num;
                this.RecalculateChildren();
                CalculatedStyle dimensions3 = this._text.GetDimensions();
                point3 = new Point((int)dimensions3.X, (int)dimensions3.Y);
                point4 = new Point(point3.X + (int)this._text.MinWidth.Pixels,
                    point3.Y + (int)this._text.MinHeight.Pixels);
                rectangle2 = new Rectangle(point3.X, point3.Y, point4.X - point3.X, point4.Y - point3.Y);
                this.actualContents = this._text.Text;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt) => base.LeftMouseDown(evt);

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsWritingText)
            {
                if (this.NeedsVirtualkeyboard())
                {
                    if (this.OnNeedingVirtualKeyboard == null)
                        return;
                    this.OnNeedingVirtualKeyboard();
                    return;
                }

                PlayerInput.WritingText = true;
                Main.CurrentInputTextTakerOverride = (object)this;
            }

            base.Update(gameTime);

            if (!IsWritingText)
                return;

            string inputText = Main.GetInputText(this.actualContents);
            if (Main.inputTextEnter)
                this.ToggleTakingText();
            else if (Main.inputTextEscape)
            {
                this.ToggleTakingText();
                if (this.OnCanceledTakingInput != null)
                    this.OnCanceledTakingInput();
            }

            this.SetContents(inputText);
        }

        public bool NeedsVirtualkeyboard() => PlayerInput.SettingsForUI.ShowGamepadHints;

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (!this.isWritingText)
                return;
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();
            Vector2 position = new Vector2((float)(Main.screenWidth / 2),
                (float)(this._text.GetDimensions().ToRectangle().Bottom + 32));
            if (ShowImePanel)
                Main.instance.DrawWindowsIMEPanel(position, 0.5f);
        }

        public void ToggleTakingText()
        {
            this.isWritingText = !this.isWritingText;
            this._text.ShowInputTicker = this.isWritingText;
            Main.clrInput();
            if (this.isWritingText)
            {
                if (this.OnStartTakingInput == null)
                    return;
                this.OnStartTakingInput();
            }
            else
            {
                if (this.OnEndTakingInput == null)
                    return;
                this.OnEndTakingInput();
            }
        }
    }

    public event Action<string> OnSearchContentsChanged;
    public event Action OnDraw;

    public bool IsSearchButtonMouseHovering => _searchBtn.IsMouseHovering;

    public bool IsWritingText => _searchBar.IsWritingText;

    /// <summary> 在搜索栏加一段 (支持拼音及拼音首字母搜索) 的文本 </summary>
    private readonly bool _pinyinSearchTip;

    public string SearchContent { get; private set; }
    public bool Visible = true;
    private CustomSearchBar _searchBar;
    private SUIPanel _searchBoxPanel;
    private UIImageButton _searchBtn;
    private bool _didClickSomething;
    private bool _didClickSearchBar;

    public SUISearchBar(bool pinyinSearchTip = false, bool showIme = true)
    {
        _pinyinSearchTip = pinyinSearchTip;
        Height = new StyleDimension(28f, 0f);
        this.SetPadding(0f);
        AddSearchBar();
        _searchBar.SetContents(null, forced: true);
        _searchBar.ShowImePanel = showIme;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Visible)
        {
            base.Draw(spriteBatch);
            OnDraw?.Invoke();
        }
    }

    private void AddSearchBar()
    {
        _searchBtn = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search"))
        {
            VAlign = 0.5f,
            HAlign = 0f
        };
        _searchBtn.OnLeftMouseDown += Click_SearchArea;
        _searchBtn.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border"));
        _searchBtn.SetVisibility(1f, 1f);
        Append(_searchBtn);

        _searchBoxPanel = new SUIPanel(UIStyle.SearchBarBorder, UIStyle.SearchBarBg)
        {
            Left = new StyleDimension(4f, 0f),
            Width = new StyleDimension(-_searchBtn.Width.Pixels - 3f, 1f),
            Height = new StyleDimension(0f, 1f),
            VAlign = 0.5f,
            HAlign = 1f
        };
        _searchBoxPanel.OnLeftMouseDown += Click_SearchArea;
        _searchBoxPanel.SetPadding(0f);
        Append(_searchBoxPanel);

        var searchBarText = Language.GetText("UI.PlayerNameSlot");
        if (Language.ActiveCulture.Name is "zh-Hans" && _pinyinSearchTip)
        {
            // 由于在这不能直接访问 new LocalizedText，我们借助 Language.GetText 来获取
            searchBarText = Language.GetText("搜索名称(支持拼音或首字母): ");
        }

        _searchBar = new CustomSearchBar(searchBarText, 0.8f)
        {
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(0f, 1f),
            HAlign = 0f,
            VAlign = 0.5f,
            IgnoresMouseInteraction = true
        };
        _searchBar.OnContentsChanged += (s =>
        {
            SearchContent = s;
            OnSearchContentsChanged?.Invoke(s);
        });
        _searchBar.OnStartTakingInput += OnStartTakingInput;
        _searchBar.OnEndTakingInput += OnEndTakingInput;
        _searchBar.OnCanceledTakingInput += OnCanceledInput;
        _searchBoxPanel.Append(_searchBar);

        UIImageButton cancelBtn = new(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel"))
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = new StyleDimension(-2f, 0f)
        };
        cancelBtn.OnMouseOver += SearchCancelButton_OnMouseOver;
        cancelBtn.OnLeftClick += SearchCancelButton_OnClick;
        _searchBoxPanel.Append(cancelBtn);
    }

    private void SearchCancelButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (_searchBar.HasContents)
        {
            _searchBar.SetContents(null, forced: true);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        else
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private void SearchCancelButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private void OnCanceledInput()
    {
        Main.LocalPlayer.ToggleInv();
    }

    private void Click_SearchArea(UIMouseEvent evt, UIElement listeningElement)
    {
        if (evt.Target.Parent != _searchBoxPanel)
        {
            _searchBar.ToggleTakingText();
            _didClickSearchBar = true;
        }
    }

    private void OnStartTakingInput()
    {
        _searchBoxPanel.BorderColor = UIStyle.SearchBarBorderSelected;
    }

    private void OnEndTakingInput()
    {
        _searchBoxPanel.BorderColor = UIStyle.SearchBarBorder;
    }

    public void AttemptStoppingUsingSearchbar()
    {
        if (!_didClickSearchBar && _searchBar.IsWritingText)
            _searchBar.ToggleTakingText();
    }

    public void ClearContents()
    {
        _searchBar.SetContents(null, forced: true);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Visible) return;

        base.Update(gameTime);

        if (_didClickSomething && !_didClickSearchBar && _searchBar.IsWritingText)
            _searchBar.ToggleTakingText();

        _didClickSomething = false;
        _didClickSearchBar = false;
    }
}