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

        //AboutPage.AddLinksToPanel(panel);

        /*ar fumoText = new SUIText
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
        panel.AddToOptionsDirect(koishi);*/

        panel.Recalculate();
    }
}
