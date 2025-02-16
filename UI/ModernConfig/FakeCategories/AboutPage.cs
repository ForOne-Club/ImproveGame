using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.FakeCategories;

public sealed class AboutPage : Category
{
    public override int ItemIconId => ItemID.GPS;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        var text = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage.About",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        AddLinksToPanel(panel);

        panel.Recalculate();
    }

    public static void AddLinksToPanel(ConfigOptionsPanel panel)
    {
        var gapProvider = new View
        {
            Width = StyleDimension.Fill,
            Height = new(30, 0f),
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(gapProvider);

        GenerateLinkElement(panel, "Mods.ImproveGame.ModernConfig.AboutPage.LinkGitHub", "https://github.com/ForOne-Club/ImproveGame");
        GenerateLinkElement(panel, "Mods.ImproveGame.ModernConfig.AboutPage.LinkDiscord", "https://discord.gg/rEmGMQv5z7");

        if (Language.ActiveCulture.Name is not "zh-Hans")
            return;

        GenerateLinkElement(panel, "Mods.ImproveGame.ModernConfig.AboutPage.LinkQQ", "https://qm.qq.com/q/MQG5T6E3io");
    }

    private static void GenerateLinkElement(ConfigOptionsPanel panel, string key, string url)
    {
        var link = new SUIText
        {
            TextOrKey = key,
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = StyleDimension.Fill,
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(2f)
        };
        link.OnUpdate += _ =>
        {
            link.TextColor = link.HoverTimer.Lerp(Color.White, Main.OurFavoriteColor);
            link.RecalculateText();
        };
        link.OnLeftMouseDown += (_, _) =>
        {
            TrUtils.OpenToURL(url);
            SoundEngine.PlaySound(SoundID.MenuOpen);
        };
        link.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        panel.AddToOptionsDirect(link);
        link.RecalculateText();
        link.SetInnerPixels(new Vector2(0f, link.TextSize.Y));

    }
}