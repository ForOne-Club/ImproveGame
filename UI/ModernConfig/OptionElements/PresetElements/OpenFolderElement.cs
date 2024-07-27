using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.OptionElements.PresetElements;

public class OpenFolderElement () : BasePresetElement(GetText("ModernConfig.Presets.OpenFolder.Label"),
    GetText("ModernConfig.Presets.OpenFolder.Tooltip"))
{
    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        TrUtils.OpenFolder(PresetHandler.ConfigPresetsPath);
    }

    protected override bool Interactable => true;
}