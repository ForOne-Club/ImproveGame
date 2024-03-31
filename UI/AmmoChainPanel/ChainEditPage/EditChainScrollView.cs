using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class EditChainScrollView : SUIScrollView2
{
    public EditChainScrollView(Orientation scrollOrientation, bool fixedSize = true) : base(scrollOrientation, fixedSize) {
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        if (!ItemSlot.ShiftInUse)
            return;
        base.ScrollWheel(evt);
    }
}