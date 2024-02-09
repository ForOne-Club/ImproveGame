namespace ImproveGame.UI.ExtremeStorage.Filters;

public sealed class WeaponFilter : Filter
{
    protected override bool ShouldInclude(Item item) =>
        item.damage > 0 && item.axe is 0 && item.hammer is 0 && item.pick is 0;
}