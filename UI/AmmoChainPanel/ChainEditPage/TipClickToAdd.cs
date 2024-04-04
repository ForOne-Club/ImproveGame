using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class TipClickToAdd : TimerView
{
    public ChainEditPage Parent;

    public TipClickToAdd(ChainEditPage parent)
    {
        Parent = parent;
        SetSizePercent(1f);
        SetSizePixels(0, -6);
        Border = 0f;
        Rounded = new Vector4(4f);
        Spacing = new Vector2(2);

        var text = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.UI.AmmoChain.ClickToAddL1",
            UseKey = true,
            TextAlign = new Vector2(0.5f)
        };
        text.SetPosPixels(0f, -15f);
        text.SetSizePercent(1f);
        text.JoinParent(this);

        text = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.UI.AmmoChain.ClickToAddL2",
            UseKey = true,
            TextAlign = new Vector2(0.5f)
        };
        text.SetPosPixels(0f, 11f);
        text.SetSizePercent(1f);
        text.JoinParent(this);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (Parent.EditingChain.Chain.Count != 0)
            return;

        base.LeftMouseDown(evt);

        if (Parent.SelectedAmmoType is ItemID.None)
            return;
        var item = new Item(Parent.SelectedAmmoType);
        Parent.EditingChain.Chain.Add(new AmmoChain.Ammo(new ItemTypeData(item), 10));
        Parent.ShouldResetCurrentChain = true;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        if (Parent.EditingChain.Chain.Count != 0)
        {
            SetSizePercent(0f);
            Recalculate();
            return;
        }

        base.Update(gameTime);

        SetSizePercent(1f);
        Recalculate();
        BgColor = Color.Black * HoverTimer.Lerp(0.3f, 0.6f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Parent.EditingChain.Chain.Count != 0)
            return;

        base.Draw(spriteBatch);
    }
}