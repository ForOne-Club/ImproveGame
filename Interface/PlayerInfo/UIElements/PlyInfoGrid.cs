using ImproveGame.Interface.BaseViews;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoGrid : View
    {
        public View uielist;
        public SUIScrollbar scrollbar;
        public PlyInfoGrid()
        {
            uielist = new View
            {
                OverflowHidden = true
            };
            scrollbar = new SUIScrollbar();
        }
    }
}
