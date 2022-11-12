using ImproveGame.Content.Items;

namespace ImproveGame.Content
{
    internal class ModItemID : ModSystem
    {
        public static int SpaceWand = ModContent.ItemType<SpaceWand>();
        public static int CreateWand = ModContent.ItemType<CreateWand>();
        public static int WallPlace = ModContent.ItemType<WallPlace>();
        public static HashSet<int> NoConsumptionItems = new() { SpaceWand, CreateWand, WallPlace };
    }
}
