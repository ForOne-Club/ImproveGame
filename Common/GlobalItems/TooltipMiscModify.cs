using ImproveGame.Common.Systems;

namespace ImproveGame.Common.GlobalItems
{
    internal class TooltipMiscModify : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ModIntegrationsSystem.WMITFLoaded && Config.ShowModName &&
                item.type != ModIntegrationsSystem.UnloadedItemType &&
                (item.type != ModIntegrationsSystem.AprilFoolsItemType /*|| !AprilFools.CheckAprilFools()*/))
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
            if (item.DamageType == DamageClass.Summon && !item.sentry)
            {
                string key = "Tips.SummonSlot";
                if (Main.LocalPlayer.slotsMinions >= Main.LocalPlayer.maxMinions)
                {
                    key += "Full";
                }
                string text = GetTextWith(key, new {
                    Current = Math.Round(Main.LocalPlayer.slotsMinions, 2),
                    Total = Main.LocalPlayer.maxMinions
                });
                TooltipLine line = new(Mod, Mod.Name, text);
                tooltips.Add(line);
            }
        }
    }
}
