using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.Utilities;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class IconElement : TimerView
{
    public AmmoChain Chain => _parent.EditingChain;
    private ChainEditPage _parent;
    public string OriginalName; // 这个名字不随游戏内改名来更改，用来找到原来的文件

    public IconElement(ChainEditPage parent, string originalName)
    {
        _parent = parent;
        OriginalName = originalName ?? "";
        Border = 2f;
        Rounded = new Vector4(12f);
        SetSizePixels(70, 70);

        BorderColor = UIStyle.PanelBorder;
        BgColor = UIStyle.PanelBg;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (_parent.IsCreatingAChain)
            return;

        ChainSaver.TryDeleteFile(OriginalName);
        AmmoChainUI.Instance.GoToWeaponPage();
        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_parent.IsCreatingAChain)
        {
            BgColor = Color.Black * 0.35f;
            BorderColor = Color.Black * 0.5f;
            return;
        }

        BgColor = Color.Black * HoverTimer.Lerp(0.35f, 0.5f);
        BorderColor = Color.Black * 0.5f;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var innerDimensions = GetInnerDimensions();
        var center = innerDimensions.Center();
        
        if (Chain is null)
            return;

        float ammoClipOpacity = _parent.IsCreatingAChain ? 1f : HoverTimer.Lerp(1f, 0.6f);
        var ammoClipTex = ModAsset.AmmoClip.Value;
        float ammoScale = _parent.IsCreatingAChain ? 1f : HoverTimer.Lerp(1f, 0.9f);
        spriteBatch.Draw(ammoClipTex, center, null, Chain.Color * ammoClipOpacity, 0f, ammoClipTex.Size() / 2f,
            ammoScale, SpriteEffects.None, 0f);

        if (_parent.IsCreatingAChain)
            return;
        var deleteColor = Color.White * HoverTimer.Lerp(0f, 1f);
        var trashTex = ModAsset.TrashHighlight.Value;
        float trashScale = HoverTimer.Lerp(0.9f, 1f);
        spriteBatch.Draw(trashTex, center, null, deleteColor, 0f, trashTex.Size() / 2f, trashScale,
            SpriteEffects.None, 0f);
    }
}