using ImproveGame.Common.ModSystems;

namespace ImproveGame.Common.GlobalItems;

public class ItemLootDisplay : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!ItemLoader.CanRightClick(item))
            return;

        if (Main.ItemDropsDB.GetRulesForItemID(item.type).Count <= 0)
            return;

        bool hasKeybind = TryGetKeybindString(KeybindSystem.GrabBagKeybind, out var keybind);
        tooltips.Add(new TooltipLine(Mod, "LootDisplay", GetTextWith("Tips.LootDisplay", new { KeybindName = keybind }))
        {
            OverrideColor = Color.SkyBlue
        });
        if (!hasKeybind)
        {
            tooltips.Add(new TooltipLine(Mod, "LootDisplay", GetText("Tips.LootDisplayBindless"))
            {
                OverrideColor = Color.SkyBlue
            });
        }
    }
}
