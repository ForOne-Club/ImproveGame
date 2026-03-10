namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class HerbFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Herb";

    public override bool Filter(Item item) => item.IsHerb();
}

public class SummonItemFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "SummonItem";

    public override bool Filter(Item item) => item.IsSummonItem();
}

public class PetFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Pet";

    public override bool Filter(Item item) => item.IsPet();
}

public class MountFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Mount";

    public override bool Filter(Item item) => item.IsMount();
}

public class MaterialFilter : FilterButton
{
    protected override int IconIndex => 4;
    protected override string LocalizationKey => "Material";

    public override bool Filter(Item item) => item.IsMaterial();
}

public class OtherMiscFilter : FilterButton
{
    protected override int IconIndex => 5;
    protected override string LocalizationKey => "Other";

    public override bool Filter(Item item) => !item.IsHerb() && !item.IsMount() && !item.IsPet() &&
                                              !item.IsSummonItem() && !item.IsMaterial();
}