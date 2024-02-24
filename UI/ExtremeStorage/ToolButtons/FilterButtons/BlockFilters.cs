namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class SolidFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Solid";

    public override bool Filter(Item item) => item.createTile >= TileID.Dirt && Main.tileSolid[item.createTile];
}

public class WallFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Wall";

    public override bool Filter(Item item) => item.createWall >= 0;
}

public class PlatformFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Platform";

    public override bool Filter(Item item) => item.createTile >= TileID.Dirt && Main.tileSolidTop[item.createTile];
}

public class OtherTileFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Other";

    public override bool Filter(Item item) =>
        item.createTile >= TileID.Dirt && !Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile];
}