using ImproveGame.Content.Items.Coin;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class ModItemSettings : Category
{
    public override int ItemIconId => ModContent.ItemType<Content.Items.Placeable.ExtremeStorage>();

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.EmptyAutofisher));
        panel.AddValueSlider(Config, nameof(Config.ExStorageSearchDistance));
        panel.AddToggle(Config, nameof(Config.WandMaterialNoConsume));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableMagickWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableSpaceWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableStarburstWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableWallPlace));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableCreateWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableLiquidWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableLiquidWandAdvanced));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailablePotionBag));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableBannerChest));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableAutofisher));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailablePaintWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableConstructWand));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableMoveChest));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableCoinOne));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableExtremeStorage));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableDetectorDrone));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableBaitSupplier));
        panel.AddToggle(AvailableConfig, nameof(AvailableConfig.AvailableActuationRodMkII));
    }
}