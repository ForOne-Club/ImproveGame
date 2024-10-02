using ImproveGame.UI;

namespace ImproveGame.Content.Items.IconDummies;

public class LicenseBook : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Book}";

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Book);
        Item.createTile = -1;
        Item.consumable = false;
        Item.maxStack = 1;
        Item.rare = ItemRarityID.Quest;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer || !player.ItemAnimationJustStarted)
            return true;

        if (LicensePanel.Instance.Enabled)
            LicensePanel.Instance.Close();
        else
            LicensePanel.Instance.Open();

        return true;
    }
}