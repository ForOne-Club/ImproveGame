namespace ImproveGame.Content.Items;

public class WallPlaceSelectorMode : SelectorItem
{
    public override string Texture => "ImproveGame/Content/Items/WallPlace";

    public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.WallPlace;

    // 修改物块
    public override void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ)
    {
        for (int j = minJ; j <= maxJ; j++)
        {
            for (int i = minI; i <= maxI; i++)
            {
                WorldGen.SquareTileFrame(i, j, false);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.sendWater(i, j);
                else
                    Liquid.AddWater(i, j);
            }
        }
    }

    public override bool IsNeedKill() => !Main.mouseLeft;

    // 修改选择的方块
    public override bool ModifySelectedTiles(Player player, int i, int j)
    {
        Tile t = Main.tile[i, j];

        return true;
    }

    public override bool CanUseItem(Player player)
    {
        if (player.noBuilding)
            return false;
        return base.CanUseItem(player);
    }

    public override bool AltFunctionUse(Player player) => true;

    public override void SetItemDefaults()
    {
        Item.rare = ItemRarityID.Red;
        Item.value = Item.sellPrice(0, 3);
        // Item.mana = 20;

        SelectRange = new(30, 30);
    }

    public override bool StartUseItem(Player player)
    {
        switch (player.altFunctionUse)
        {
            case 0:
                ItemRotation(player);
                break;
            case 2:
                return false;
        }

        return base.StartUseItem(player);
    }
}