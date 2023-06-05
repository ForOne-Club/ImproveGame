using ImproveGame.Content.Items;

namespace ImproveGame.Content
{
    internal class ModItemID : ModSystem
    {
        public static readonly int SpaceWand = ModContent.ItemType<SpaceWand>();
        public static readonly int CreateWand = ModContent.ItemType<CreateWand>();
        public static readonly int WallPlace = ModContent.ItemType<WallPlace>();
        public static readonly HashSet<int> NoConsumptionItems = new() { SpaceWand, CreateWand, WallPlace };
    }
}
