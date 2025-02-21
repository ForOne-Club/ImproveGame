namespace ImproveGame.Content.Items.Globes.Core;

/// <summary>
/// 球基类，但是Tooltip会说明一个世界有多个结构，使用一次显示一个
/// </summary>
public abstract class GlobePlentyTooltip (int rare, int itemValue) : Globe(rare, itemValue)
{
    public override LocalizedText Tooltip => GetLocalizedText(nameof(Tooltip) + "_1");
}