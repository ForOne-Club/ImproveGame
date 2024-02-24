namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class MeleeFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Melee";

    public override bool Filter(Item item) =>
        item.DamageType is MeleeDamageClass or MeleeNoSpeedDamageClass;
}

public class RangedFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Ranged";

    public override bool Filter(Item item) =>
        item.DamageType is RangedDamageClass;
}

public class MagicFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Magic";

    public override bool Filter(Item item) =>
        item.DamageType is MagicDamageClass or MagicSummonHybridDamageClass;
}

public class SummonFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Summon";

    public override bool Filter(Item item) =>
        item.DamageType is SummonDamageClass or SummonMeleeSpeedDamageClass;
}

public class OtherDamageFilter : FilterButton
{
    protected override int IconIndex => 4;
    protected override string LocalizationKey => "Other";

    // 注意这个or不是按英语语法来的，这里是非原版或者以下几种
    public override bool Filter(Item item) =>
        item.DamageType is not VanillaDamageClass or DefaultDamageClass or GenericDamageClass or ThrowingDamageClass;
}