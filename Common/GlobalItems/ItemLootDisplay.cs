using ImproveGame.Common.Systems;
using System.Collections.Generic;

namespace ImproveGame.Common.GlobalItems
{
    public class ItemLootDisplay : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool hasLoot = Main.ItemDropsDB.GetRulesForItemID(item.type, includeGlobalDrops: false).Count > 0;
            if (hasLoot) {
                bool hasKeybind = TryGetKeybindString(KeybindSystem.GrabBagKeybind, out var keybind);
                tooltips.Add(new(Mod, "LootDisplay", GetTextWith("Tips.LootDisplay", new { KeybindName = keybind })) {
                    OverrideColor = Color.SkyBlue
                });
                if (!hasKeybind)
                {
                    tooltips.Add(new(Mod, "LootDisplay", GetText("Tips.LootDisplayBindless")){
                        OverrideColor = Color.SkyBlue
                    });
                }
            }
        }
    }
}
