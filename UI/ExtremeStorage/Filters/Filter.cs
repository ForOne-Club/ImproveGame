namespace ImproveGame.UI.ExtremeStorage.Filters;

public abstract class Filter
{
    public static readonly Dictionary<ItemGroup, Filter> Filters = new()
    {
        {ItemGroup.Weapon, new WeaponFilter()},
        {ItemGroup.Tool, new ToolFilter()},
        {ItemGroup.Ammo, new AmmoFilter()},
        {ItemGroup.Armor, new ArmorFilter()},
        {ItemGroup.Accessory, new AccessoryFilter()},
        {ItemGroup.Block, new BlockFilter()},
        {ItemGroup.Misc, new MiscFilter()}
    };

    public bool Check(Item item) => !item.IsAir && ShouldInclude(item);

    protected abstract bool ShouldInclude(Item item);
}