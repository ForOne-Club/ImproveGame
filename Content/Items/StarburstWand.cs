using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class StarburstWand : MagickWand
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.StarburstWand;

        public override void SetItemDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 5, 0, 0);

            SelectRange = new(40, 40);
            KillSize = new(7, 5);
            ExtraRange = new(16, 10);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.noBuilding)
                return false;
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<MagickWand>())
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.CrystalBall).Register();
        }
    }
}
