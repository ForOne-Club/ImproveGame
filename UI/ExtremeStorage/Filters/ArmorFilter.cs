namespace ImproveGame.UI.ExtremeStorage.Filters;

public class ArmorFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsArmor();
}