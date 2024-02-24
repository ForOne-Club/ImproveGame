using ImproveGame.Packets.NetStorager;
using ImproveGame.UIFramework;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

[Obsolete("使用率较低，容易误触")]
public class LootAllButton : ToolButton
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.LootAll);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, FrameVertically, IsMouseHovering ? 1 : 0, 3);

    public override string HoverText => Language.GetTextValue("LegacyInterface.29");
}