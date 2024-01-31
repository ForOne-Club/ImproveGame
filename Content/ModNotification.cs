using ImproveGame.Interface.Common;
using ImproveGame.Interface.Graphics2D;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.Social.Base;
using Terraria.UI.Chat;

namespace ImproveGame.Content;

public class ModNotification
{
    public string DisplayText;
    public Color TextColor;

    public ModNotification(string displayText, Color textColor)
    {
        DisplayText = displayText;
        TextColor = textColor;
    }
}

public class ModNotificationPopup : IInGameNotification
{
    private ModNotification _notification;
    private int _timeLeft;
    private bool _isMouseHovering;
    private const int _timeLeftMax = 300;

    private float Scale
    {
        get
        {
            if (_timeLeft < 24)
                return MathHelper.Lerp(0f, 1f, (float)_timeLeft / 24);

            if (_timeLeft > _timeLeftMax - 15)
                return MathHelper.Lerp(1f, 0f, ((float)_timeLeft - _timeLeftMax + 15) / 15f);

            return 1f;
        }
    }

    private float Opacity
    {
        get
        {
            float scale = Scale;
            if (scale <= 0.2f)
                return 0f;

            return (scale - 0.2f) / 0.8f;
        }
    }

    public object CreationObject { get; private set; }

    public bool ShouldBeRemoved => _timeLeft <= 0;

    public ModNotificationPopup(string displayText, Color textColor)
    {
        _notification = new ModNotification(displayText, textColor);
        _timeLeft = _timeLeftMax;
        CreationObject = _notification;
    }

    public void Update()
    {
        if (!_isMouseHovering || _timeLeft <= 24 || _timeLeft >= _timeLeftMax - 15)
            _timeLeft--;
    }

    public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
    {
        float opacity = Opacity;
        if (opacity <= 0f)
            return;

        string displayText = _notification.DisplayText;

        float scaleFactor = Scale * 1.1f;
        Vector2 size = (FontAssets.ItemStack.Value.MeasureString(displayText) + new Vector2(58f, 10f)) * scaleFactor;
        Rectangle r = Utils.CenteredRectangle(bottomAnchorPosition + new Vector2(0f, (0f - size.Y) * 0.5f), size);
        Vector2 mouseScreen = Main.MouseScreen;
        _isMouseHovering = r.Contains(mouseScreen.ToPoint());
        var panelBorder = UIStyle.PanelBorder * Opacity * (_isMouseHovering ? 0.75f : 0.5f);
        SDFRectangle.HasBorder(r.TopLeft(), r.Size(), new Vector4(UIStyle.ItemSlotBorderRound),
            UIStyle.PanelBg * Opacity, UIStyle.ItemSlotBorderSize, panelBorder);

        var textPos = r.Center.ToVector2();
        textPos.Y += 4f * Scale;
        Color textColor = _notification.TextColor * (Main.mouseTextColor / 255f) * opacity;
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        Vector2 textSize = font.MeasureString(displayText);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, displayText, textPos, textColor, 0.0f,
            textSize / 2f, new Vector2(Scale), spread: 1f);

        if (_isMouseHovering)
            OnMouseOver();
    }

    public void PushAnchor(ref Vector2 positionAnchorBottom)
    {
        float num = 70f * Opacity;
        positionAnchorBottom.Y -= num;
    }

    private void OnMouseOver()
    {
        if (PlayerInput.IgnoreMouseInterface)
            return;

        Main.player[Main.myPlayer].mouseInterface = true;
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            Main.mouseLeftRelease = false;
            _timeLeft = 24;
        }
    }

    // 原版中此方法无用
    public void DrawInNotificationsArea(SpriteBatch spriteBatch, Rectangle area, ref int gamepadPointLocalIndexTouse)
    {
        Utils.DrawInvBG(spriteBatch, area, Color.Red);
    }
}