using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace ImproveGame.Content.Tiles;

public class RubbleBedTile : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
        TileID.Sets.IsValidSpawnPoint[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        DustType = DustID.Stone;

        // Placement
        TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
        TileObjectData.newTile.CoordinateHeights = new[] {16, 18};
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.newAlternate.DrawYOffset = 2;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);
    }

    public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
    {
        // Default values match the regular vanilla bed
        // You might need to mess with the info here if your bed is not a typical 4x2 tile
        info.VisualOffset.Y += 4f; // Move player down a notch because the bed is not as high as a regular bed
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
        {
            player.GamepadEnableGrappleCooldown();
            player.sleeping.StartSleeping(player, i, j);
        }

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;

        if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
        {
            // Match condition in RightClick. Interaction should only show if clicking it does something
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.SleepingIcon;
        }
    }
}