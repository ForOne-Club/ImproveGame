using Terraria.ID;

namespace ImproveGame.UI.ExtremeStorage.Filters;

public class AmmoFilter : Filter
{
    protected override bool ShouldInclude(Item item) => item.IsAmmo();
}