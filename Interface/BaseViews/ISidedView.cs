namespace ImproveGame.Interface.BaseViews;

public interface ISidedView
{
    /// <summary>
    /// 开启GUI界面
    /// </summary>
    public void Open();

    /// <summary>
    /// 关闭GUI界面
    /// </summary>
    public void Close();

    public void OnSwapSlide(float factor);
}