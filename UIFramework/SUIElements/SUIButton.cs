using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.SUIElements;

/// <summary>
/// 按钮，有 “图标模式” 和 “无图标” 两种模式。
/// </summary>
public class SUIButton : TimerView
{
    public bool IconMode;
    public bool TextHasBorder = true;
    public Vector2 TextAlign;

    private static readonly float iconAndTextSpacing = 6f;
    private Texture2D _texture;
    private string _text;
    private float _textScale = 1f;
    public Vector2 TextSize { get; private set; }
    public float TextScale
    {
        get => _textScale;
        set
        {
            _textScale = value;
            TextSize = FontAssets.MouseText.Value.MeasureString(Text) * value;
        }
    }
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            TextSize = FontAssets.MouseText.Value.MeasureString(value) * TextScale;
        }
    }

    public Color BeginBorderColor = UIStyle.PanelBorder;
    public Color EndBorderColor = UIStyle.ItemSlotBorderFav;
    public Color BeginBgColor = UIStyle.ButtonBg;
    public Color EndBgColor = UIStyle.ButtonBgHover;
    public Color TextColor = Color.White;

    private bool RussianLoaded => LanguageManager.Instance.ActiveCulture.Name is "ru-RU";

    public SUIButton(string text)
    {
        Text = text;
        SetPadding(RussianLoaded ? 12f : 18f, 8f);
        SetInnerPixels(TextSize);
        OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);

        Rounded = new Vector4(10f);
        Border = 2f;
    }

    public SUIButton(Texture2D texture, string text) : this(text)
    {
        // temporary fix for russain text being too long
        // 俄语文字过长问题的暂时修复，只有带icon的按钮目前发现了这个问题
        if (RussianLoaded)
        {
            TextScale *= 0.7f;
        }

        IconMode = true;
        _texture = texture;
        SetPadding(RussianLoaded ? 12f : 18f, 0f);
        SetInnerPixels(_texture.Width + TextSize.X + 4 + iconAndTextSpacing, 40f);
    }
    public void SetIcon(Texture2D texture)
    {
        _texture = texture;
        SetInnerPixels(_texture.Width + TextSize.X + 4 + iconAndTextSpacing, 40f);
    }
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        BgColor = HoverTimer.Lerp(BeginBgColor, EndBgColor);
        BorderColor = HoverTimer.Lerp(BeginBorderColor, EndBorderColor);
        base.DrawSelf(spriteBatch);

        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensionsSize();

        if (IconMode)
        {
            Vector2 texturePos = innerPos + new Vector2(0, (innerSize.Y - _texture.Size().Y) / 2);
            spriteBatch.Draw(_texture, texturePos, Color.White);
        }

        Vector2 textPos = innerPos;

        if (IconMode)
        {
            textPos += new Vector2(_texture.Width + 2 + iconAndTextSpacing, (innerSize.Y - TextSize.Y) / 2);
        }
        else
        {
            textPos += (innerSize - TextSize) * TextAlign;
        }

        textPos.Y += UIConfigs.Instance.GeneralFontOffsetY;
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        TextSnippet[] array = ChatManager.ParseMessage(_text, TextColor).ToArray();
        ChatManager.ConvertNormalSnippets(array);
        var textScale = new Vector2(TextScale);

        if (TextHasBorder)
        {
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, array, textPos,
                new Color(0, 0, 0, TextColor.A), 0f, Vector2.Zero, textScale, spread: 1f);
        }

        ChatManager.DrawColorCodedString(spriteBatch, font, array, textPos, Color.White, 0f, Vector2.Zero, textScale, out int _, -1f);
        // TrUtils.DrawBorderString(spriteBatch, _text, textPos, TextColor);
    }
}
