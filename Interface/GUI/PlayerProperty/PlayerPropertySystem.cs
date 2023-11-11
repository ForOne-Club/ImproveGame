namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 抽象就是对世界的鞭挞
/// </summary>
public class PlayerPropertySystem : ModSystem
{
    public static PlayerPropertySystem Instance;

    public Dictionary<string, Miximixi> Miximixis = new();

    public override void PostSetupContent()
    {
        Instance = this;

        #region 近战属性
        Miximixi melee = new Miximixi(GetTexture("UI/PlayerInfo/Melee").Value, "UI.PlayerProperty.Melee");
        melee.UIPosition = new Vector2(620f, 20f);

        // 伤害
        melee.Balabalas.Add(new Balabala(melee, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Melee), 2)}%"));

        // 暴击
        melee.Balabalas.Add(new Balabala(melee, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Melee), 2)}%"));

        // 速度
        melee.Balabalas.Add(new Balabala(melee, "UI.PlayerProperty.Speed",
            () => $"{MathF.Round(Main.LocalPlayer.GetAttackSpeed(DamageClass.Melee) * 100f - 100f, 2)}%"));

        // 穿甲
        melee.Balabalas.Add(new Balabala(melee, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Melee), 2)}"));
        #endregion

        #region 远程
        Miximixi ranged = new Miximixi(GetTexture("UI/PlayerInfo/Ranged").Value, "UI.PlayerProperty.Ranged");

        // 伤害
        ranged.Balabalas.Add(new Balabala(ranged, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Ranged), 2)}%"));

        // 暴击
        ranged.Balabalas.Add(new Balabala(ranged, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Ranged), 2)}%"));

        // 穿甲
        ranged.Balabalas.Add(new Balabala(ranged, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Ranged), 2)}"));
        #endregion

        #region 魔法
        Miximixi magic = new Miximixi(GetTexture("UI/PlayerInfo/Magic").Value, "UI.PlayerProperty.Magic");

        // 伤害
        magic.Balabalas.Add(new Balabala(magic, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Magic), 2)}%"));

        // 法术暴击
        magic.Balabalas.Add(new Balabala(magic, "UI.PlayerProperty.Crit",
            () => $"{Math.Round(PlayerCrit(DamageClass.Magic), 2)}%"));

        // 法术回复
        magic.Balabalas.Add(new Balabala(magic, "UI.PlayerProperty.Regen",
            () => $"{Main.LocalPlayer.manaRegen / 2f}/s"));

        // 法术消耗减免
        magic.Balabalas.Add(new Balabala(magic, "UI.PlayerProperty.Cost",
            () => $"{MathF.Round(Main.LocalPlayer.manaCost * 100f, 2)}%"));

        // 法术穿甲
        magic.Balabalas.Add(new Balabala(magic, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Magic), 2)}"));
        #endregion

        #region 召唤
        Miximixi summon = new Miximixi(GetTexture("UI/PlayerInfo/Summon").Value, "UI.PlayerProperty.Summon");

        // 伤害
        summon.Balabalas.Add(new Balabala(summon, "UI.PlayerProperty.Damage",
            () => $"{Math.Round(PlayerDamage(DamageClass.Summon), 2)}%"));

        // 召唤栏
        summon.Balabalas.Add(new Balabala(summon, "UI.PlayerProperty.MaxMinions",
            () => $"{Main.LocalPlayer.slotsMinions}/{Main.LocalPlayer.maxMinions}"));

        // 哨兵栏
        summon.Balabalas.Add(new Balabala(summon, "UI.PlayerProperty.MaxTurrets",
            () => $"{Main.projectile.Count(proj => proj.active && proj.owner == Main.LocalPlayer.whoAmI && proj.WipableTurret)}/{Main.LocalPlayer.maxTurrets}"));

        // 召唤穿甲
        summon.Balabalas.Add(new Balabala(summon, "UI.PlayerProperty.ArmorPenetration",
            () => $"{MathF.Round(Main.LocalPlayer.GetTotalArmorPenetration(DamageClass.Summon), 2)}"));
        #endregion

        #region 其他
        Miximixi other = new Miximixi(GetTexture("UI/PlayerInfo/Luck").Value, "UI.PlayerProperty.Other");

        // 生命回复
        other.Balabalas.Add(new Balabala(other, "UI.PlayerProperty.LifeRegen",
            () => $"{Main.LocalPlayer.lifeRegen / 2f}/s"));

        // 免伤
        other.Balabalas.Add(new Balabala(other, "UI.PlayerProperty.Endurance",
            () => $"{MathF.Round(Main.LocalPlayer.endurance * 100f, 2)}%"));

        // 仇恨
        other.Balabalas.Add(new Balabala(other, "UI.PlayerProperty.Aggro",
            () => $"{Main.LocalPlayer.aggro}"));

        // 幸运
        other.Balabalas.Add(new Balabala(other, "UI.PlayerProperty.Luck",
            () => $"{Math.Round(Main.LocalPlayer.luck, 2)}"));

        // 渔力
        other.Balabalas.Add(new Balabala(other, "UI.PlayerProperty.FishingSkill",
            () => $"{Main.LocalPlayer.fishingSkill}"));
        #endregion

        Miximixis.Add("Melee", melee);
        Miximixis.Add("Ranged", ranged);
        Miximixis.Add("Magic", magic);
        Miximixis.Add("Summon", summon);
        Miximixis.Add("Other", other);

        // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
        // new PlyInfoCard(GetHJSON("WingTimeMax"),
        //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
    }

    /// <summary>
    /// 获取 LocalPlayer 的伤害
    /// </summary>
    public static float PlayerDamage(DamageClass damageClass)
    {
        return (Main.LocalPlayer.GetTotalDamage(damageClass).Additive - 1) * 100f;
    }

    /// <summary>
    /// 获取 LocalPlayer 的暴击
    /// </summary>
    public static float PlayerCrit(DamageClass damageClass)
    {
        return Main.LocalPlayer.GetTotalCritChance(damageClass);
    }
}
