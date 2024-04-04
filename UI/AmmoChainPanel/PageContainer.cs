using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel;

public class PageContainer : View
{
    public PageContainer()
    {
        RelativeMode = RelativeMode.Vertical;
        this.SetSize(0f, 400f, 1f, 0f);
        SetPadding(0f);
    }
}