namespace ImproveGame.Common.Utils;

public static class NPCHelper
{
    public static void SetBaseValues(this NPC npc, int width, int height, int lifeMax, bool friendly, int value,
        int damage = 0, int defense = 0, float knockBackResist = 0f)
    {
        npc.width = width;
        npc.height = height;
        npc.lifeMax = lifeMax;
        npc.friendly = friendly;
        npc.value = value;
        npc.damage = damage;
        npc.defense = defense;
        npc.knockBackResist = knockBackResist;
    }

    public static NPCShop Add<T>(this NPCShop shop, int customBuyPrice, params Condition[] condition)
        where T : ModItem =>
        shop.Add(new NPCShop.Entry(new Item(ModContent.ItemType<T>())
        {
            shopCustomPrice = customBuyPrice
        }, condition));

    public static NPCShop Add<T>(this NPCShop shop, Condition condition, int platinum = 0, int gold = 0, int silver = 0)
        where T : ModItem =>
        shop.Add(new NPCShop.Entry(new Item(ModContent.ItemType<T>())
        {
            shopCustomPrice = Item.buyPrice(platinum, gold, silver)
        }, condition));

    public static NPCShop Add<T>(this NPCShop shop, int platinum = 0, int gold = 0, int silver = 0)
        where T : ModItem =>
        shop.Add(new NPCShop.Entry(new Item(ModContent.ItemType<T>())
        {
            shopCustomPrice = Item.buyPrice(platinum, gold, silver)
        }));
}