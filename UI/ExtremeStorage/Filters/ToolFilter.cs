namespace ImproveGame.UI.ExtremeStorage.Filters;

public class ToolFilter : Filter
{
    protected override bool ShouldInclude(Item item) =>
        item.axe != 0 || item.hammer != 0 || item.pick != 0 || item.IsHook();
}