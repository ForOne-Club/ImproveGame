using Terraria.GameContent.Creative;

namespace ImproveGame.UI.ExtremeStorage;

public static class FilterHelper
{
    public static HashSet<int> SpecialToolIds => new ItemFilters.Tools()._itemIdsThatAreAccepted;

    public static bool IsTool(this Item item) => IsOrdinaryTool(item) || item.IsHook() || item.fishingPole > 0 ||
                                                 IsWiringTool(item) || IsOtherTool(item);

    public static bool IsAccessory(this Item item) => item.accessory;

    public static bool IsAmmo(this Item item) => item.ammo != AmmoID.None && !item.notAmmo;

    public static bool IsArmor(this Item item) => item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0;

    public static bool IsPlaceable(this Item item) => item.createTile >= TileID.Dirt || item.createWall > WallID.None;

    public static bool IsWeapon(this Item item) =>
        item.damage > 0 && item.axe is 0 && item.hammer is 0 && item.pick is 0;

    public static ContentSamples.CreativeHelper.ItemGroup GetCreativeItemGroup(this Item item) =>
        ContentSamples.CreativeHelper.GetItemGroup(item, out _);

    public static bool IsOrdinaryTool(this Item item) => item.axe != 0 || item.hammer != 0 || item.pick != 0;

    public static bool IsOtherTool(this Item item) => SpecialToolIds.Contains(item.type);

    public static bool IsWiringTool(this Item item) =>
        GetCreativeItemGroup(item) is ContentSamples.CreativeHelper.ItemGroup.Wiring;

    public static bool IsHerb(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.AlchemyPlants
        or ContentSamples.CreativeHelper.ItemGroup.AlchemySeeds;

    public static bool IsSummonItem(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.EventItem
        or ContentSamples.CreativeHelper.ItemGroup.BossItem;

    public static bool IsPet(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.VanityPet
        or ContentSamples.CreativeHelper.ItemGroup.LightPet;

    public static bool IsMount(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.Mount;

    public static bool IsMaterial(this Item item) => item.material;
}