namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class BootFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Boot";

    public override bool Filter(Item item) => item.shoeSlot >= 0;
}

public class WingFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Wing";

    public override bool Filter(Item item) => item.wingSlot >= 0;
}

public class ShieldFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Shield";

    public override bool Filter(Item item) => item.shieldSlot >= 0;
}

public class OtherAccessoryFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Other";

    public override bool Filter(Item item) => item.shieldSlot < 0 && item.wingSlot < 0 && item.shoeSlot < 0;
}

public class VanityAccessoryFilter : FilterButton
{
    protected override int IconIndex => 4;
    protected override string LocalizationKey => "Vanity";

    public override bool Filter(Item item) => item.vanity;
}