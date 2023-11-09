using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.PlayerProperty.Elements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerProperty;

public class PlayerPropertyGUI : ViewBody
{
    private static bool _visible;
    public override bool Display { get => Visible; set => Visible = value; }

    public static bool Visible
    {
        get => UIConfigs.Instance.PlyInfo switch
        {
            UIConfigs.PAPDisplayMode.AlwaysDisplayed => _visible,
            UIConfigs.PAPDisplayMode.WhenOpeningBackpack => Main.playerInventory && _visible,
            UIConfigs.PAPDisplayMode.NotDisplayed => false,
            _ => false
        };
        set => _visible = value;
    }

    private static bool _expanded;
    public static Player LocalPlayer;

    private static readonly Vector2
        _inventoryOpenPos = new Vector2(480, 25),
        _inventoryClosePos = new Vector2(590, 25);

    private AnimationTimer _invTimer, _openTimer;

    public SUIPanel Window;
    public SUIButton TitleButton;
    public View CardsView;

    public override void OnInitialize()
    {
        LocalPlayer = Main.LocalPlayer;

        if (float.TryParse(Key2HJSON("CardWidth"), out float width))
        {
            PlayerPropertyCard.DefaultWidth = width;
        }

        _invTimer = new AnimationTimer();
        _openTimer = new AnimationTimer()
        {
            State = AnimationState.CompleteClose,
            Timer = 0
        };

        Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
        {
            Shaded = true,
            ShadowThickness = UIColor.ShadowThicknessThinner
        };
        Window.Opacity.Type = OpacityType.Self;
        Window.SetPadding(0);
        Window.Join(this);

        TitleButton = new SUIButton(Key2HJSON("Name"))
        {
            Opacity = { Type = OpacityType.Self },
            DragIgnore = true,
            TextAlign = { X = 0.5f, Y = 0.5f },
            Width = new StyleDimension(0f, 1f),
            Relative = RelativeMode.Horizontal,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
            BeginBgColor = UIColor.TitleBg2 * 0.75f,
            EndBgColor = UIColor.TitleBg2,
            TextHasBorder = false
        };

        TitleButton.OnLeftMouseDown += (_, _) =>
        {
            _expanded = !_expanded;
            CardsView.OverflowHidden = true;
        };
        TitleButton.Join(Window);

        CardsView = new View()
        {
            Relative = RelativeMode.Vertical,
            OverflowHidden = true
        };
        CardsView.SetPadding(10, 7, 10, 10);

        #region 近战卡片
        // 创建卡片
        var card = CreateCard("近战属性", "UI/PlayerInfo/Melee2", out _, out _, out _);
        card.Width.Pixels = 160f;
        card.Height.Pixels = card.VPadding() + ((30 + 4) * 5 - 4);
        card.Join(CardsView);

        // 近战伤害
        var md = new PlayerPropertyCard(Key2HJSON("Damage"),
            () => $"{Math.Round(GetDamage(DamageClass.Melee), 2)}%");
        md.Join(card);

        // 近战暴击
        md = new PlayerPropertyCard(Key2HJSON("Crit"),
            () => $"{Math.Round(GetCrit(DamageClass.Melee), 2)}%");
        md.Join(card);

        // 近战速度
        md = new PlayerPropertyCard(Key2HJSON("Speed"),
            () => $"{MathF.Round(LocalPlayer.GetAttackSpeed(DamageClass.Melee) * 100f - 100f, 2)}%");
        md.Join(card);

        // 近战穿甲
        md = new PlayerPropertyCard(Key2HJSON("ArmorPenetration"),
            () => $"{MathF.Round(LocalPlayer.GetTotalArmorPenetration(DamageClass.Melee), 2)}");
        md.Join(card);
        #endregion

        #region 远程卡片
        card = CreateCard("远程属性", "UI/PlayerInfo/Ranged2", out _, out _, out _);
        card.Width.Pixels = 160f;
        card.Height.Pixels = card.VPadding() + ((30 + 4) * 4 - 4);
        card.Join(CardsView);

        // 远程伤害
        new PlayerPropertyCard(Key2HJSON("Damage"),
            () => $"{Math.Round(GetDamage(DamageClass.Ranged), 2)}%").Join(card);

        // 远程暴击
        new PlayerPropertyCard(Key2HJSON("Crit"),
            () => $"{Math.Round(GetCrit(DamageClass.Ranged), 2)}%").Join(card);

        // 远程穿甲
        md = new PlayerPropertyCard(Key2HJSON("ArmorPenetration"),
            () => $"{MathF.Round(LocalPlayer.GetTotalArmorPenetration(DamageClass.Ranged), 2)}");
        md.Join(card);
        #endregion

        #region 法术卡片
        card = CreateCard("魔法属性", "UI/PlayerInfo/Magic", out _, out _, out _);
        card.Width.Pixels = 160f;
        card.Height.Pixels = card.VPadding() + ((30 + 4) * 6 - 4);
        card.Join(CardsView);

        // 法术伤害
        new PlayerPropertyCard(Key2HJSON("Damage"),
            () => $"{Math.Round(GetDamage(DamageClass.Magic), 2)}%").Join(card);

        // 法术暴击
        new PlayerPropertyCard(Key2HJSON("Crit"),
            () => $"{Math.Round(GetCrit(DamageClass.Magic), 2)}%").Join(card);

        // 法术回复
        new PlayerPropertyCard(Key2HJSON("Regen"),
            () => $"{LocalPlayer.manaRegen / 2f}/s").Join(card);

        // 法术消耗减免
        new PlayerPropertyCard(Key2HJSON("Cost"),
            () => $"{MathF.Round(LocalPlayer.manaCost * 100f, 2)}%").Join(card);

        // 法术穿甲
        md = new PlayerPropertyCard(Key2HJSON("ArmorPenetration"),
            () => $"{MathF.Round(LocalPlayer.GetTotalArmorPenetration(DamageClass.Magic), 2)}");
        md.Join(card);
        #endregion

        #region 召唤卡片
        card = CreateCard("召唤属性", "UI/PlayerInfo/Summon", out _, out _, out _);
        card.Width.Pixels = 160f;
        card.Height.Pixels = card.VPadding() + ((30 + 4) * 5 - 4);
        card.Join(CardsView);

        // 召唤伤害
        new PlayerPropertyCard(Key2HJSON("Damage"),
            () => $"{Math.Round(GetDamage(DamageClass.Summon), 2)}%").Join(card);

        // 召唤栏
        new PlayerPropertyCard(Key2HJSON("MaxMinions"),
            () => $"{LocalPlayer.slotsMinions}/{LocalPlayer.maxMinions}").Join(card);

        // 哨兵栏
        new PlayerPropertyCard(Key2HJSON("MaxTurrets"),
            () => $"{Main.projectile.Count(proj => proj.active && proj.owner == LocalPlayer.whoAmI && proj.WipableTurret)}/{LocalPlayer.maxTurrets}")
            .Join(card);

        // 召唤穿甲
        md = new PlayerPropertyCard(Key2HJSON("ArmorPenetration"),
            () => $"{MathF.Round(LocalPlayer.GetTotalArmorPenetration(DamageClass.Summon), 2)}");
        md.Join(card);
        #endregion

        #region 未分类
        card = CreateCard("杂项", "UI/PlayerInfo/Luck", out _, out _, out _);
        card.Width.Pixels = 160f;
        card.Height.Pixels = card.VPadding() + ((30 + 4) * 6 - 4);
        card.Join(CardsView);

        // 生命回复
        new PlayerPropertyCard(Key2HJSON("LifeRegen"),
            () => $"{LocalPlayer.lifeRegen / 2f}/s").Join(card);

        // 免伤
        new PlayerPropertyCard(Key2HJSON("Endurance"),
            () => $"{MathF.Round(LocalPlayer.endurance * 100f, 2)}%").Join(card);

        // 仇恨
        new PlayerPropertyCard(Key2HJSON("Aggro"),
            () => $"{LocalPlayer.aggro}").Join(card);

        // 幸运
        new PlayerPropertyCard(Key2HJSON("Luck"),
            () => $"{Math.Round(LocalPlayer.luck, 2)}").Join(card);

        // 渔力
        new PlayerPropertyCard(Key2HJSON("FishingSkill"),
            () => $"{LocalPlayer.fishingSkill}").Join(card);
        #endregion

        float maxHeight = 0f;

        foreach (var item in CardsView.Children)
        {
            if (item.Height.Pixels > maxHeight)
            {
                maxHeight = item.Height.Pixels;
            }
        }

        CardsView.SetInnerPixels((160 + 4) * 5 - 4, maxHeight);
        CardsView.Join(Window);

        // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
        // new PlyInfoCard(GetHJSON("WingTimeMax"),
        //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);

        _openTimer.OnOpened += () => { CardsView.OverflowHidden = false; };
    }

