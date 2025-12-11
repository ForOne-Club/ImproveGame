using ImproveGame.Helpers.Extensions;

namespace ImproveGame.UI.ExtremeStorage.Filters;

public class BlockFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsPlaceable();
}