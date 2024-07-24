using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.OptionElements.PresetElements;

public class OfficialPresetElement : BasePresetElement
{
    public OfficialPresetElement(string label, string tooltip, string link, Action applyCallback) : base(label, tooltip)
    {
        var buttonBox = new View
        {
            IsAdaptiveWidth = true,
            Height = { Percent = 1f },
            VAlign = 0.5f,
            HAlign = 1f
        };
        buttonBox.SetPadding(0);
        buttonBox.JoinParent(this);

        var infoTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonSeed", AssetRequestMode.ImmediateLoad).Value;
        var infoTooltip = GetText("ModernConfig.ButtonInfo");
        var infoButton = new SUIImageButton(infoTexture, infoTooltip)
        {
            Spacing = new Vector2(2),
            RelativeMode = RelativeMode.Horizontal,
            ImageScale = 1.3f,
            VAlign = 0.5f
        };
        infoButton.ResetSize();
        infoButton.OnLeftMouseDown += (_, _) =>
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            TrUtils.OpenToURL(link);
        };
        infoButton.JoinParent(buttonBox);

        var applyTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay", AssetRequestMode.ImmediateLoad).Value;
        var applyTooltip = GetText("ModernConfig.ButtonApply");
        var applyButton = new SUIImageButton(applyTexture, applyTooltip)
        {
            Spacing = new Vector2(2),
            RelativeMode = RelativeMode.Horizontal,
            ImageScale = 1.3f,
            VAlign = 0.5f
        };
        applyButton.ResetSize();
        applyButton.OnLeftMouseDown += (_, _) =>
        {
            SoundEngine.PlaySound(SoundID.Chat);
            applyCallback.Invoke();
        };
        applyButton.JoinParent(buttonBox);
    }
}