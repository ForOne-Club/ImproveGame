namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class HeadgearFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Headgear";

    public override bool Filter(Item item) => item.headSlot >= 0;
}

public class TorsoFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Torso";

    public override bool Filter(Item item) => item.bodySlot >= 0;
}

public class PantsFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Pants";

    public override bool Filter(Item item) => item.legSlot >= 0;
}

public class VanityArmorFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Vanity";

    public override bool Filter(Item item) => item.vanity;
}