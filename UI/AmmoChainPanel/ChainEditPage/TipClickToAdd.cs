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
        SetSizePixels(566, 80);
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
        base.LeftMouseDown(evt);

        if (Parent.SelectedAmmoType is ItemID.None)
            return;
        var item = new Item(Parent.SelectedAmmoType);
        if (!item.IsAmmo())
            return;
        Parent.EditingChain.Chain.Add(new AmmoChain.Ammo(new ItemTypeData(item), 10));
        Parent.ShouldResetCurrentChain = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        BgColor = Color.Black * HoverTimer.Lerp(0.3f, 0.6f);
    }
}