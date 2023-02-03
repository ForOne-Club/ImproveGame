using ImproveGame.Common.Packets;
using ImproveGame.Common.Packets.NetStorager;

namespace ImproveGame.Interface.ExtremeStorage;

public class RecipeToggleButton : ToolButtonBase
{
    public override void OnTakeEffect() =>
        ExtremeStorageGUI.DisplayCrafting = !ExtremeStorageGUI.DisplayCrafting;

    public override Texture2D Texture =>
        TextureAssets.CraftToggle[ExtremeStorageGUI.DisplayCrafting.ToInt() * 2 + IsMouseHovering.ToInt()].Value;

    public override string HoverText => Language.GetTextValue("GameUI.CraftingWindow");
}

public class SortButton : ToolButtonBase
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.SortStorage);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 2);

    public override string HoverText => Language.GetTextValue("LegacyInterface.122");
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
}

public class LootAllButton : ToolButtonBase
{
    public override void OnTakeEffect() => ToolOperation.Send(ToolOperation.OperationType.LootAll);

    public override Texture2D Texture => ToolIcons.Value;

    public override Rectangle? SourceRectangle => ToolIcons.Frame(2, 6, IsMouseHovering ? 1 : 0, 3);

    public override string HoverText => Language.GetTextValue("LegacyInterface.29");
}

public abstract class ToolButtonBase : View
{
    public static Asset<Texture2D> ToolIcons;

    public ToolButtonBase()
    {
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
            Main.instance.MouseText(HoverText);
    }

    public abstract void OnTakeEffect();

    public override void MouseDown(UIMouseEvent evt)
    {
        base.MouseDown(evt);
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