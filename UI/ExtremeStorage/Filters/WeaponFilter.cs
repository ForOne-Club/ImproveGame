namespace ImproveGame.UI.ExtremeStorage.Filters;

public sealed class WeaponFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsWeapon();
}