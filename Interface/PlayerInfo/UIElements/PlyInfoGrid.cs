using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoGrid : RelativeUIE
    {
        public RelativeUIE uielist;
        public SUIScrollbar scrollbar;
        public PlyInfoGrid()
        {
            uielist = new RelativeUIE
            {
                OverflowHidden = true
            };
            scrollbar = new SUIScrollbar();
        }
    }
}
