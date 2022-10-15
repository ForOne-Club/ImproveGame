using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items
{
    public class PaintWand : SelectorItem
    {
        public override bool ModifySelectedTiles(Player player, int i, int j)
        {
            var tile = Main.tile[i, j];
            bool tileMode = WandSystem.PaintWandMode == WandSystem.PaintMode.Tile;
            bool wallMode = WandSystem.PaintWandMode == WandSystem.PaintMode.Wall;
            bool removeMode = WandSystem.PaintWandMode == WandSystem.PaintMode.Remove;
            if (removeMode)
            {
                if (tile.TileColor > 0 && tile.HasTile)
                {
                    WorldGen.paintTile(i, j, 0, broadCast: true);
                }
                if (tile.WallColor > 0 && tile.WallType > 0)
                {
                    WorldGen.paintWall(i, j, 0, broadCast: true);
                }

                // 漆铲可用于清除图格上的苔藓
                if (tile.TileType != TileID.LongMoss)
                    return true;

                WorldGen.KillTile(i, j);
                if (tile.HasTile)
                    return true;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);

                if (Main.rand.NextBool(9))
                {
                    int frameX = tile.TileFrameX;
                    int type = 4349 + frameX / 22;
                    switch (frameX / 22)
                    {
                        case 6:
                            type = 4377;
                            break;
                        case 7:
                            type = 4378;
                            break;
                        case 8:
                            type = 4389;
                            break;
                    }

                    int number = Item.NewItem(new EntitySource_ItemUse(player, player.HeldItem), player.Center, 16, 16, type);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }
            else
            {
                PickItemInInventory(
                    player,
                    (Item item) =>
                        item.paint > 0 &&
                        ((tileMode && tile.TileColor != item.paint) ||
                        (wallMode && tile.WallColor != item.paint)),
                    false,
                    out int index
                );
                if (index == -1)
                    return true;
                var item = player.inventory[index];
                if (index != -1)
                {
                    // 原版逻辑
                    if ((tileMode && WorldGen.paintTile(i, j, item.paint, broadCast: true)) || WorldGen.paintWall(i, j, item.paint, broadCast: true))
                    {
                        if (item.stack >= 999)
                            return true;
                        if (ItemLoader.ConsumeItem(item, player))
                            item.stack--;
                        if (item.stack <= 0)
                            item.SetDefaults();
                    }
                }
            }
            return true;
        }

        public override bool IsNeedKill()
        {
            return !Main.mouseLeft;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(1000, 1000);
        }

        public override bool StartUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                UISystem.Instance.PaintWandGUI.ToggleMode();
                return false;
            }

            return base.StartUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddRecipeGroup(RecipeGroupID.IronBar, 6)
                .AddIngredient(ItemID.Paintbrush, 1)
                .AddIngredient(ItemID.PaintRoller, 1)
                .AddIngredient(ItemID.PaintScraper, 1)
                .AddTile(TileID.Anvils).Register();
        }
    }
}
