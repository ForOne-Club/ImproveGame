namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public static class ToolHandler
{
    private static readonly Dictionary<Type, ToolButtonBase> Buttons;

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
    }
    
    public static bool TryGetButton<T>(out ToolButtonBase button) where T : ToolButtonBase
    {
        return TryGetButton(typeof(T), out button);
    }
    
    public static bool TryGetButton(Type type, out ToolButtonBase button)
    {
        return Buttons.TryGetValue(type, out button);
    }
    
    private static void RegisterButton<T>() where T : ToolButtonBase
    {
        var type = typeof(T);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        if (ctor is null)
            throw new UsageException("ToolButtonBase must have a parameterless constructor.");
        var instance = ctor.Invoke(null);
        Buttons.Add(typeof(T), (ToolButtonBase) instance);
    }
}