using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

public class SUIBuildMaterialItemSlot : SUIItemSlot
{
    public override bool CanPutInItemSlot(Item item) => item.IsAir || item.createTile >= TileID.Dirt || item.createWall >= WallID.None;
}
