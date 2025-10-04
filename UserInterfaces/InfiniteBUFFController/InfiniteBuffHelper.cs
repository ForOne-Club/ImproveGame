namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

internal static class InfiniteBuffHelper
{
    public static string LeftClickDisable => GetText("BuffTracker.LeftClickDisable");
    public static string LeftClickEnable => GetText("BuffTracker.LeftClickEnable");
    public static string RightClickDisable => GetText("BuffTracker.RightClickDisable");
    public static string RightClickEnable => GetText("BuffTracker.RightClickEnable");

    public static string GetLeftClickString(bool enable)
    {
        return enable ? LeftClickDisable : LeftClickEnable;
    }

    public static string GetRightClickString(bool enable)
    {
        return enable ? RightClickDisable : RightClickEnable;
    }
}
