namespace ImproveGame.Common.Conditions
{
    public static class ConfigCondition
    {
        public static Condition AvailableAutofisherC = new Condition("Mods.ImproveGame.Conditions.AvailableAutofisher", () => AvailableConfig.AvailableAutofisher);
        public static Condition AvailableActuationRodMkIIC = new Condition("Mods.ImproveGame.Conditions.AvailableActuationRodMkII", () => AvailableConfig.AvailableActuationRodMkII);
        public static Condition AvailableBaitSupplierC = new Condition("Mods.ImproveGame.Conditions.AvailableBaitSupplier", () => AvailableConfig.AvailableBaitSupplier);
        public static Condition AvailableExtremeStorageC = new Condition("Mods.ImproveGame.Conditions.AvailableExtremeStorage", () => AvailableConfig.AvailableExtremeStorage);
        public static Condition AvailableLiquidWandC = new Condition("Mods.ImproveGame.Conditions.AvailableLiquidWand", () => AvailableConfig.AvailableLiquidWand);
        public static Condition NotAvailableLiquidWandC = new Condition("Mods.ImproveGame.Conditions.NotAvailableLiquidWand", () => !AvailableConfig.AvailableLiquidWand);
        public static Condition AvailableLiquidWandAdvancedC = new Condition("Mods.ImproveGame.Conditions.AvailableLiquidWandAdvanced", () => AvailableConfig.AvailableLiquidWandAdvanced);
        public static Condition AvailableMagickWandC = new Condition("Mods.ImproveGame.Conditions.AvailableMagickWand", () => AvailableConfig.AvailableMagickWand);
        public static Condition NotAvailableMagickWandC = new Condition("Mods.ImproveGame.Conditions.NotAvailableMagickWand", () => !AvailableConfig.AvailableMagickWand);
        public static Condition AvailableStarburstWandC = new Condition("Mods.ImproveGame.Conditions.AvailableStarburstWand", () => AvailableConfig.AvailableStarburstWand);
        public static Condition AvailablePotionBagC = new Condition("Mods.ImproveGame.Conditions.AvailablePotionBag", () => AvailableConfig.AvailablePotionBag);
        public static Condition AvailableSpaceWandC = new Condition("Mods.ImproveGame.Conditions.AvailableSpaceWand", () => AvailableConfig.AvailableSpaceWand);
        public static Condition AvailableWallPlaceC = new Condition("Mods.ImproveGame.Conditions.AvailableWallPlace", () => AvailableConfig.AvailableWallPlace);
        public static Condition AvailablePaintWandC = new Condition("Mods.ImproveGame.Conditions.AvailablePaintWand", () => AvailableConfig.AvailablePaintWand);
        public static Condition AvailableBannerChestC = new Condition("Mods.ImproveGame.Conditions.AvailableBannerChest", () => AvailableConfig.AvailableBannerChest);
        public static Condition AvailableConstructWandC = new Condition("Mods.ImproveGame.Conditions.AvailableConstructWand", () => AvailableConfig.AvailableConstructWand);
        public static Condition AvailableCreateWandC = new Condition("Mods.ImproveGame.Conditions.AvailableCreateWand", () => AvailableConfig.AvailableCreateWand);
        public static Condition AvailableDetectorDroneC = new Condition("Mods.ImproveGame.Conditions.AvailableDetectorDrone", () => AvailableConfig.AvailableDetectorDrone);
        public static Condition AvailableMoveChestC = new Condition("Mods.ImproveGame.Conditions.AvailableMoveChest", () => AvailableConfig.AvailableMoveChest);
        public static Condition EnableQuickShimmerC = new Condition("Mods.ImproveGame.Conditions.EnableQuickShimmer", () => Config.QuickShimmer);
        public static Condition EnableMinimapMarkC = new Condition("Mods.ImproveGame.Conditions.EnableMinimapMark", () => Config.MinimapMark);
        public static Condition EnableWeatherControlC = new Condition("Mods.ImproveGame.Conditions.EnableWeatherControl", () => Config.WeatherControl);
    }
}
