using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.GUI.PlayerProperty;

public class PropertyBar : TimerView
{
    public BaseProperty BaseProperty { get; set; }

    public float Scale = 0.85f;
    public string PropertyName;
    public Vector2 TextSize;
    private Func<string> PropertyValue { get; set; }

    public PropertyBar(string propertyName, Func<string> propertyValue, BaseProperty baseProperty)
    {
        Wrap = false;
        Relative = RelativeMode.Vertical;
        Spacing = new Vector2(2f);

        Width.Pixels = 0f;
        Width.Percent = 1f;
        Height.Pixels = 30f;

        PaddingLeft = 10f;
        PaddingRight = 14f;
        DragIgnore = true;

        PropertyName = propertyName;
        TextSize = FontAssets.MouseText.Value.MeasureString(propertyName) * Scale;
        PropertyValue = propertyValue;
        PropertyValue ??= () => string.Empty;

        BgColor = UIColor.TitleBg2 * 0.75f;
        Rounded = new Vector4(6f);

        BaseProperty = baseProperty;
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        base.DrawSelf(sb);
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();
        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensions().Size();

        Vector2 iconPos = innerPos + new Vector2(30 * Scale, innerSize.Y) / 2f;

        Vector2 textPos = innerPos +
            new Vector2(0, UIConfigs.Instance.GeneralFontOffsetY * Scale + (innerSize.Y - TextSize.Y) / 2);
        DrawString(textPos, PropertyName, Color.White * Opacity.Value, Color.Black * Opacity.Value, Scale);

        string infoText = PropertyValue();
        Vector2 infoSize = GetFontSize(infoText) * Scale;
        Vector2 infoPos = innerPos + new Vector2(innerSize.X - infoSize.X,
            UIConfigs.Instance.GeneralFontOffsetY * Scale + (innerSize.Y - infoSize.Y) / 2);
        DrawString(infoPos, infoText, Color.White * Opacity.Value, Color.Black * Opacity.Value, Scale);
    }
}
