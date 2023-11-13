namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 基本属性
/// </summary>
public class BaseProperty
{
    /// <summary>
    /// 判断是否来自其他 Mod
    /// </summary>
    public bool IsOtherMod { get; set; }

    public bool Favorite { get; set; } = true;
    public BasePropertyCategory Parent { get; set; }

    public string NameKey { get; set; }

    public string Name => IsOtherMod ? Language.GetTextValue(NameKey) : GetText(NameKey);

    public Func<string> Value { get; set; }

    public BaseProperty(BasePropertyCategory parent, string nameKey, Func<string> value, bool isOtherMod = false)
    {
        Parent = parent;
        NameKey = nameKey;
        Value = value;
        IsOtherMod = isOtherMod;
    }
}
