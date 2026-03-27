using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs.FavoritedSystem;

public class FavoritedOptionDatabase
{
    internal static HashSet<string> FavoritedOptions = [];

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

    public static bool IsFavorited(ModConfig config, string optionName) =>
        FavoritedOptions.Contains($"{config.Name}.{optionName}");

    public static void ToggleFavoriteForOption(ModConfig config, string optionName)
    {
        string name = $"{config.Name}.{optionName}";
        if (!FavoritedOptions.Add(name))
            FavoritedOptions.Remove(name);

        ClientConfigCore.SaveConfig();
    }
}