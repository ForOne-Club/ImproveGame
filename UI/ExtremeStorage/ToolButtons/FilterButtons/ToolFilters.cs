namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class PickaxeFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Pickaxe";

    public override bool Filter(Item item) => item.pick > 0;
}

public class AxeFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Axe";

    public override bool Filter(Item item) => item.axe > 0;
}

public class HammerFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Hammer";

    public override bool Filter(Item item) => item.hammer > 0;
}

public class HookFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Hook";

    public override bool Filter(Item item) => item.IsHook();
}

public class WiringFilter : FilterButton
{
    protected override int IconIndex => 4;
    protected override string LocalizationKey => "Wiring";

    public override bool Filter(Item item) => item.IsWiringTool();
}

public class FishingPoleFilter : FilterButton
{
    protected override int IconIndex => 5;
    protected override string LocalizationKey => "FishingPole";

    public override bool Filter(Item item) => item.fishingPole > 0;
}

public class OtherToolFilter : FilterButton
{
    protected override int IconIndex => 6;
    protected override string LocalizationKey => "Other";

    public override bool Filter(Item item) => item.IsOtherTool();
}