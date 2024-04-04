using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class TipPutInItem : TimerView
{
    public WeaponPage Parent;
    private SUIText _text;

    public TipPutInItem(WeaponPage parent)
    {
        Parent = parent;
        SetSizePercent(1f);
        Border = 0f;
        Rounded = new Vector4(4f);
        Spacing = new Vector2(2);

        _text = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.UI.AmmoChain.PutInItem",
            UseKey = true,
            TextAlign = new Vector2(0.5f)
        };
        _text.SetSizePercent(1f);
        _text.JoinParent(this);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Parent.CurrentPreview.ListView.Children.Any())
        {
            SetSizePercent(0f);
            Recalculate();
            return;
        }

        _text.TextOrKey = Parent.WeaponSlot.Item.IsAir
            ? "Mods.ImproveGame.UI.AmmoChain.PutInItem"
            : "Mods.ImproveGame.UI.AmmoChain.ClickAmmoChain";
        if (ChainSaver.AmmoChains.Count is 0)
            _text.TextOrKey = "Mods.ImproveGame.UI.AmmoChain.AddAmmoChain";
        if (!Parent.WeaponSlot.Item.IsAir && Parent.WeaponSlot.Item.useAmmo <= 0)
            _text.TextOrKey = "Mods.ImproveGame.UI.AmmoChain.ItemNotAvailable";

        SetSizePercent(1f);
        Recalculate();
        BgColor = Color.Transparent;

        base.Draw(spriteBatch);
    }
}