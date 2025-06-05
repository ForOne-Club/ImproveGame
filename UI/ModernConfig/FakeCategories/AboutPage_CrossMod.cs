using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.FakeCategories
{
    public class AboutPage_CrossMod : Category
    {
        public override void AddOptions(ConfigOptionsPanel panel)
        {
            panel.ShouldHideSearchBar = true;

            var text = new SUIText
            {
                TextOrKey = GetAboutText?.Invoke() ?? "Empty",
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
        }

        public override int ItemIconId => iconID;
        readonly int iconID;
        readonly Func<Texture2D> GetIconTex;
        readonly Func<string> GetLabel;
        readonly Func<string> GetTooltip;
        readonly Func<string> GetAboutText;
        public override Texture2D GetIcon() => GetIconTex?.Invoke() ?? base.GetIcon();
        public override string Label => GetLabel?.Invoke() ?? base.Label;
        public override string Tooltip => GetTooltip?.Invoke() ?? base.Tooltip;

        public AboutPage_CrossMod(Func<string> getAboutText, int itemIconID = 0, Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
        {
            GetAboutText = getAboutText;
            iconID = itemIconID;
            GetIconTex = getIconTexture;
            GetLabel = getLabel;
            GetTooltip = getTooltip;
        }
    }
}
