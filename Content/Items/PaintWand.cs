using ImproveGame.Common.Systems;

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

        public override bool AltFunctionUse(Player player) => true;

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(30, 30);
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
            //CreateRecipe()
            //    .AddRecipeGroup(RecipeGroupID.Wood, 18)
            //    .AddIngredient(ItemID.JungleSpores, 6)
            //    .AddIngredient(ItemID.Ruby, 1)
            //    .AddTile(TileID.WorkBenches).Register();
        }
    }
}
