using ImproveGame.Content.Functions.ChainedAmmo;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

[Obsolete("弃用，目前使用 OpenFolderButton", true)]
public class OpenFolderComponent (WeaponPage parent) : ToolComponent(parent)
{
    protected override string Key => "Mods.ImproveGame.UI.AmmoChain.OpenFolder";

    protected override Texture2D Icon => ModAsset.FolderLargeIcon.Value;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        TrUtils.OpenFolder(ChainSaver.SavePath);
    }
}