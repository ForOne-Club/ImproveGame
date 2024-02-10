namespace ImproveGame.UI.ExtremeStorage.Filters;

public class ToolFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsTool();
}