namespace ImproveGame.Content.Items.Coin
{
    public class CoinOne : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("幸运金币");
            Tooltip.SetDefault("带在身上会有好事发生!!!");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Red;

            Item.value = Item.sellPrice(1);
        }
    }
}
