namespace ImproveGame.Common.ModHooks;

/// <summary>
/// 提供一个PostSetupContent钩子，给ModPlayer用
/// </summary>
public interface IHookPostSetup
{
    /// <summary>
    /// 和SystemLoader.PostSetupContent同时执行
    /// </summary>
    void PostSetupContent();
}