namespace ImproveGame.Interface;

public static class VanillaLayer
{
    public const string RadialHotbars = "Radial Hotbars";
}

[AttributeUsage(AttributeTargets.Class)]
public class AutoCreateGUIAttribute(string layerName, string ownName) : Attribute
{
    public string LayerName = layerName;
    public string OwnName = ownName;
}
