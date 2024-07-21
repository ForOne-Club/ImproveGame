using ImproveGame.Common.Configs;
using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategoryCard : TimerView
{
    public readonly Category Category;
    private readonly AnimationTimer _selectTimer = new (3);
    private readonly SlideText _labelElement;

    public CategoryCard(Category category)
    {
        Category = category;

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);
        SetSizePixels(0f, 40f);
        SetSizePercent(1f, 0f);
        Rounded = new Vector4(UIStyle.ItemSlotBorderRound);
        Border = UIStyle.ItemSlotBorderSize;

        _texture = category.GetIcon();

        OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        OnLeftMouseDown += LeftMouseDownEvent;

        _labelElement = new SlideText(category.Label, 54)
        {
            VAlign = 0.5f,
            Left = {Pixels = 44},
            RelativeMode = RelativeMode.None
        };
        _labelElement.JoinParent(this);
    }

    private void LeftMouseDownEvent(UIMouseEvent evt, UIElement listeningElement)
    {
        ConfigOptionsPanel.CurrentCategory = Category;
    }

    private readonly Texture2D _texture;

    public Color BeginBorderColor = UIStyle.PanelBorder;
    public Color EndBorderColor = UIStyle.ItemSlotBorderFav;
    public Color BeginBgColor = UIStyle.ButtonBg;
    public Color EndBgColor = UIStyle.ButtonBgHover;

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        // 及时更新文字，用于切换语言的情况
        if (Category.Label != _labelElement.DisplayText)
            _labelElement.DisplayText = Category.Label;
        // 英文长一点，因此缩小字体
        float textScale = Language.ActiveCulture.Name is "zh-Hans"
            ? 1f
            : 0.94f;
        _labelElement.TextScale = textScale;
        
        _selectTimer.UpdateHighFps();
        if (ConfigOptionsPanel.CurrentCategory == Category)
            _selectTimer.Open();
        else
            _selectTimer.Close();

        BgColor = _selectTimer.Lerp(BeginBgColor, EndBgColor);
        BorderColor = HoverTimer.Lerp(BeginBorderColor, EndBorderColor);
        base.DrawSelf(spriteBatch);

        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensionsSize();

        var texCenter = innerPos + new Vector2(24, innerSize.Y / 2);
        var texOrigin = _texture.Size() / 2;
        float maxTextureSize = 24f;
        float scale = maxTextureSize / Math.Max(_texture.Width, _texture.Height);
        spriteBatch.Draw(_texture, texCenter, null, Color.White, 0f, texOrigin, scale, SpriteEffects.None, 0f);
    }
}