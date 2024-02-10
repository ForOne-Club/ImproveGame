namespace ImproveGame.UIFramework.BaseViews;

public enum OpacityType { Parent, Own }

public class Opacity
{
    public OpacityType Type;
    private float _value = 1f;
    private float _parentValue = 1f;
    private readonly View _parent;
    public float Value
    {
        get => Type switch
        {
            OpacityType.Parent => _parentValue,
            OpacityType.Own => _value,
            _ => _value
        };
    }

    public Opacity(UIElement parent)
    {
        if (parent is View view)
        {
            _parent = view;
        }
    }

    public bool SetValue(float value)
    {
        _value = value;
        return Type is OpacityType.Own;
    }

    public void Recalculate()
    {
        if (Type is OpacityType.Parent)
        {
            _parentValue = (_parent.Parent as View)?.Opacity?.Value ?? _value;
        }
    }

    public static float operator *(Opacity opacity, float value) => opacity.Value * value;

    public static Color operator *(Color value, Opacity opacity) => value * opacity.Value;

    public static Color operator *(Opacity opacity, Color value) => value * opacity.Value;
}

