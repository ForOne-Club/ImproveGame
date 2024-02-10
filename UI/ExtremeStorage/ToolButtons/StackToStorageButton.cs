using ImproveGame.Packets.Items;
using ImproveGame.Packets.NetStorager;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public class StackToStorageButton : ToolButton
{
    public override void OnTakeEffect()
    {
        InventoryFavoritedPacket.Send();
        ToolOperation.Send(ToolOperation.OperationType.StackToStorage);
    }

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, FrameVertically, IsMouseHovering ? 1 : 0, 1);

    public override string HoverText => Language.GetTextValue("LegacyInterface.31");
}