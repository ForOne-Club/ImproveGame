using ImproveGame.Common.Conditions;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModSystems;

namespace ImproveGame.Content.Items
{
    public class StarburstWand : MagickWand, IConditionItem
    {
        public new Condition UseCondition => ConfigCondition.AvailableStarburstWandC;

        public override void SetItemDefaults()
        {
            base.SetItemDefaults();
            Item.width = 40;
            Item.height = 46;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 8, 0, 0);

            SelectRange = new(40, 40);
            KillSize = new(7, 5);
            ExtraRange = new(16, 10);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.noBuilding)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeSystem.AnyCobaltBar, 10)
                .AddIngredient(ItemID.Diamond, 1)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.CrystalBall)
                .Register();

            CreateRecipe().AddIngredient(ModContent.ItemType<MagickWand>())
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
