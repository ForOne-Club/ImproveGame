using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class WeaponPage : View
{
    public Item SlotItem => _weaponSlot.Item;
    public WeaponSlot WeaponSlot => _weaponSlot;

    // 上半部分面板
    private View _upperPanel;

    // 下半部分面板
    private View _lowerPanel;

    private WeaponSlot _weaponSlot;
    public SUIScrollView2 CurrentPreview { get; set; }
    public SUIScrollView2 Presets { get; private set; }

    private PresetComponent _hoveredPreset;

    private int _checkChainsCounter; // 每30帧检测一下有没有新的弹药链

    public WeaponPage()
    {
        int midPixel = 100;
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
        _lowerPanel.SetPadding(14f, 10, 2f, 20f);
        _lowerPanel.SetSize(0f, -midPixel, 1f, 1f);
        _lowerPanel.JoinParent(this);
        SetupLowerPanel();
    }

    private void SetupUpperPanel()
    {
        _weaponSlot = new WeaponSlot(this)
        {
            RelativeMode = RelativeMode.Vertical
        };
        _weaponSlot.JoinParent(_upperPanel);

        CurrentPreview = new SUIScrollView2(Orientation.Horizontal)
        {
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(4f)
        };
        CurrentPreview.SetPadding(2f, 0f);
        CurrentPreview.SetSize(-90f, 0f, 1f, 1f);
        CurrentPreview.JoinParent(_upperPanel);
        CurrentPreview.Append(new TipPutInItem(this));
        SetupPreview();
    }

    private void SetupLowerPanel()
    {
        Presets = new SUIScrollView2(Orientation.Vertical)
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(4f)
        };
        Presets.SetPadding(2f, 0f);
        Presets.SetSize(0f, 0f, 1f, 1f);
        Presets.JoinParent(_lowerPanel);
        ChainSaver.ReadAllAmmoChains();
        ReloadPresetsElement();
    }

    /// <summary>
    /// 重新加载 可用的 预设列表视图
    /// </summary>
    public void ReloadPresetsElement()
    {
        Presets.ListView.RemoveAllChildren();

        float length = 87;
        var size = new Vector2(length * 2 + 8, length);

        var addChain = new AddChainComponent(this);
        addChain.SetSize(size);
        addChain.JoinParent(Presets.ListView);

        foreach ((string path, AmmoChain ammoChain) in ChainSaver.AmmoChains)
        {
            string name = path.Split('\\').Last();
            name = name[..^ChainSaver.Extension.Length];

            var component = new PresetComponent(this, ammoChain, name)
            {
                RelativeMode = RelativeMode.Horizontal,
                Spacing = new Vector2(8f),
                PreventOverflow = true,
            };

            // if (i % HNumber == 0)
            //     component.DirectLineBreak = true;

            component.SetSize(size);
            component.JoinParent(Presets.ListView);
        }

        // var getHelp = new GetHelpComponent(this);
        // getHelp.SetSize(size);
        // getHelp.JoinParent(Presets.ListView);
        // var openFolder = new OpenFolderComponent(this);
        // openFolder.SetSize(size);
        // openFolder.JoinParent(Presets.ListView);

        Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // 鼠标滚轮锁定
        if (CurrentPreview.IsMouseHovering || Presets.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Ammo Chain UI");
            Main.LocalPlayer.mouseInterface = true;
        }

        // 检查有没有新东西
        _checkChainsCounter++;
        if (_checkChainsCounter >= 30)
        {
            int oldAmmoChainsCount = ChainSaver.AmmoChains.Count;
            ChainSaver.ReadAllAmmoChains();
            if (ChainSaver.AmmoChains.Count != oldAmmoChainsCount)
            {
                ReloadPresetsElement();
                SetupPreview();
            }
            _checkChainsCounter = 0;
        }

        // 检查鼠标悬停在哪个上面
        var oldHovered = _hoveredPreset;
        _hoveredPreset = null;
        foreach (var preset in from c in Presets.ListView.Children where c is PresetComponent
                 select c as PresetComponent)
        {
            if (preset.IsMouseHovering)
            {
                _hoveredPreset = preset;
                break;
            }
        }

        if (oldHovered != _hoveredPreset)
            SetupPreview();
    }

    public void SetupPreview()
    {
        CurrentPreview.ListView.RemoveAllChildren();

        Color slotBgColor = UIStyle.TrashSlotBg;
        List<AmmoChain.Ammo> chain;
        if (_hoveredPreset is not null)
            chain = _hoveredPreset.Chain.Chain;
        else if (!SlotItem.IsAir && SlotItem.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalItem))
        {
            globalItem.Chain ??= new AmmoChain();
            chain = globalItem.Chain.Chain;
            slotBgColor = UIStyle.ItemSlotBg * 0.8f;
        }
        else
            return;

        foreach ((ItemTypeData itemTypeData, int times) in chain)
        {
            var itemSlot = new ItemSlotForPreview(itemTypeData.Item.Clone())
            {
                RelativeMode = RelativeMode.Horizontal,
                Spacing = new Vector2(4),
                BgColor = slotBgColor,
                Item = {stack = times}
            };
            itemSlot.JoinParent(CurrentPreview.ListView);
        }

        Recalculate();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}