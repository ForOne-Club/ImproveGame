using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoGrid : RelativeElement
    {
        public RelativeElement uielist;
        public SUIScrollbar scrollbar;
        public PlyInfoGrid()
        {
            uielist = new RelativeElement
            {
                OverflowHidden = true
            };
            scrollbar = new SUIScrollbar();
        }
    }
}
