namespace ImproveGame.Common.Conditions
{
    public static class ConfigCondition
    {
        public static Condition AvailableAutofisherC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableAutofisher", () => AvailableConfig.AvailableAutofisher);
        public static Condition AvailableActuationRodMkIIC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableActuationRodMkII", () => AvailableConfig.AvailableActuationRodMkII);
        public static Condition AvailableBaitSupplierC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableBaitSupplier", () => AvailableConfig.AvailableBaitSupplier);
        public static Condition AvailableExtremeStorageC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableExtremeStorage", () => AvailableConfig.AvailableExtremeStorage);
        public static Condition AvailableLiquidWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableLiquidWand", () => AvailableConfig.AvailableLiquidWand);
        public static Condition NotAvailableLiquidWandC { get; } = new Condition("Mods.ImproveGame.Conditions.NotAvailableLiquidWand", () => !AvailableConfig.AvailableLiquidWand);
        public static Condition AvailableLiquidWandAdvancedC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableLiquidWandAdvanced", () => AvailableConfig.AvailableLiquidWandAdvanced);
        public static Condition AvailableMagickWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableMagickWand", () => AvailableConfig.AvailableMagickWand);
        public static Condition NotAvailableMagickWandC { get; } = new Condition("Mods.ImproveGame.Conditions.NotAvailableMagickWand", () => !AvailableConfig.AvailableMagickWand);
        public static Condition AvailableStarburstWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableStarburstWand", () => AvailableConfig.AvailableStarburstWand);
        public static Condition AvailablePotionBagC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailablePotionBag", () => AvailableConfig.AvailablePotionBag);
        public static Condition AvailableSpaceWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableSpaceWand", () => AvailableConfig.AvailableSpaceWand);
        public static Condition AvailableWallPlaceC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableWallPlace", () => AvailableConfig.AvailableWallPlace);
        public static Condition AvailablePaintWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailablePaintWand", () => AvailableConfig.AvailablePaintWand);
        public static Condition AvailableBannerChestC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableBannerChest", () => AvailableConfig.AvailableBannerChest);
        public static Condition AvailableConstructWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableConstructWand", () => AvailableConfig.AvailableConstructWand);
        public static Condition AvailableCreateWandC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableCreateWand", () => AvailableConfig.AvailableCreateWand);
        public static Condition AvailableDetectorDroneC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableDetectorDrone", () => AvailableConfig.AvailableDetectorDrone);
        public static Condition AvailableMoveChestC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableMoveChest", () => AvailableConfig.AvailableMoveChest);
        public static Condition AvailableShimmerBucketC { get; } = new Condition("Mods.ImproveGame.Conditions.AvailableShimmerBucket", () => AvailableConfig.AvailableShimmerBucket);
        public static Condition EnableQuickShimmerC { get; } = new Condition("Mods.ImproveGame.Conditions.EnableQuickShimmer", () => Config.QuickShimmer);
        public static Condition EnableMinimapMarkC { get; } = new Condition("Mods.ImproveGame.Conditions.EnableMinimapMark", () => Config.MinimapMark);
        public static Condition EnableWeatherControlC { get; } = new Condition("Mods.ImproveGame.Conditions.EnableWeatherControl", () => Config.WeatherControl);
    }
}
