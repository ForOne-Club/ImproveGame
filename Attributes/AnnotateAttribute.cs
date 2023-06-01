namespace ImproveGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class AnnotateAttribute : Attribute
{
    public string Annotate = string.Empty;

    public AnnotateAttribute() { }

    public AnnotateAttribute(string annotate)
    {
        Annotate = annotate;
    }
}
