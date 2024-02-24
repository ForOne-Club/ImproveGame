using ImproveGame.Packets.Items;
using ImproveGame.Packets.NetStorager;
using ImproveGame.UIFramework;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public class DepositAllButton : ToolButton
{
    public override void OnTakeEffect()
    {
        InventoryFavoritedPacket.Send();
        ToolOperation.Send(ToolOperation.OperationType.DepositAll);
    }

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, FrameVertically, IsMouseHovering ? 1 : 0, 0);

    public override string HoverText => Language.GetTextValue("LegacyInterface.30");
}