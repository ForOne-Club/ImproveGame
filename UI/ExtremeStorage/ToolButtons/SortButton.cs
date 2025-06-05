using ImproveGame.Packets.NetStorager;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public class SortButton : ToolButton
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.SortStorage);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, FrameVertically, IsMouseHovering ? 1 : 0, 2);

    public override string HoverText => Language.GetTextValue("LegacyInterface.122");
}