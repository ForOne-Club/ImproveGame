using ImproveGame.Content.Items;
using ImproveGame.Content.Items.Globes;
using ImproveGame.Content.Items.ItemContainer;
using ImproveGame.Content.Items.Placeable;

namespace ImproveGame.Content
{
    public class SLU : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (ModLoader.TryGetMod("ShopLookup", out Mod mod))
            {
                mod.Call(3, Mod, "Wand", TextureAssets.Item[ModContent.ItemType<StarburstWand>()].Value, Wand());
                mod.Call(3, Mod, "Locator", TextureAssets.Item[ModContent.ItemType<AetherGlobe>()].Value, Locator());
                mod.Call(3, Mod, "Other", TextureAssets.Item[ModContent.ItemType<ExtremeStorage>()].Value, Other());
            }
        }
        private static NPCShop Wand()
        {
            return new NPCShop(-1, "Wand")
                .Add<CreateWand>()
                .Add<MagickWand>()
                .Add<PaintWand>()
                .Add<MoveChest>()
                .Add<WallPlace>(Condition.DownedKingSlime)
                .Add<SpaceWand>(Condition.DownedKingSlime)
                .Add<LiquidWand>(Condition.DownedEowOrBoc)
                .Add<StarburstWand>(Condition.Hardmode)
                .Add<ConstructWand>(Condition.Hardmode);
        }
        private static NPCShop Locator()
        {
            return new NPCShop(-1, "Locator")
                .Add<FloatingIslandGlobe>()
                .Add<PyramidGlobe>()
                .Add<AetherGlobe>()
                .Add<DungeonGlobe>()
                .Add<EnchantedSwordGlobe>()
                .Add<PlanteraGlobe>(Condition.Hardmode)
                .Add<TempleGlobe>(Condition.DownedPlantera);
        }
        private static NPCShop Other()
        {
            return new NPCShop(-1, "Other")
                .Add<BannerChest>()
                .Add<ExtremeStorage>()
                .Add<Autofisher>()
                .Add<DetectorDrone>()
                .Add<StorageCommunicator>()
                .Add<BaitSupplier>()
                .Add<PotionBag>()
                .Add<Dummy>()
                .Add<WeatherBook>(Condition.DownedEowOrBoc);
        }
    }
}
