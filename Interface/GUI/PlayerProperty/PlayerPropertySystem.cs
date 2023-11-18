namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 抽象就是对世界的鞭挞
/// </summary>
public class PlayerPropertySystem : ModSystem
{
    public static PlayerPropertySystem Instance { get; private set; }

    public Dictionary<string, BasePropertyCategory> PropertyCategories { get; private set; } = new();

    public override void Load()
    {
        Instance = this;

        #region 近战属性

        BasePropertyCategory melee =
            new BasePropertyCategory(ModAsset.Melee.Value, "UI.PlayerProperty.Melee");

        // 近战伤害、暴击、攻速与穿甲
        AddDamageProperty(melee, DamageClass.Melee);
        AddCritProperty(melee, DamageClass.Melee);
        AddAttackSpeedProperty(melee, DamageClass.Melee);
        AddArmorPenetrationProperty(melee, DamageClass.Melee);

        #endregion

        #region 远程

        BasePropertyCategory ranged =
            new BasePropertyCategory(ModAsset.Ranged.Value, "UI.PlayerProperty.Ranged");

        // 远程伤害
        AddDamageProperty(ranged, DamageClass.Ranged);

        // 弹药伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.BulletDamage",
            () =>
            {
                var sm = Main.LocalPlayer.bulletDamage;
                return BonusSyntax((sm.Additive * sm.Multiplicative - 1f) * 100, true);
            }));

        // 弓箭伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.ArrowDamage",
            () =>
            {
                var sm = Main.LocalPlayer.arrowDamage;
                return BonusSyntax((sm.Additive * sm.Multiplicative - 1f) * 100, true);
            }));

        // 其他伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.SpecialistDamage",
            () =>
            {
                var sm = Main.LocalPlayer.specialistDamage;
                return BonusSyntax((sm.Additive * sm.Multiplicative - 1f) * 100, true);
            }));

        // 远程暴击与穿甲
        AddCritProperty(ranged, DamageClass.Ranged);
        AddArmorPenetrationProperty(ranged, DamageClass.Ranged);

        #endregion

        #region 魔法

        BasePropertyCategory magic =
            new BasePropertyCategory(ModAsset.Magic.Value, "UI.PlayerProperty.Magic");

        // 法术伤害与暴击
        AddDamageProperty(magic, DamageClass.Magic);
        AddCritProperty(magic, DamageClass.Magic);

        // 法术回复
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Regen",
            () => $"{Main.LocalPlayer.manaRegen / 2f}/s"));

        // 法术消耗减免
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Cost",
            () => BonusSyntax((Main.LocalPlayer.manaCost - 1f) * 100f, true)));

        // 法术穿甲
        AddArmorPenetrationProperty(magic, DamageClass.Magic);

        #endregion

        #region 召唤

        BasePropertyCategory summon =
            new BasePropertyCategory(ModAsset.Summon.Value, "UI.PlayerProperty.Summon");

        // 召唤伤害
        AddDamageProperty(summon, DamageClass.Summon);

        // 鞭子速度
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.SummonMeleeSpeed",
            () => BonusSyntax(Main.LocalPlayer.GetAttackSpeed(DamageClass.SummonMeleeSpeed) * 100f - 100f, true)));

        // 召唤穿甲
        AddArmorPenetrationProperty(summon, DamageClass.Summon);

        // 召唤栏
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.MaxMinions",
            () => $"{Main.LocalPlayer.slotsMinions}/{Main.LocalPlayer.maxMinions}"));

        // 哨兵栏
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.MaxTurrets",
            () =>
                $"{Main.projectile.Count(proj => proj.active && proj.owner == Main.LocalPlayer.whoAmI && proj.WipableTurret)}/{Main.LocalPlayer.maxTurrets}"));

        #endregion

        #region 投掷属性 (出于跨模组考虑)

        BasePropertyCategory throwing =
            new BasePropertyCategory(ModAsset.Throwing.Value, "UI.PlayerProperty.Throwing");

        // 投掷伤害、暴击、攻速与穿甲
        AddDamageProperty(throwing, DamageClass.Throwing);
        AddCritProperty(throwing, DamageClass.Throwing);
        AddAttackSpeedProperty(throwing, DamageClass.Throwing);
        AddArmorPenetrationProperty(throwing, DamageClass.Throwing);

        #endregion

        #region 其他

        BasePropertyCategory other =
            new BasePropertyCategory(GetTexture("UI/PlayerInfo/Luck").Value, "UI.PlayerProperty.Other");

        // 生命回复
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.LifeRegen",
            () => $"{Main.LocalPlayer.lifeRegen / 2f}/s"));

        // 免伤
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.Endurance",
            () => BonusSyntax(Main.LocalPlayer.endurance * 100f)));

        // 仇恨
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.Aggro",
            () => $"{Main.LocalPlayer.aggro}"));

        // 幸运
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.Luck",
            () => $"{Math.Round(Main.LocalPlayer.luck, 2)}"));

        #endregion

        PropertyCategories.Add("Melee", melee);
        PropertyCategories.Add("Ranged", ranged);
        PropertyCategories.Add("Magic", magic);
        PropertyCategories.Add("Summon", summon);
        PropertyCategories.Add("Throwing", throwing);
        PropertyCategories.Add("Other", other);

        // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
        // new PlyInfoCard(GetHJSON("WingTimeMax"),
        //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
    }

    public static void CalamityIntegration(Mod calamityMod)
    {
        if (!calamityMod.TryFind<DamageClass>("RogueDamageClass", out var damageClass))
            return;

        BasePropertyCategory rogue = new(ModAsset.Rogue.Value, "UI.PlayerProperty.CalamityMod.Rogue", false,
            ModContent.Request<Texture2D>("CalamityMod/icon_small", AssetRequestMode.ImmediateLoad).Value);

        AddDamageProperty(rogue, damageClass);
        AddCritProperty(rogue, damageClass);

        // 灾厄特有，盗贼弹幕速度
        rogue.BaseProperties.Add(new BaseProperty(rogue, "UI.PlayerProperty.CalamityMod.RogueVelocity",
            () => BonusSyntax((float)calamityMod.Call("GetRogueVelocity", Main.LocalPlayer) * 100f - 100f, true)));

        AddAttackSpeedProperty(rogue, damageClass);
        AddArmorPenetrationProperty(rogue, damageClass);

        Instance.PropertyCategories.Add("CalamityMod.Rogue", rogue);
    }

    public static void ThoriumIntegration(Mod thoriumMod)
    {
        var thoriumIcon = ModContent.Request<Texture2D>("ThoriumMod/icon_small", AssetRequestMode.ImmediateLoad).Value;
        
        if (thoriumMod.TryFind<DamageClass>("HealerDamage", out var healerDamageClass) &&
            thoriumMod.TryFind<DamageClass>("HealerTool", out var healerToolClass))
        {
            BasePropertyCategory healer = new(ModAsset.Healer.Value, "UI.PlayerProperty.ThoriumMod.Healer", false, thoriumIcon);

            AddDamageProperty(healer, healerDamageClass);
            AddCritProperty(healer, healerDamageClass);
            // 光辉施法速度
            AddAttackSpeedProperty(healer, healerDamageClass);
            // 治疗速度（法术施法速度）
            healer.BaseProperties.Add(new BaseProperty(healer, "UI.PlayerProperty.ThoriumMod.HealingSpeed",
                () => BonusSyntax(PlayerAttackSpeed(healerToolClass), true)));
            // 法术回血量加成
            healer.BaseProperties.Add(new BaseProperty(healer, "UI.PlayerProperty.ThoriumMod.HealBonus",
                () =>
                {
                    int healBonus = (int)thoriumMod.Call("GetHealBonus", Main.LocalPlayer);
                    return healBonus > 0 ? $"+{healBonus}" : healBonus.ToString();
                }));
            AddArmorPenetrationProperty(healer, healerDamageClass);

            Instance.PropertyCategories.Add("ThoriumMod.Healer", healer);
        }

        if (thoriumMod.TryFind<DamageClass>("BardDamage", out var bardDamageClass))
        {
            BasePropertyCategory bard = new(ModAsset.Bard.Value, "UI.PlayerProperty.ThoriumMod.Bard", false, thoriumIcon);
            
            // 乐师伤害、暴击、攻速与穿甲
            AddDamageProperty(bard, bardDamageClass);
            AddCritProperty(bard, bardDamageClass);
            AddAttackSpeedProperty(bard, bardDamageClass);
            AddArmorPenetrationProperty(bard, bardDamageClass);

            Instance.PropertyCategories.Add("ThoriumMod.Bard", bard);
        }
    }

    #region 快捷添加伤害类型基础属性

    /// <summary>
    /// 伤害
    /// </summary>
    public static void AddDamageProperty(BasePropertyCategory category, DamageClass damageClass)
    {
        category.BaseProperties.Add(new BaseProperty(category, "UI.PlayerProperty.Damage",
            () => BonusSyntax(PlayerDamage(damageClass), true)));
    }

    /// <summary>
    /// 暴击
    /// </summary>
    public static void AddCritProperty(BasePropertyCategory category, DamageClass damageClass)
    {
        category.BaseProperties.Add(new BaseProperty(category, "UI.PlayerProperty.Crit",
            () => BonusSyntax(PlayerCrit(damageClass), true)));
    }

    /// <summary>
    /// 攻速
    /// </summary>
    public static void AddAttackSpeedProperty(BasePropertyCategory category, DamageClass damageClass)
    {
        category.BaseProperties.Add(new BaseProperty(category, "UI.PlayerProperty.Speed",
            () => BonusSyntax(PlayerAttackSpeed(damageClass), true)));
    }

    /// <summary>
    /// 穿甲
    /// </summary>
    public static void AddArmorPenetrationProperty(BasePropertyCategory category, DamageClass damageClass)
    {
        category.BaseProperties.Add(new BaseProperty(category, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(damageClass), 2)}"));
    }

    #endregion

    #region 各种获取属性的方法

    /// <summary>
    /// 获取 LocalPlayer 的伤害
    /// </summary>
    public static float PlayerDamage(DamageClass damageClass)
    {
        var sm = Main.LocalPlayer.GetTotalDamage(damageClass);
        return (sm.Additive * sm.Multiplicative - 1) * 100f;
    }

    /// <summary>
    /// 获取 LocalPlayer 的暴击
    /// </summary>
    public static float PlayerCrit(DamageClass damageClass)
    {
        return Main.LocalPlayer.GetTotalCritChance(damageClass);
    }

    /// <summary>
    /// 获取 LocalPlayer 的攻速
    /// </summary>
    public static float PlayerAttackSpeed(DamageClass damageClass)
    {
        return Main.LocalPlayer.GetTotalAttackSpeed(damageClass) * 100f - 100f;
    }

    #endregion

    /// <summary>
    /// 对于传入的value，以加成格式显示 (四舍五入并保留两位小数) <br/>
    /// 不带符号：value% <br/>
    /// 带符号：+value%(正值) 或 -value%(负值) 或 -0%(0) <br/>
    /// </summary>
    public static string BonusSyntax(float value, bool sign = false)
    {
        float roundedValue = MathF.Round(value, 2);
        if (!sign)
            return $"{roundedValue}%";
        return roundedValue > 0 ? $"+{roundedValue}%" : $"{roundedValue}%";
    }
}