using CommunityToolkit.Mvvm.ComponentModel;
using SilkyUIFramework;

namespace ImproveGame.Styles;

/// <summary>
/// Qot 基础样式（扩展中），基于 SilkyUI 开发的所有 UI 样式以此为基类
/// </summary>
public partial class DefaultStyle : ObservableObject
{
    [ObservableProperty]
    public partial Color BorderColor { get; set; } = SUIColor.Border;

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; } = SUIColor.Background * 0.75f;
}
