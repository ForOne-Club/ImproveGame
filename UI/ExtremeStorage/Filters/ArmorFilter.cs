namespace ImproveGame.UI.ExtremeStorage.Filters;

public class ArmorFilter : Filter
{
    protected override bool ShouldInclude(Item item) =>
        item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0;
}