    /// <summary>
    /// 创建一个卡片
    /// </summary>
    public static View CreateCard(string name, string iconPath,
        out View tb, out SUIImage ig, out SUITitle st)
    {
        // 大容器
        View card = new()
        {
            Rounded = new Vector4(10f),
            Border = 2f,
            BorderColor = UIColor.PanelBorder,
            Relative = RelativeMode.Horizontal,
            Spacing = new Vector2(4f)
        };
        card.SetPadding(5);

        // 标题容器
        tb = new()
        {
            Rounded = new Vector4(8f),
            BgColor = UIColor.TitleBg2
        };
        tb.Width.Percent = 1f;
        tb.Height.Pixels = 30f;
        tb.SetPadding(8f, 0f);
        tb.Join(card);

        // 图标
        ig = new SUIImage(GetTexture(iconPath).Value)
        {
            ImagePercent = new Vector2(0.5f),
            ImageOrigin = new Vector2(0.5f),
            ImageScale = 0.8f
        };
        ig.Width.Pixels = 20f;
        ig.Height.Percent = 1f;
        ig.Join(tb);

        // 标题文字
        st = new(name, 0.36f)
        {
            HAlign = 0.5f
        };
        st.Height.Percent = 1f;
        st.Width.Pixels = st.TextSize.X;
        st.Join(tb);

        return card;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
            Main.LocalPlayer.mouseInterface = true;
        }

