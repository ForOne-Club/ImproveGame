using ImproveGame.Common.Players;

namespace ImproveGame.Common.GlobalItems;

public class AutoTrashGlobalItem : GlobalItem
{
    public override bool OnPickup(Item item, Player player)
    {
        if (player.TryGetModPlayer(out AutoTrashPlayer autoTrashPlayer) && true && autoTrashPlayer.ContainsAutoTrash(item.type))
        {
            autoTrashPlayer.AddToLastItem(item);
            SoundEngine.PlaySound(SoundID.Grab);
            return false;
        }

        return true;
    }
}
