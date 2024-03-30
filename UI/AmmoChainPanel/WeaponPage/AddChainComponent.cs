using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class AddChainComponent : TimerView
{
    private WeaponPage _parent;
    public SUIText Text;

    public AddChainComponent(WeaponPage parent, string name)
    {
        _parent = parent;
        Border = 2f;
        Rounded = new Vector4(12f);

        BorderColor = UIStyle.PanelBorder;
        BgColor = UIStyle.TrashSlotBg;

        Text = new SUIText
        {
            TextOrKey = name,
            IsLarge = false,
            TextAlign = new Vector2(0.5f, 0.5f),
            TextBorder = 1f,
            TextScale = 1f,
            IsWrapped = true,
            MaxCharacterCount = 10,
            OverflowHidden = true,
            MaxLines = 3
        };
        Text.SetSizePercent(1f, 1f);
        Text.JoinParent(this);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        string name = GetText("UI.AmmoChain.FileName");
        AmmoChainUI.Instance.StartEditingChain(new AmmoChain(), true, name);
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
    }
}