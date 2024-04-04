using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public abstract class ToolComponent : TimerView
{
    protected virtual string Key => "Null";
    protected virtual Texture2D Icon => ModAsset.AddChainIcon.Value;
    private WeaponPage _parent;
    public SUIText Text;

    public ToolComponent(WeaponPage parent)
    {
        _parent = parent;
        Border = 2f;
        Rounded = new Vector4(12f);

        RelativeMode = RelativeMode.Horizontal;
        Spacing = new Vector2(8f);
        PreventOverflow = true;

        BorderColor = UIStyle.PanelBorder;
        BgColor = UIStyle.TrashSlotBg;

        Text = new SUIText
        {
            UseKey = true,
            IsLarge = false,
            TextAlign = new Vector2(0.5f, 0.5f),
            TextBorder = 1f,
            TextScale = 0.9f,
            IsWrapped = true,
            MaxCharacterCount = 10,
            OverflowHidden = true,
            MaxLines = 3
        };
        Text.SetSizePercent(0.6f, 1f);
        Text.SetSizePixels(-6f, 0f);
        Text.SetPos(0f, 0f, 0.4f, 0f);
        Text.JoinParent(this);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        BgColor = Color.Black * HoverTimer.Lerp(0.35f, 0.5f);
        BorderColor = Color.Black * 0.5f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Text.TextOrKey = Key;

        base.Draw(spriteBatch);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var innerDimensions = GetInnerDimensions();
        var center = innerDimensions.Center();
        center.X -= innerDimensions.Width / 4f;
        var pos = center - Icon.Size() / 2f;
        spriteBatch.Draw(Icon, pos, Color.White);
    }
}