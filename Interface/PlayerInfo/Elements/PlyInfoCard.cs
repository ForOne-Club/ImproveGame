using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.PlayerInfo.Elements;

public class PlyInfoCard : TimerView
{
    internal static float width = 200, height = 36f;
    private static readonly Vector2 spacing = new Vector2(6);

    /// <summary> 计算总大小 </summary>
    public static Vector2 TotalSize(int row, int column)
    {
        return new Vector2((width + spacing.X) * row - spacing.X, (height + spacing.Y) * column - spacing.Y);
    }

    private readonly Texture2D _icon;
    private const float _scale = 0.85f;
    private readonly string _text;
    private Vector2 _textSize;

    private readonly Func<string> _textFunc;

    public Color BeginBorderColor = UIColor.PanelBorder * 0.75f;
    public Color EndBorderColor = UIColor.PanelBorder;
    public Color BeginBgColor = new Color(43, 56, 101) * 0.75f;
    public Color EndBgColor = new Color(43, 56, 101);

    public PlyInfoCard(string text, Func<string> textFunc, string icon)
    {
        Wrap = true;
        Relative = RelativeMode.Vertical;
        Spacing = spacing;

        PaddingLeft = 10f;
        PaddingRight = 14f;
        Width.Pixels = width;
        Height.Pixels = height;
        DragIgnore = true;

        _text = text;
        _textSize = FontAssets.MouseText.Value.MeasureString(text) * _scale;
        _textFunc = textFunc;
        _textFunc ??= () => string.Empty;
        _icon = GetTexture($"UI/PlayerInfo/{icon}").Value;

        Rounded = new Vector4(10f);
        Border = 2f;
        BorderColor = UIColor.PanelBorder;
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        BgColor = HoverTimer.Lerp(BeginBgColor, EndBgColor);
        base.DrawSelf(sb);
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();
        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensions().Size();

        Vector2 iconPos = innerPos + new Vector2(30 * _scale, innerSize.Y) / 2f;

        float maxSize = 32f;
        float iconScale = _icon.Width > maxSize || _icon.Height > maxSize
            ? _icon.Width > _icon.Height ? maxSize / _icon.Width : maxSize / _icon.Height
            : 1f;
        sb.Draw(_icon, iconPos, null, Color.White * Opacity.Value, 0f, _icon.Size() / 2f, iconScale * _scale, 0, 0f);

        Vector2 textPos = innerPos + new Vector2((30 + 5) * _scale,
            UIConfigs.Instance.GeneralFontOffsetY * _scale + (innerSize.Y - _textSize.Y) / 2);
        DrawString(textPos, _text, Color.White * Opacity.Value, Color.Black * Opacity.Value, _scale);

        string infoText = _textFunc();
        Vector2 infoSize = GetFontSize(infoText) * _scale;
        Vector2 infoPos = innerPos + new Vector2(innerSize.X - infoSize.X,
            UIConfigs.Instance.GeneralFontOffsetY * _scale + (innerSize.Y - infoSize.Y) / 2);
        DrawString(infoPos, infoText, Color.White * Opacity.Value, Color.Black * Opacity.Value, _scale);
    }
}
