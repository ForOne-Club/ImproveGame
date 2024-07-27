﻿using ImproveGame.UI.ModernConfig.OfficialPresets;
using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UI.ModernConfig.OptionElements.PresetElements;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using PinyinNet;
using Terraria.GameInput;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig;

public sealed class ConfigOptionsPanel : SUIPanel
{
    internal static ConfigOptionsPanel Instance;
    public bool ShouldHideSearchBar;
    public bool DelayRefreshCurrentPage;
    private static Category _currentCategory;
    public static Category CategoryToSelectOnOpen;

    private HashSet<string> _addedOptions = [];
    private List<ModernConfigOption> _allOptions = [];
    private SUIEditableText _searchBar { get;  set; }
    private SUIScrollView2 _options { get;  set; }
    public SUIDropdownListContainer DropdownList { get;  set; }

    public static Category CurrentCategory
    {
        get => _currentCategory;
        set
        {
            if (_currentCategory != value)
            {
                _currentCategory = value;
                Instance._options.ScrollBar.TargetScrollPosition = Vector2.Zero;
                Instance._options.ScrollBar.CurrentScrollPosition = Vector2.Zero;
                Instance.RefreshCurrentPage();
            }
        }
    }

    public ConfigOptionsPanel(Color color) : base(color, color)
    {
        const int searchBarHeight = 30;
        const int gap = 6;
        Instance = this;

        _searchBar = new SUIEditableText
        {
            RelativeMode = RelativeMode.Vertical,
            BgColor = UIStyle.SearchBarBg,
            BorderColor = UIStyle.SearchBarBorder,
            Rounded = new Vector4(10f),
            MaxLength = 30,
            VAlign = 0.5f,
            OverflowHidden = true
        };
        _searchBar.OnUpdate += element =>
        {
            var view = (SUIEditableText)element;
            view.BorderColor = view.IsMouseHovering ? UIStyle.SearchBarBorderSelected : UIStyle.SearchBarBorder;
            view.BgColor = UIStyle.SearchBarBg;

            switch (ShouldHideSearchBar)
            {
                case true when _searchBar.Height.Pixels != 0:
                    _searchBar.SetSize(0f, 0f, 1f);
                    Recalculate();
                    break;
                case false when _searchBar.Height.Pixels == 0:
                    _searchBar.SetSize(0f, searchBarHeight, 1f);
                    Recalculate();
                    break;
            }
        };
        _searchBar.ContentsChanged += SearchBarTextChanged;
        _searchBar.InnerText.TextOffset.X = 6f;
        _searchBar.InnerText.Placeholder = GetText("Search");
        _searchBar.SetSize(0f, searchBarHeight, 1f);
        _searchBar.JoinParent(this);

        _options = new SUIScrollView2(Orientation.Vertical)
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(gap)
        };
        _options.SetPadding(0f, 0f);
        _options.SetSize(0f, -searchBarHeight - gap, 1f, 1f);
        _options.JoinParent(this);

        DropdownList = new SUIDropdownListContainer();
        DropdownList.JoinParent(this);
    }

    public void RefreshCurrentPage()
    {
        _options.ListView.RemoveAllChildren();
        _allOptions.Clear();
        _addedOptions.Clear();
        ShouldHideSearchBar = false;
        _currentCategory.AddOptions(this);
        string text = _searchBar.Text;
        SearchBarTextChanged(ref text);
        Recalculate();
    }

    private void SearchBarTextChanged(ref string text)
    {
        DropdownList.Enabled = false;
        if (ShouldHideSearchBar)
            return;

        if (string.IsNullOrEmpty(text))
        {
            // 刷一下就走
            _options.ListView.RemoveAllChildren();
            _allOptions.ForEach(o =>
            {
                o.Highlighted = false;
                o.JoinParent(_options.ListView);
            });
            Recalculate();
            return;
        }

        // 找出所有匹配的选项，并着色（设置Highlighted）
        var allOptions = _allOptions.ToList(); // ToList() 防止遍历时修改列表
        var matchedOptions = new List<ModernConfigOption>();
        string searchContent = RemoveSpaces(text.ToLower());
        foreach (var option in allOptions)
        {
            string name = option.Label;
            string pinyin = RemoveSpaces(PinyinConvert.GetPinyinForAutoComplete(name));
            bool labelMatched = name.Contains(searchContent);
            bool pinyinMatched = Language.ActiveCulture.Name is "zh-Hans" && pinyin.Contains(searchContent);
            if (labelMatched || pinyinMatched)
            {
                option.Highlighted = true;
                matchedOptions.Add(option);
            }
            else
            {
                option.Highlighted = false;
            }
        }

        // 将所有匹配的选项排序到allOptions列表的最前面
        foreach (var option in matchedOptions)
        {
            allOptions.Remove(option);
            allOptions.Insert(0, option);
        }

        // 最后重新加入到_options
        _options.ListView.RemoveAllChildren();
        allOptions.ForEach(o => o.JoinParent(_options.ListView));
        Recalculate();
    }

    public void AddToggle(ModConfig config, string name) => AddToAllOptions<OptionToggle>(config, name);

    public void AddValueSlider(ModConfig config, string name) => AddToAllOptions<OptionSlider>(config, name);

    public void AddValueText(ModConfig config, string name) => AddToAllOptions<OptionNumber>(config, name);

    public void AddEnum(ModConfig config, string name) => AddToAllOptions<OptionDropdownList>(config, name);

    private void AddToAllOptions<T>(ModConfig config, string name) where T : ModernConfigOption
    {
        // 如果已经添加过这个选项，直接返回
        if (!_addedOptions.Add(name))
            return;
        // 如果当前分类不允许添加这个选项，直接返回
        if (!CurrentCategory.CanOptionBeAdded(config, name))
            return;
        // 创建实例并加入到_allOptions列表
        var instance = (ModernConfigOption)Activator.CreateInstance(typeof(T), config, name);
        _allOptions.Add(instance);
    }

    public void AddToOptionsDirect(View view)
    {
        view.JoinParent(_options.ListView);
    }

    public void AddOfficialPreset<T>() where T : OfficialPreset
    {
        var preset = Activator.CreateInstance<T>();
        AddToOptionsDirect(new OfficialPresetElement(preset.Label, preset.Tooltip, preset.Link, preset.OnApply));
    }

    public override void Update(GameTime gameTime)
    {
        if (DelayRefreshCurrentPage)
        {
            DelayRefreshCurrentPage = false;
            RefreshCurrentPage();
        }

        base.Update(gameTime);

        if (IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Modern Config UI");
        if (CategoryToSelectOnOpen is not null)
        {
            CurrentCategory = CategoryToSelectOnOpen;
            CategoryToSelectOnOpen = null;
        }
    }
}