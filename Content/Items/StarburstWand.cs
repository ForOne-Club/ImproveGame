using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class StarburstWand : MagickWand
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.width = 40;
            Item.height = 46;
            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.mana = 20;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 2, 0, 0);

            KillTilesOffsetX = -3;
            KillTilesOffsetY = -2;
            KillTilesWidth = 7;
            KillTilesHeight = 5;
            RangeX = 10;
            RangeY = 8;
            OpenUI = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<MagickWand>())
                .AddIngredient(520, 5)
                .AddIngredient(521, 5)
                .AddIngredient(527, 1)
                .AddIngredient(528, 1)
                .AddTile(125).Register();
        }
    }
}
