using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.PlayerProperty.Elements;

public class PlayerPropertyCard : TimerView
{
    public static float DefaultWidth = 200, DefaultHeight = 36f;
    public static readonly Vector2 spacing = new Vector2(4);

    /// <summary> 计算总大小 </summary>
    public static Vector2 TotalSize(int row, int column)
    {
        return new Vector2((DefaultWidth + spacing.X) * row - spacing.X, (DefaultHeight + spacing.Y) * column - spacing.Y);
    }

    public float Scale = 0.85f;
    public string PropertyName;
    public Vector2 TextSize;
    private Func<string> PropertyValue;

    public PlayerPropertyCard(string propertyName, Func<string> propertyValue)
    {
        Wrap = false;
        Relative = RelativeMode.Vertical;
        Spacing = spacing;

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
        Rounded = new Vector4(8f);
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        base.DrawSelf(sb);
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();
        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensions().Size();

        Vector2 iconPos = innerPos + new Vector2(30 * Scale, innerSize.Y) / 2f;

        /*float maxSize = 32f;
        float iconScale = _icon.Width > maxSize || _icon.Height > maxSize
            ? _icon.Width > _icon.Height ? maxSize / _icon.Width : maxSize / _icon.Height
            : 1f;
        sb.Draw(_icon, iconPos, null, Color.White * Opacity.Value, 0f, _icon.Size() / 2f, iconScale * _scale, 0, 0f);*/

        Vector2 textPos = innerPos + new Vector2(5 * Scale,
            UIConfigs.Instance.GeneralFontOffsetY * Scale + (innerSize.Y - TextSize.Y) / 2);
        DrawString(textPos, PropertyName, Color.White * Opacity.Value, Color.Black * Opacity.Value, Scale);

        string infoText = PropertyValue();
        Vector2 infoSize = GetFontSize(infoText) * Scale;
        Vector2 infoPos = innerPos + new Vector2(innerSize.X - infoSize.X,
            UIConfigs.Instance.GeneralFontOffsetY * Scale + (innerSize.Y - infoSize.Y) / 2);
        DrawString(infoPos, infoText, Color.White * Opacity.Value, Color.Black * Opacity.Value, Scale);
    }
}
