namespace ImproveGame.Interface.BaseViews;

public class BaseGrid : View
{
    /// <summary>
    /// 当前方向上的格子数量
    /// </summary>
    public int MonomerNumber;

    /// <summary>
    /// 格子之间的间距
    /// </summary>
    public Vector2 MonomerSpacing;

    /// <summary>
    /// 单体大小
    /// </summary>
    public Vector2 MonomerSize;

    /// <summary>
    /// 初始设置
    /// </summary>
    /// <param name="monomerNumber"></param>
    /// <param name="monomerSpacing"></param>
    /// <param name="monomerSize"></param>
    public void SetGridValues(int monomerNumber, Vector2 monomerSpacing, Vector2 monomerSize)
    {
        MonomerNumber = monomerNumber;
        MonomerSpacing = monomerSpacing;
        MonomerSize = monomerSize;
    }

    /// <summary>
    /// 在每次添加后新元素后调用
    /// </summary>
    public void CalculateAndSetGridSizePixels()
    {
        List<UIElement> uies = (List<UIElement>)Children;
        float widthNumber = Math.Min(uies.Count, MonomerNumber);
        float heightNumber = MathF.Ceiling((float)uies.Count / MonomerNumber);
        Width.Pixels = widthNumber * MonomerSize.X + (widthNumber - 1) * MonomerSpacing.X;
        Height.Pixels = heightNumber * MonomerSize.Y + (heightNumber - 1) * MonomerSpacing.Y;
        Recalculate();
    }

    public void CalculateChildrenPositionPixels()
    {
        List<UIElement> uies = (List<UIElement>)Children;

        for (int i = 0; i < uies.Count; i++)
        {
            uies[i].Left.Pixels = i % MonomerNumber * (MonomerSize.X + MonomerSpacing.X);
            uies[i].Top.Pixels = i / MonomerNumber * (MonomerSize.Y + MonomerSpacing.Y);
        }
    }

    public override void Recalculate()
    {
        Opacity.Recalculate();
        CalculateChildrenPositionPixels();
        base.Recalculate();
    }
}
