namespace ImproveGame.UI.ExtremeStorage.Filters;

public class MiscFilter : Filter
{
    protected override bool ShouldInclude(Item item) =>
        item.headSlot < 0 && item.bodySlot < 0 && item.legSlot < 0 && !item.accessory && item.damage <= 0 &&
        item.axe is 0 && item.hammer is 0 && item.pick is 0 && item.createTile <= TileID.Dirt &&
        item.createWall <= WallID.None && (item.ammo == AmmoID.None || item.notAmmo);
}