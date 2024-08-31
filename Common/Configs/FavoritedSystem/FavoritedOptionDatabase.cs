using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs.FavoritedSystem;

public class FavoritedOptionDatabase
{
    internal static HashSet<string> FavoritedOptions = [];

    /// <summary>
    /// 新增选项，用于在更新时提示玩家
    /// </summary>
    internal static HashSet<string> NewOptions =>
    [
        nameof(ImproveConfigs.AmmoChain),
        nameof(ImproveConfigs.SimpleVeinMining),
        nameof(ImproveConfigs.DisableVeinMiningPopup)
    ];

    public static void SetDefaultFavoritedOptions()
    {
        FavoritedOptions =
        [
            "ImproveConfigs.SuperVault",
            "ImproveConfigs.GrabDistance",
            "ImproveConfigs.ExtraToolSpeed",
            "ImproveConfigs.ModifyPlayerPlaceSpeed",
            "ImproveConfigs.ModifyPlayerTileRange",
            "ImproveConfigs.NPCCoinDropRate",
            "ImproveConfigs.BannerRequirement",
            "ImproveConfigs.ModifyNPCHappiness",
            "ImproveConfigs.WandMaterialNoConsume"
        ];
    }

    public static void ToggleFavoriteForOption(ModConfig config, string optionName)
    {
        string name = $"{config.Name}.{optionName}";
        if (!FavoritedOptions.Add(name))
            FavoritedOptions.Remove(name);

        AdditionalConfig.Save();
    }
}