using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.OptionElements.PresetElements;

public class AddPresetElement : BasePresetElement
{
    public AddPresetElement() : base(GetText("ModernConfig.Presets.AddPreset.Label"),
        GetText("ModernConfig.Presets.AddPreset.Tooltip"))
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
            TrUtils.OpenToURL(GetText("ModernConfig.Presets.AddPreset.Link"));
        };
        infoButton.JoinParent(buttonBox);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        SoundEngine.PlaySound(SoundID.Item37);
        PresetHandler.SaveAsPreset("预设");
    }
}