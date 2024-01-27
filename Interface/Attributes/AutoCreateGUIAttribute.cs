namespace ImproveGame.Interface.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoCreateGUIAttribute(string layerName, string ownName) : Attribute
{
    public string LayerName = layerName;
    public string OwnName = ownName;
}
