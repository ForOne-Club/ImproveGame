namespace ImproveGame.Content.Items.Coin
{
    public class CoinOne : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.CoinLuckValue[Type] = 3000000;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(3);
            Item.maxStack = 9999;
        }
    }
}
