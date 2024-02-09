using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public abstract class ToolButtonBase : View
{
    protected static Asset<Texture2D> ToolIcons => ModAsset.ToolIcons;
    protected const int FrameVertically = 7;

    public ToolButtonBase()
    {
        SetSizePixels(30f, 30f);
        RelativeMode = RelativeMode.Horizontal;
        VAlign = 0.5f;
        Spacing = new Vector2(2f, 0f);
    }

    public virtual Rectangle? SourceRectangle => null;

    public abstract Texture2D Texture { get; }

    public abstract string HoverText { get; }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (ExtremeStorageGUI.CurrentGroup is ItemGroup.Setting)
            return;

        base.DrawSelf(spriteBatch);

        var dimensions = GetDimensions();
        var pos = dimensions.Position();
        spriteBatch.Draw(position: pos, texture: Texture, sourceRectangle: SourceRectangle, color: Color.White);

        if (IsMouseHovering)
            UICommon.TooltipMouseText(HoverText);
    }

    public abstract void OnTakeEffect();

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        if (ExtremeStorageGUI.CurrentGroup is ItemGroup.Setting)
            return;
        OnTakeEffect();
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }
}