using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.FakeCategories;

public class AboutPage : Category
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
            Width = {Precent = 1f},
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));
        
        var discordLink = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage.LinkDiscord",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = StyleDimension.Fill,
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(40f)
        };
        discordLink.OnUpdate += _ =>
        {
            discordLink.TextColor = discordLink.HoverTimer.Lerp(Color.White, Main.OurFavoriteColor);
            discordLink.RecalculateText();
        };
        discordLink.OnLeftMouseDown += (_, _) =>
        {
            TrUtils.OpenToURL("https://discord.gg/rEmGMQv5z7");
            SoundEngine.PlaySound(SoundID.MenuOpen);
        };
        discordLink.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        panel.AddToOptionsDirect(discordLink);
        discordLink.RecalculateText();
        discordLink.SetInnerPixels(new Vector2(0f, discordLink.TextSize.Y));
        
        if (Language.ActiveCulture.Name is not "zh-Hans")
            return;
        
        var qqLink = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage.LinkQQ",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = StyleDimension.Fill,
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(2f)
        };
        qqLink.OnUpdate += _ =>
        {
            qqLink.TextColor = qqLink.HoverTimer.Lerp(Color.White, Main.OurFavoriteColor);
            qqLink.RecalculateText();
        };
        qqLink.OnLeftMouseDown += (_, _) =>
        {
            TrUtils.OpenToURL("https://qm.qq.com/q/MQG5T6E3io");
            SoundEngine.PlaySound(SoundID.MenuOpen);
        };
        qqLink.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        panel.AddToOptionsDirect(qqLink);
        qqLink.RecalculateText();
        qqLink.SetInnerPixels(new Vector2(0f, qqLink.TextSize.Y));
        
        panel.Recalculate();
    }
}