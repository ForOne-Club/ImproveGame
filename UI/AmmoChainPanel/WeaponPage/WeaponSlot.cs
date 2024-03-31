﻿using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class WeaponSlot : BigBagItemSlot
{
    private WeaponPage _parent;
    
    public WeaponSlot(WeaponPage parent) : base([new Item()], 0)
    {
        _parent = parent;
        FavoriteAllowed = false;
        SetSizePixels(80f, 80f);

        FastStackAction += () =>
        {
            DoPossibleItemChange();
        };
    }

    public void DoPossibleItemChange()
    {
        _parent.SetupPreview();
        if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out WeaponKeeper keeper))
            keeper.Weapon = Item;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        DoPossibleItemChange();

        if (!Main.mouseItem.IsAir && !Main.playerInventory)
            Main.playerInventory = true;
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        if (Main.LocalPlayer is null || !Main.LocalPlayer.TryGetModPlayer(out WeaponKeeper keeper))
            return;

        if (keeper.Weapon.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalItem) && globalItem.Chain is not null &&
            globalItem.Chain.Chain.Count > 0)
        {
            globalItem.Chain = new AmmoChain();
            _parent.SetupPreview();

            SoundEngine.PlaySound(SoundID.ResearchComplete);

            var parentPosition = AmmoChainUI.Instance.MainPanel.GetInnerDimensions().Center();
            var mousePosition = Main.MouseScreen;
            var finalPosition = mousePosition - parentPosition;
            finalPosition.Y += 4f;
            AmmoChainUI.Instance.GenerateParticleAt(finalPosition);
        }
    }

    public override void Update(GameTime gameTime)
    {
        RightMouseDownTimer = -1; // 不能右键拿取，因为右键是拿来清除弹药链的
        if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out WeaponKeeper keeper))
            Items[Index] = keeper.Weapon;
        base.Update(gameTime);
    }
}