using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig;

public class TooltipPanel : SUIPanel
{
    internal static TooltipPanel Instance;

    public TooltipTextElement Text;

    public TooltipPanel() : base(Color.Black * 0.4f, Color.Black * 0.4f)
    {
        Instance = this;

        SetPadding(16, 10, 16, 10);
        Text = new TooltipTextElement();
        Text.JoinParent(this);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Instance.Text.TextOrKey == "")
            Instance.Text.TextOrKey = GetText("ModernConfig.NoTooltip");
        base.Draw(spriteBatch);
        Instance.Text.TextOrKey = ConfigOptionsPanel.CurrentCategory?.Tooltip ?? "";
    }

    public static void SetText(string text)
    {
        Instance.Text.TextOrKey = text;
    }
}