namespace ImproveGame.UI.ExtremeStorage.Filters;

public class AccessoryFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsAccessory();
}