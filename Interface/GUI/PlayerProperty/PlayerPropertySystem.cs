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
        BasePropertyCategory melee = new BasePropertyCategory(GetTexture("UI/PlayerInfo/Melee").Value, "UI.PlayerProperty.Melee");

        // 伤害
        melee.BaseProperties.Add(new BaseProperty(melee, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Melee), 2)}%"));

        // 暴击
        melee.BaseProperties.Add(new BaseProperty(melee, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Melee), 2)}%"));

        // 速度
        melee.BaseProperties.Add(new BaseProperty(melee, "UI.PlayerProperty.Speed",
            () => $"{MathF.Round(Main.LocalPlayer.GetAttackSpeed(DamageClass.Melee) * 100f - 100f, 2)}%"));

        // 穿甲
        melee.BaseProperties.Add(new BaseProperty(melee, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Melee), 2)}"));
        #endregion

        #region 远程
        BasePropertyCategory ranged = new BasePropertyCategory(GetTexture("UI/PlayerInfo/Ranged").Value, "UI.PlayerProperty.Ranged");

        // 伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Ranged), 2)}%"));

        // 弹药伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.BulletDamage",
            () =>
            {
                var sm = Main.LocalPlayer.bulletDamage;
                return $"{Math.Round((sm.Additive * sm.Multiplicative - 1f) * 100, 2)}%";
            }));

        // 弓箭伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.ArrowDamage",
            () =>
            {
                var sm = Main.LocalPlayer.arrowDamage;
                return $"{Math.Round((sm.Additive * sm.Multiplicative - 1f) * 100, 2)}%";
            }));

        // 其他伤害
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.SpecialistDamage",
            () =>
            {
                var sm = Main.LocalPlayer.specialistDamage;
                return $"{Math.Round((sm.Additive * sm.Multiplicative - 1f) * 100, 2)}%";
            }));

        // 暴击
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Ranged), 2)}%"));

        // 穿甲
        ranged.BaseProperties.Add(new BaseProperty(ranged, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Ranged), 2)}"));
        #endregion

        #region 魔法
        BasePropertyCategory magic = new BasePropertyCategory(GetTexture("UI/PlayerInfo/Magic").Value, "UI.PlayerProperty.Magic");

        // 伤害
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Magic), 2)}%"));

        // 法术暴击
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Magic), 2)}%"));

        // 法术回复
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Regen",
            () => $"{Main.LocalPlayer.manaRegen / 2f}/s"));

        // 法术消耗减免
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.Cost",
            () => $"{MathF.Round(Main.LocalPlayer.manaCost * 100f, 2)}%"));

        // 法术穿甲
        magic.BaseProperties.Add(new BaseProperty(magic, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Magic), 2)}"));
        #endregion

        #region 召唤
        BasePropertyCategory summon = new BasePropertyCategory(GetTexture("UI/PlayerInfo/Summon").Value, "UI.PlayerProperty.Summon");

        // 伤害
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Summon), 2)}%"));

        // 鞭子速度
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.SummonMeleeSpeed",
            () => $"{MathF.Round(Main.LocalPlayer.GetAttackSpeed(DamageClass.SummonMeleeSpeed) * 100f - 100f, 2)}%"));

        // 召唤穿甲
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Summon), 2)}"));

        // 召唤栏
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.MaxMinions",
            () => $"{Main.LocalPlayer.slotsMinions}/{Main.LocalPlayer.maxMinions}"));

        // 哨兵栏
        summon.BaseProperties.Add(new BaseProperty(summon, "UI.PlayerProperty.MaxTurrets",
            () => $"{Main.projectile.Count(proj => proj.active && proj.owner == Main.LocalPlayer.whoAmI && proj.WipableTurret)}/{Main.LocalPlayer.maxTurrets}"));
        #endregion

        #region 其他
        BasePropertyCategory other = new BasePropertyCategory(GetTexture("UI/PlayerInfo/Luck").Value, "UI.PlayerProperty.Other");

        // 生命回复
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.LifeRegen",
            () => $"{Main.LocalPlayer.lifeRegen / 2f}/s"));

        // 免伤
        other.BaseProperties.Add(new BaseProperty(other, "UI.PlayerProperty.Endurance",
            () => $"{MathF.Round(Main.LocalPlayer.endurance * 100f, 2)}%"));

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
        PropertyCategories.Add("Other", other);

        // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
        // new PlyInfoCard(GetHJSON("WingTimeMax"),
        //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
    }

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
}
