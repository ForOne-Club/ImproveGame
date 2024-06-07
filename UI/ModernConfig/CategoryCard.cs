using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategoryCard : TimerView
{
    public readonly Category Category;
    
    public CategoryCard(Category category)
    {
        Category = category;

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);
        SetSizePixels(0f, 40f);
        SetSizePercent(1f, 0f);
        Rounded = new Vector4(10f);
        Border = 1.5f;

        Main.instance.LoadItem(category.ItemIconId);
        _texture = TextureAssets.Item[category.ItemIconId].Value;
        _text = GetText($"ModernConfig.{category.LocalizationKey}");
        
        OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        OnLeftClick += LeftClickEvent;
    }

    private void LeftClickEvent(UIMouseEvent evt, UIElement listeningElement)
    {
        ConfigOptionsPanel.CurrentCategory = Category;
    }
    public Vector2 TextAlign;
    private readonly Texture2D _texture;
    private string _text;
    public Vector2 TextSize { get; private set; }
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            TextSize = FontAssets.MouseText.Value.MeasureString(value);
        }
    }

    public Color BeginBorderColor = UIStyle.PanelBorder;
    public Color EndBorderColor = UIStyle.ItemSlotBorderFav;
    public Color BeginBgColor = UIStyle.ButtonBg;
    public Color EndBgColor = UIStyle.ButtonBgHover;
    public Color TextColor = Color.White;

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        BgColor = HoverTimer.Lerp(BeginBgColor, EndBgColor);
        BorderColor = HoverTimer.Lerp(BeginBorderColor, EndBorderColor);
        base.DrawSelf(spriteBatch);

        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensionsSize();
        
        var texCenter = innerPos + new Vector2(24, innerSize.Y / 2);
        var texOrigin = _texture.Size() / 2;
        float maxTextureSize = 24f;
        float scale = maxTextureSize / Math.Max(_texture.Width, _texture.Height);
        spriteBatch.Draw(_texture, texCenter, null, Color.White, 0f, texOrigin, scale, SpriteEffects.None, 0f);

        DynamicSpriteFont font = FontAssets.MouseText.Value;
        Vector2 textPos = texCenter + new Vector2(maxTextureSize / 2f + 8f, 0);
        textPos.Y += UIConfigs.Instance.GeneralFontOffsetY;
        textPos.Y -= font.LineSpacing / 2f;
        textPos.Y -= 2f;

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, _text, textPos,
            TextColor, new Color(0, 0, 0, TextColor.A), 0f, Vector2.Zero, Vector2.One, spread: 1f);
    }
}