using Terraria.DataStructures;

namespace ImproveGame.Content.Functions.VeinMiner;

// 用来让物品全都聚到玩家身上
public class VeinMinerGlobalItem : GlobalItem
{
    public override void OnSpawn(Item item, IEntitySource source)
    {
        if (!VeinMinerSystem.VeinMining || source is not EntitySource_TileBreak ||
            !Main.player.IndexInRange(VeinMinerSystem.MinerIndex))
            return;

        var player = Main.player[VeinMinerSystem.MinerIndex];
        item.Center = player.Center;
    }
}