using ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public static class ToolHandler
{
    private static readonly Dictionary<Type, ToolButton> Buttons;

    static ToolHandler()
    {
        Buttons = [];
        // RegisterButton<LootAllButton>();
        RegisterButton<SortButton>();
        RegisterButton<DepositAllButton>();
        RegisterButton<RecipeToggleButton>();
        RegisterButton<StackToInventoryButton>();
        RegisterButton<StackToStorageButton>();
        RegisterButton<AddChestButton>();

        // 筛选器按钮
        RegisterFilterButton<MeleeFilter>(ItemGroup.Weapon);
        RegisterFilterButton<RangedFilter>(ItemGroup.Weapon);
        RegisterFilterButton<MagicFilter>(ItemGroup.Weapon);
        RegisterFilterButton<SummonFilter>(ItemGroup.Weapon);
        RegisterFilterButton<OtherDamageFilter>(ItemGroup.Weapon);
        
        RegisterFilterButton<PickaxeFilter>(ItemGroup.Tool);
        RegisterFilterButton<AxeFilter>(ItemGroup.Tool);
        RegisterFilterButton<HammerFilter>(ItemGroup.Tool);
        RegisterFilterButton<HookFilter>(ItemGroup.Tool);
        RegisterFilterButton<WiringFilter>(ItemGroup.Tool);
        RegisterFilterButton<FishingPoleFilter>(ItemGroup.Tool);
        RegisterFilterButton<OtherToolFilter>(ItemGroup.Tool);
        
        RegisterFilterButton<ArrowFilter>(ItemGroup.Ammo);
        RegisterFilterButton<BulletFilter>(ItemGroup.Ammo);
        RegisterFilterButton<RocketFilter>(ItemGroup.Ammo);
        RegisterFilterButton<DartFilter>(ItemGroup.Ammo);
        RegisterFilterButton<OtherAmmoFilter>(ItemGroup.Ammo);
        
        RegisterFilterButton<HeadgearFilter>(ItemGroup.Armor);
        RegisterFilterButton<TorsoFilter>(ItemGroup.Armor);
        RegisterFilterButton<PantsFilter>(ItemGroup.Armor);
        RegisterFilterButton<VanityArmorFilter>(ItemGroup.Armor);
        
        RegisterFilterButton<BootFilter>(ItemGroup.Accessory);
        RegisterFilterButton<WingFilter>(ItemGroup.Accessory);
        RegisterFilterButton<ShieldFilter>(ItemGroup.Accessory);
        RegisterFilterButton<OtherAccessoryFilter>(ItemGroup.Accessory);
        RegisterFilterButton<VanityAccessoryFilter>(ItemGroup.Accessory);
        
        RegisterFilterButton<SolidFilter>(ItemGroup.Block);
        RegisterFilterButton<WallFilter>(ItemGroup.Block);
        RegisterFilterButton<PlatformFilter>(ItemGroup.Block);
        RegisterFilterButton<OtherTileFilter>(ItemGroup.Block);
        
        RegisterFilterButton<HerbFilter>(ItemGroup.Misc);
        RegisterFilterButton<SummonItemFilter>(ItemGroup.Misc);
        RegisterFilterButton<PetFilter>(ItemGroup.Misc);
        RegisterFilterButton<MountFilter>(ItemGroup.Misc);
        RegisterFilterButton<MaterialFilter>(ItemGroup.Misc);
        RegisterFilterButton<OtherMiscFilter>(ItemGroup.Misc);
    }

    public static bool TryGetFilterButtons(ItemGroup group, out IEnumerable<FilterButton> buttons)
    {
        buttons = Buttons
            .Where((pair => pair.Value is FilterButton filterButton && filterButton.Group == group))
            .Select(pair => (FilterButton) pair.Value);
        return buttons.Any();
    }

    public static bool TryGetButton<T>(out ToolButton button) where T : ToolButton
    {
        return Buttons.TryGetValue(typeof(T), out button);
    }

    public static bool TryGetButton(Type type, out ToolButton button)
    {
        return Buttons.TryGetValue(type, out button);
    }

    private static void RegisterButton<T>() where T : ToolButton
    {
        var type = typeof(T);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        if (ctor is null)
            throw new UsageException("ToolButton must have a parameterless constructor.");
        var instance = ctor.Invoke(null);
        Buttons.Add(type, (ToolButton) instance);
    }

    private static void RegisterFilterButton<T>(ItemGroup group) where T : FilterButton
    {
        var type = typeof(T);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        if (ctor is null)
            throw new UsageException("FilterButton must have a parameterless constructor.");
        var instance = (FilterButton) ctor.Invoke(null);
        instance.Group = group;
        Buttons.Add(type, instance);
    }
}