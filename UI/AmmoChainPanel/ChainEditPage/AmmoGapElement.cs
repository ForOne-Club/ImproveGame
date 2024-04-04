using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class AmmoGapElement : TimerView
{
    public int Index;
    public ChainEditPage Parent;
    
    public AmmoGapElement(int index, ChainEditPage parent)
    {
        Index = index;
        Parent = parent;
        SetSizePixels(14, 60);
        Border = 0f;
        Rounded = new Vector4(4f);
        RelativeMode = RelativeMode.Horizontal;
        Spacing = new Vector2(2);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (Parent.SelectedAmmoType is ItemID.None)
            return;
        var item = new Item(Parent.SelectedAmmoType);
        Parent.EditingChain.Chain.Insert(Index, new AmmoChain.Ammo(new ItemTypeData(item), 10));
        Parent.ShouldResetCurrentChain = true;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        BgColor = Color.Black * HoverTimer.Lerp(0f, 0.5f);
    }
}