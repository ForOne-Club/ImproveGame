using ImproveGame.Common.ModSystems;

namespace ImproveGame.Common.GlobalItems
{
    internal class TooltipMiscModify : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ModIntegrationsSystem.WMITFLoaded && Config.ShowModName &&
                item.type != ModIntegrationsSystem.UnloadedItemType)
            {
                if (item.ModItem is not null && !item.Name.Contains("[" + item.ModItem.Mod.Name + "]") && !item.Name.Contains("[" + item.ModItem.Mod.DisplayName + "]"))
                {
                    string text = GetTextWith("Tips.FromMod", new { item.ModItem.Mod.DisplayName });
                    TooltipLine line = new(Mod, Mod.Name, text)
                    {
                        OverrideColor = Colors.RarityBlue
                    };
                    tooltips.Add(line);
                }
            }
        }
    }
}