        // 动画开/关
        if (Main.playerInventory)
            _invTimer.Open();
        else
            _invTimer.Close();
        if (_expanded)
            _openTimer.Open();
        else
            _openTimer.Close();
        _invTimer.Update();
        _openTimer.Update();

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        bool recalculate = false;

        if (Opacity.Value != _openTimer.Schedule &&
            Window.Opacity.SetValue(_openTimer.Schedule))
        {
            recalculate = true;
        }

        Window.ShadowColor = Window.BorderColor * _openTimer.Lerp(0.25f, 0.5f);


        float maxHeight = 0f;

        foreach (var item in CardsView.Children)
        {
            if (item.Height.Pixels > maxHeight)
            {
                maxHeight = item.Height.Pixels;
            }
        }

        float cardHeight = _openTimer.Lerp(0, maxHeight + CardsView.VPadding());
        if (Math.Abs(CardsView.Height.Pixels - cardHeight) > 0)
        {
            CardsView.Height.Pixels = cardHeight;
            recalculate = true;
        }

        float round = _openTimer.Lerp(10f, 0);
        TitleButton.Rounded.Z = TitleButton.Rounded.W = round;

        Vector2 mainPanelSize = new Vector2(_openTimer.Lerp(TitleButton.HPadding() + TitleButton.TextSize.X, CardsView.Width.Pixels), TitleButton.Height.Pixels + CardsView.Height.Pixels);
        if (Window.GetInnerSizePixels() != mainPanelSize)
        {
            Window.SetInnerPixels(mainPanelSize);
            recalculate = true;
        }

        Vector2 endPos = _invTimer.Lerp(_inventoryOpenPos, _inventoryClosePos);

        if (Window.GetPosPixel() != endPos)
        {
            Window.SetPosPixels(endPos);
            recalculate = true;
        }

        if (recalculate)
            Window.Recalculate();
    }

    private static float GetDamage(DamageClass damageClass) =>
        MathF.Round((LocalPlayer.GetTotalDamage(damageClass).Additive - 1) * 100f);

    private static float GetCrit(DamageClass damageClass) => LocalPlayer.GetTotalCritChance(damageClass);
    private static string Key2HJSON(string str) => GetText($"UI.PlayerInfo.{str}");

    public override bool CanPriority(UIElement target) => target != this;

    public override bool CanDisableMouse(UIElement target)
    {
        return target != this && Window.IsMouseHovering || Window.KeepPressed;
    }
}