namespace ImproveGame.Content.Items.Coin;

public class DevMark : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Quest;
        Item.value = 0;
        Item.maxStack = 9999;
    }
}