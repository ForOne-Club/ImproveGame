namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 巴拉巴拉
/// </summary>
public class Balabala
{
    public Miximixi Parent { get; set; }

    public string NameKey { get; set; }

    public string Name => GetText(NameKey);

    public Func<string> Value { get; set; }

    public Balabala(Miximixi parent, string nameKey, Func<string> value)
    {
        Parent = parent;
        NameKey = nameKey;
        Value = value;
    }
}
