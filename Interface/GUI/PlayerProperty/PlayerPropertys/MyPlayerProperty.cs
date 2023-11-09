using ImproveGame.Interface.PlayerProperty.Elements;

namespace ImproveGame.Interface.GUI.PlayerProperty.PlayerPropertys;

public class MyPlayerProperty
{
    // 收藏
    public bool Favorite { get; set; }
    // 小图标
    public Texture2D Icon { get; set; }
    // 属性名的 Key
    public string NameKey { get; set; }
    // 属性名
    public string Name => GetText($"UI.PlayerInfo.{NameKey}");
    // 属性值
    public Func<Player, float> InitialValue { get; set; }
    // 属性加上符号的值
    public Func<float, string> SignedValue { get; set; }

    public MyPlayerProperty(Texture2D icon, string nameKey, Func<Player, float> initialValue, Func<float, string> signedValue)
    {
        Icon = icon;
        NameKey = nameKey;
        InitialValue = initialValue;
        SignedValue = signedValue;
    }

    // 所有的 PlayerProperty
    public static readonly List<MyPlayerProperty> MyPlayerPropertys = new();

    static MyPlayerProperty()
    {
        ref var mpps = ref MyPlayerPropertys;

        // 生命回复
        var mpp = new MyPlayerProperty(null, "LifeRegen",
            (player) => player.lifeRegen / 2f,
            (initialValue) => $"{Math.Round(initialValue, 2)}/s");

        // 通用穿甲
        mpp = new MyPlayerProperty(null, "ArmorPenetration",
            (player) => player.GetTotalArmorPenetration(DamageClass.Generic),
            (initialValue) => Math.Round(initialValue, 2).ToString());

        // 近战穿甲
        mpp = new MyPlayerProperty(null, "ArmorPenetration",
            (player) => player.GetTotalArmorPenetration(DamageClass.Melee),
            (initialValue) => Math.Round(initialValue, 2).ToString());

        // 远程穿甲
        mpp = new MyPlayerProperty(null, "ArmorPenetration",
            (player) => player.GetTotalArmorPenetration(DamageClass.Ranged),
            (initialValue) => Math.Round(initialValue, 2).ToString());

        // 魔法穿甲
        mpp = new MyPlayerProperty(null, "ArmorPenetration",
            (player) => player.GetTotalArmorPenetration(DamageClass.Magic),
            (initialValue) => Math.Round(initialValue, 2).ToString());

        // 召唤穿甲
        mpp = new MyPlayerProperty(null, "ArmorPenetration",
            (player) => player.GetTotalArmorPenetration(DamageClass.Summon),
            (initialValue) => Math.Round(initialValue, 2).ToString());

        /*// 近战伤害
        new PlayerPropertyCard(Key2HJSON("MeleeDamage"),
            () => $"{GetDamage(DamageClass.Melee)}%",
            "Melee2").Join(CardsView);

        // 近战暴击
        new PlayerPropertyCard(Key2HJSON("MeleeCrit"),
            () => $"{GetCrit(DamageClass.Melee)}%", "Melee2").Join(CardsView);

        // 近战速度
        new PlayerPropertyCard(Key2HJSON("MeleeSpeed"),
            () => $"{MathF.Round(_player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2").Join(CardsView);

        // 远程伤害
        new PlayerPropertyCard(Key2HJSON("RangedDamage"),
            () => $"{GetDamage(DamageClass.Ranged)}%",
            "Ranged2").Join(CardsView);

        // 远程暴击
        var bj = new PlayerPropertyCard(Key2HJSON("RangedCrit"),
            () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2");
        bj.Rounded.Z = 12f;
        bj.Join(CardsView);

        // 法术回复
        new PlayerPropertyCard(Key2HJSON("ManaRegen"),
            () => $"{_player.manaRegen / 2f}/s", "ManaRegen").Join(CardsView);

        // 法术伤害
        new PlayerPropertyCard(Key2HJSON("ManaDamage"),
            () => $"{GetDamage(DamageClass.Magic)}%",
            "Magic").Join(CardsView);

        // 法术暴击
        new PlayerPropertyCard(Key2HJSON("ManaCrit"),
            () => $"{GetCrit(DamageClass.Magic)}%", "Magic").Join(CardsView);

        // 法术消耗减免
        new PlayerPropertyCard(Key2HJSON("ManaCost"),
            () => $"{MathF.Round(_player.manaCost * 100f)}%",
            "Magic").Join(CardsView);

        // 召唤伤害
        new PlayerPropertyCard(Key2HJSON("SummonDamage"),
            () => $"{GetDamage(DamageClass.Summon)}%",
            "Summon").Join(CardsView);

        // 召唤栏
        new PlayerPropertyCard(Key2HJSON("MaxMinions"),
            () => $"{_player.slotsMinions}/{_player.maxMinions}", "Summon").Join(CardsView);

        // 哨兵栏
        new PlayerPropertyCard(Key2HJSON("MaxTurrets"),
            () => $"{Main.projectile.Count(proj => proj.active && proj.owner == _player.whoAmI && proj.WipableTurret)}/{_player.maxTurrets}",
            "Summon").Join(CardsView);

        // 免伤
        new PlayerPropertyCard(Key2HJSON("Endurance"),
            () => $"{MathF.Round(_player.endurance * 100f)}%",
            "Endurance3").Join(CardsView);

        // 渔力
        new PlayerPropertyCard(Key2HJSON("FishingSkill"),
            () => $"{_player.fishingSkill}",
            "FishingSkill").Join(CardsView);

        // 幸运
        new PlayerPropertyCard(Key2HJSON("Luck"),
            () => $"{_player.luck}", "Luck").Join(CardsView);

        // 仇恨
        new PlayerPropertyCard(Key2HJSON("Aggro"),
            () => $"{_player.aggro}", "Aggro").Join(CardsView);*/

        mpps.Add(mpp);
    }
}
