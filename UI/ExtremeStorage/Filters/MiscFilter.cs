namespace ImproveGame.UI.ExtremeStorage.Filters;

public class MiscFilter : Filter
{
    protected override bool ShouldInclude(Item item) => !item.IsAccessory() && !item.IsAmmo() && !item.IsArmor() &&
                                                        !item.IsPlaceable() && !item.IsTool() && !item.IsWeapon();
}