using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class PresetComponent : TimerView
{
    private WeaponPage _parent;
    public AmmoChain Chain;
    public SUIText Text;

    public PresetComponent(WeaponPage parent, AmmoChain chain, string name)
    {
        _parent = parent;
        Border = 2f;
        Rounded = new Vector4(12f);
        Chain = chain;

        BorderColor = UIStyle.PanelBorder;
        BgColor = UIStyle.PanelBg;

        Text = new SUIText
        {
            TextOrKey = name,
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

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (_parent.SlotItem.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalItem))
        {
            globalItem.Chain = Chain;
        }
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        base.RightMouseDown(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        BgColor = Color.Black * HoverTimer.Lerp(0.35f, 0.5f);
        BorderColor = Color.Black * 0.5f;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var innerDimensions = GetInnerDimensions();
        var ammoClipCenter = innerDimensions.Center();
        ammoClipCenter.X -= innerDimensions.Width / 3.5f;
        var ammoClip = ModAsset.AmmoClip.Value;
        var ammoClipPos = ammoClipCenter - ammoClip.Size() / 2f;
        spriteBatch.Draw(ammoClip, ammoClipPos, Color.White);
    }
}