namespace ImproveGame.Interface.GUI.PlayerStats;

/// <summary>
/// 基本属性
/// </summary>
public class BaseStat
{
    /// <summary>
    /// Whether this stat category is added via Mod.Call
    /// </summary>
    public bool IsAddedFromCall { get; set; }

    public bool Favorite { get; set; } = true;
    public BaseStatsCategory Parent { get; set; }

    public string NameKey { get; set; }

    public string Name => IsAddedFromCall ? Language.GetTextValue(NameKey) : GetText(NameKey);

    public Func<string> Value { get; set; }

    public BaseStat(BaseStatsCategory parent, string nameKey, Func<string> value, bool isAddedFromCall = false)
    {
        Parent = parent;
        NameKey = nameKey;
        Value = value;
        IsAddedFromCall = isAddedFromCall;
    }
}
