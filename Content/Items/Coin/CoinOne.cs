namespace ImproveGame.Content.Items.Coin
{
    public class CoinOne : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;

            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(1);
        }
    }
}
