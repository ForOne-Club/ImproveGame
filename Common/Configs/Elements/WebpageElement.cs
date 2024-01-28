namespace ImproveGame.Common.Configs.Elements;

internal class FaqElement : WebpageElement
{
    protected override string Url => Language.ActiveCulture.Name is "zh-Hans"
        ? "https://gitee.com/MyGoold/improve-game/wikis/%E5%B8%B8%E8%A7%81%E9%97%AE%E9%A2%98"
        : "https://github.com/487666123/ImproveGame/wiki/Frequently-Asked-Questions";
}

internal abstract class WebpageElement : LargerPanelElement
{
    protected abstract string Url { get; }

    public override void LeftClick(UIMouseEvent evt) {
        base.LeftClick(evt);

        // 打开网页
        TrUtils.OpenToURL(Url);
    }
}