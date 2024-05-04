namespace ImproveGame.Common.ModHooks;

/// <summary>
/// 所有IL, On调用的ModHook都放在这里
/// </summary>
public class ModHookSystem : ModSystem
{
    public override void Load()
    {
        On_ItemSlot.OverrideLeftClick += ApplyLeftClick;
    }

    private bool ApplyLeftClick(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot)
    {
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            bool result = false;

            foreach (var globalItem in from i in GlobalList<GlobalItem>.Globals where i is IItemOverrideLeftClick
                     select i)
                result |= ((IItemOverrideLeftClick)globalItem).OverrideLeftClick(inv, context, slot);

            if (inv[slot].ModItem is IItemOverrideLeftClick)
                result |= ((IItemOverrideLeftClick) inv[slot].ModItem).OverrideLeftClick(inv, context, slot);

            if (!result)
                return orig.Invoke(inv, context, slot);

            return true;
        }

        return orig.Invoke(inv, context, slot);
    }
}