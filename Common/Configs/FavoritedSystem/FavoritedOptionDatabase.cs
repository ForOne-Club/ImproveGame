namespace ImproveGame.Common.Configs.FavoritedSystem;

public class FavoritedOptionDatabase
{
    internal static HashSet<string> FavoritedOptions = new();

    /// <summary>
    /// 新增选项，用于在更新时提示玩家
    /// </summary>
    internal static HashSet<string> NewOptions => new()
    {
        "QuickRespawn",
        "NoPylonRestrictions",
        "NoSleepRestrictions",
        "BombsNotDamage",
        "LightNotBlocked"
    };

    public static void SetDefaultFavoritedOptions()
    {
        FavoritedOptions = new HashSet<string>
        {
            "SuperVault",
            "GrabDistance",
            "ExtraToolSpeed",
            "ModifyPlayerPlaceSpeed",
            "ModifyPlayerTileRange",
            "NPCCoinDropRate",
            "BannerRequirement",
            "ModifyNPCHappiness",
            "WandMaterialNoConsume"
        };
    }

    public static void ToggleFavoriteForOption(string name)
    {
        if (FavoritedOptions.Contains(name))
            FavoritedOptions.Remove(name);
        else
            FavoritedOptions.Add(name);

        AdditionalConfig.Save();
    }
}