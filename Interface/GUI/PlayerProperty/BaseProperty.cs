namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 基本属性
/// </summary>
public class BaseProperty
{
    /// <summary>
    /// Whether this property category is added via Mod.Call
    /// </summary>
    public bool IsAddedFromCall { get; set; }

    public bool Favorite { get; set; } = true;
    public BasePropertyCategory Parent { get; set; }

    public string NameKey { get; set; }

    public string Name => IsAddedFromCall ? Language.GetTextValue(NameKey) : GetText(NameKey);

    public Func<string> Value { get; set; }

    public BaseProperty(BasePropertyCategory parent, string nameKey, Func<string> value, bool isAddedFromCall = false)
    {
        Parent = parent;
        NameKey = nameKey;
        Value = value;
        IsAddedFromCall = isAddedFromCall;
    }
}
