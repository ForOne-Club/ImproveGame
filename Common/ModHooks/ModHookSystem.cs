using ImproveGame.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ImproveGame.Common.ModHooks
{
    /// <summary>
    /// 所有IL, On调用的ModHook都放在这里
    /// </summary>
    public class ModHookSystem : ModSystem
    {
        // 现在还没开放OverrideHover的接口，只能On了
        // 等我的PR: https://github.com/tModLoader/tModLoader/pull/2620 被接受就可以改掉了
        public override void Load() {
            On.Terraria.UI.ItemSlot.OverrideHover_ItemArray_int_int += ApplyHover;
        }

        // 为了确保只有在物品栏才能用
        private void ApplyHover(On.Terraria.UI.ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            bool result = false;
            if (Main.LocalPlayer.TryGetModPlayer<ShiftClickSlotPlayer>(out var modPlayer))
                result |= modPlayer.ApplyHover(inv, context, slot);
            if (inv[slot].ModItem is IItemOverrideHover)
                result |= (inv[slot].ModItem as IItemOverrideHover).OverrideHover(inv, context, slot);
            if (!result)
                orig.Invoke(inv, context, slot);
        }
    }
}
