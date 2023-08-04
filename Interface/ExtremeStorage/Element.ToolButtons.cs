using ImproveGame.Common.Animations;
using ImproveGame.Common.Packets;
using ImproveGame.Common.Packets.NetStorager;
using Terraria.ModLoader.UI;

namespace ImproveGame.Interface.ExtremeStorage;

public class RecipeToggleButton : ToolButtonBase
{
    public override void OnTakeEffect() =>
        ExtremeStorageGUI.DisplayCrafting = !ExtremeStorageGUI.DisplayCrafting;

    public override Texture2D Texture =>
        TextureAssets.CraftToggle[ExtremeStorageGUI.DisplayCrafting.ToInt() * 2 + IsMouseHovering.ToInt()].Value;

    public override string HoverText => Language.GetTextValue("GameUI.CraftingWindow");

    public RecipeToggleButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public class SortButton : ToolButtonBase
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.SortStorage);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 2);

    public override string HoverText => Language.GetTextValue("LegacyInterface.122");

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        const int buttonCoordY = 38;
        float x = MathHelper.Lerp(-34f, -34f - buttonCoordY, FoldTimer.Schedule);
        float y = MathHelper.Lerp(buttonCoordY * 1, buttonCoordY * 2, FoldTimer.Schedule);
        this.SetPos(x, y, 1f);
    }

    public SortButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public class StackToStorageButton : ToolButtonBase
{
    public override void OnTakeEffect()
    {
        InventoryFavoritedPacket.Send();
        ToolOperation.Send(ToolOperation.OperationType.StackToStorage);
    }

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 1);

    public override string HoverText => Language.GetTextValue("LegacyInterface.31");

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        const int buttonCoordY = 38;
        float x = MathHelper.Lerp(-34f, -34f - buttonCoordY, FoldTimer.Schedule);
        float y = MathHelper.Lerp(buttonCoordY * 2, buttonCoordY * 3, FoldTimer.Schedule);
        this.SetPos(x, y, 1f);
    }

    public StackToStorageButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public class DepositAllButton : ToolButtonBase
{
    public override void OnTakeEffect()
    {
        InventoryFavoritedPacket.Send();
        ToolOperation.Send(ToolOperation.OperationType.DepositAll);
    }

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 0);

    public override string HoverText => Language.GetTextValue("LegacyInterface.30");

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        const int buttonCoordY = 38;
        float x = MathHelper.Lerp(-34f, -34f - buttonCoordY, FoldTimer.Schedule);
        float y = MathHelper.Lerp(buttonCoordY * 3, buttonCoordY * 4, FoldTimer.Schedule);
        this.SetPos(x, y, 1f);
    }

    public DepositAllButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public class StackToInventoryButton : ToolButtonBase
{
    public override void OnTakeEffect()
    {
        InventoryFavoritedPacket.Send();
        ToolOperation.Send(ToolOperation.OperationType.StackToInventory);
    }

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 4);

    public override string HoverText => Language.GetTextValue("LegacyInterface.82");

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        const int buttonCoordY = 38;
        float x = -34f - buttonCoordY;
        float y = MathHelper.Lerp(buttonCoordY * 2, buttonCoordY * 5, FoldTimer.Schedule);
        this.SetPos(x, y, 1f);
    }

    public StackToInventoryButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public class LootAllButton : ToolButtonBase
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.LootAll);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 3);

    public override string HoverText => Language.GetTextValue("LegacyInterface.29");

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        const int buttonCoordY = 38;
        float x = -34f - buttonCoordY;
        float y = MathHelper.Lerp(buttonCoordY * 3, buttonCoordY * 6, FoldTimer.Schedule);
        this.SetPos(x, y, 1f);
    }

    public LootAllButton(AnimationTimer foldTimer) : base(foldTimer)
    {
    }
}

public abstract class ToolButtonBase : View
{
    public readonly AnimationTimer FoldTimer;
    public static Asset<Texture2D> ToolIcons;

    public ToolButtonBase(AnimationTimer foldTimer)
    {
        FoldTimer = foldTimer;
        SetSizePixels(30f, 30f);
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