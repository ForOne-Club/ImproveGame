namespace ImproveGame.UI.ExtremeStorage.Filters;

public class BlockFilter : Filter
{
    protected override bool ShouldInclude(Item item) =>
        item.createTile >= TileID.Dirt || item.createWall > WallID.None;
}