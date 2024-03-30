using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class ChainEditPage : View
{
    public bool ShouldResetCurrentChain;
    public int SelectedAmmoType;
    public string ChainName;
    public bool IsCreatingAChain;
    public AmmoChain EditingChain;

    // 上半部分面板
    private View _upperPanel;

    // 下半部分面板
    private View _lowerPanel;

    private SUIScrollView2 _currentChain { get; set; }
    public SUIScrollView2 _availableAmmos { get; private set; }

    public ChainEditPage()
    {
        int midPixel = 90;
        SetSizePercent(1f, 1f);

        _upperPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        _upperPanel.SetPadding(20, 10f, 20, 10f);
        _upperPanel.SetSize(0f, midPixel, 1f, 0f);
        _upperPanel.JoinParent(this);
        SetupUpperPanel();

        this.MakeHorizontalSeparator();

        _lowerPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        _lowerPanel.SetPadding(14f, 10, 14f, 20f);
        _lowerPanel.SetSize(0f, -midPixel, 1f, 1f);
        _lowerPanel.JoinParent(this);
        SetupLowerPanel();
    }

    private void SetupUpperPanel()
    {
    }

    private void SetupLowerPanel()
    {
        _currentChain = new SUIScrollView2(Orientation.Horizontal)
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(4f)
        };
        _currentChain.SetPadding(2f, 0f);
        _currentChain.SetSize(0f, 76f, 1f, 0f);
        _currentChain.JoinParent(_lowerPanel);
        EditingChain = new AmmoChain();
        SetupCurrentChain(EditingChain);

        _availableAmmos = new SUIScrollView2(Orientation.Horizontal)
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(4f)
        };
        _availableAmmos.SetPadding(2f, 0f);
        _availableAmmos.SetSize(0f, 150f, 1f, 0f);
        _availableAmmos.JoinParent(_lowerPanel);
        SetupAvailableAmmos();
        
        var saveButton = new SUIButton(GetText("UI.AmmoChain.Confirm"))
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(10f, 10f),
            TextAlign = new Vector2(0.5f)
        };
        saveButton.OnLeftMouseDown += ClickOnSaveButton;
        saveButton.SetSize(280f, 40f, 0f, 0f);
        saveButton.JoinParent(_lowerPanel);
        
        var cancelButton = new SUIButton(GetText("UI.AmmoChain.Cancel"))
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(10f, 0f),
            TextAlign = new Vector2(0.5f)
        };
        cancelButton.OnLeftMouseDown += ClickOnCancelButton;
        cancelButton.SetSize(280f, 40f, 0f, 0f);
        cancelButton.JoinParent(_lowerPanel);
    }

    private void ClickOnSaveButton(UIMouseEvent evt, UIElement listeningelement)
    {
        ChainSaver.SaveAsFile(EditingChain, ChainName);
        AmmoChainUI.Instance.GoToWeaponPage();
    }

    private void ClickOnCancelButton(UIMouseEvent evt, UIElement listeningelement)
    {
        AmmoChainUI.Instance.GoToWeaponPage();
    }

    /// <summary>
    /// 重新加载 可用的 预设列表视图
    /// </summary>
    public void SetupAvailableAmmos()
    {
        _availableAmmos.ListView.RemoveAllChildren();

        var items = ContentSamples.ItemsByType.Values.Where(i => i.IsAmmo()).ToList();
        items.Sort((a, b) => a.ammo.CompareTo(b.ammo));
        foreach (var itemSlot in items.Select(item => new SelectableAmmoSlot(item, this))) {
            itemSlot.JoinParent(_availableAmmos.ListView);
        }

        Recalculate();
    }

    public void SetupCurrentChain(AmmoChain chain)
    {
        _currentChain.ListView.RemoveAllChildren();

        if (chain.Chain.Count == 0)
        {
            _currentChain.ListView.Append(new TipClickToAdd(this));
            Recalculate();
            return;
        }

        new AmmoGapElement(0, this).JoinParent(_currentChain.ListView);

        for (var i = 0; i < chain.Chain.Count; i++)
        {
            var element = chain.Chain[i];
            var itemTypeData = element.ItemData;
            int times = element.Times;

            var itemSlot = new ItemSlotForEdit(i, this);
            itemSlot.AirItem.SetDefaults(itemTypeData.Item.type);
            itemSlot.AirItem.stack = times;
            itemSlot.JoinParent(_currentChain.ListView);

            new AmmoGapElement(i + 1, this).JoinParent(_currentChain.ListView);
        }

        Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (ShouldResetCurrentChain)
        {
            ShouldResetCurrentChain = false;
            SetupCurrentChain(EditingChain);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}