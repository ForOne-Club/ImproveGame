namespace ImproveGame.QolUISystem.UIStruct;

public struct MouseHover
{
    /// <summary>
    /// 完整, 同于 Terraria.UIElement.IsHoverXXX
    /// </summary>
    public bool Whole;

    /// <summary>
    /// 仅在 Qol.EventTrigger.EventHandler() 中使用
    /// </summary>
    public bool Temporary;
}
