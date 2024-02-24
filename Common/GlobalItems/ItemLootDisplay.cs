using ImproveGame.Common.ModSystems;

namespace ImproveGame.Common.GlobalItems;

public class ItemLootDisplay : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        // ItemLoader.CanRightClick 里面只要 Main.mouseRight == false 就直接返回了，不知道为什么
        // 所以这里必须伪装成右键点击
        bool oldRight = Main.mouseRight;
        Main.mouseRight = true;
        if (!CollectHelper.ItemCanRightClick[item.type] && !ItemLoader.CanRightClick(item))
        {
            Main.mouseRight = oldRight;
            return;
        }
        Main.mouseRight = oldRight;

        if (Main.ItemDropsDB.GetRulesForItemID(item.type).Count <= 0) return;

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
