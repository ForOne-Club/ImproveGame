namespace ImproveGame.UIFramework.BaseViews;

public interface ISidedView
{
    /// <summary>
    /// 开启GUI界面
    /// </summary>
    public void Open() {}

    /// <summary>
    /// 关闭GUI界面
    /// </summary>
    public void Close() {}

    /// <summary>
    /// 在什么样的条件下会强制关闭GUI界面
    /// </summary>
    public bool ForceCloseCondition() => Main.LocalPlayer.chest != -1 || !Main.playerInventory ||
                                         Main.LocalPlayer.sign > -1 || Main.LocalPlayer.talkNPC > -1;

    public void OnSwapSlide(float factor);
}