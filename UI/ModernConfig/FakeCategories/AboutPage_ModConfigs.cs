using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.FakeCategories;
public sealed class AboutPage_ModConfig : Category
{
    public override int ItemIconId => ItemID.GPS;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        var text = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage_ModConfig.About",
            //TextOrKey = "你好！这个页面是由错数螺线(LogSpiral)脑子一热试着做的！\n因为Cyril做的ModernConfig实在是太【顺滑口感】了\n如果出了什么问题【请求原谅】，我是说【私信轰炸】来催促他【摧毁漏洞】",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1f,
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        AboutPage.AddLinksToPanel(panel);

        var fumoText = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage_ModConfig.FumoText",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = StyleDimension.Fill,
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(20f)
        };
        panel.AddToOptionsDirect(fumoText);
        fumoText.RecalculateText();
        fumoText.SetInnerPixels(new Vector2(0f, fumoText.TextSize.Y));

        var koishi = new SUIDrawingImage(self => SDFGraphics.FumoFumoKoishi(self.GetDimensions().Center() + new Vector2(0, -75)))
        {
            Width = new(400, 0f),
            Height = new(500, 0),
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(koishi);

        panel.Recalculate();
    }
    //public override string Label => "关于(";
    //public override string Tooltip => "通用版ModernConfig PrPrPrPrPr";
}
