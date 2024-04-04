using ImproveGame.Content.Functions.ChainedAmmo;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

[Obsolete("弃用，目前使用 LightBulbHelp", true)]
public class GetHelpComponent (WeaponPage parent) : ToolComponent(parent)
{
    protected override string Key => "Mods.ImproveGame.UI.AmmoChain.GetHelp";

    protected override Texture2D Icon => ModAsset.QuestionMarkIcon.Value;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        TrUtils.OpenToURL(Language.ActiveCulture.Name is "zh-Hans"
            ? "https://gitee.com/MyGoold/improve-game/wikis/%E5%BC%B9%E8%8D%AF%E9%93%BE"
            : "https://github.com/487666123/ImproveGame/wiki/Ammo-Chain");
    }
}