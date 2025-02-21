using ImproveGame.Common.ModSystems;
using ReLogic.OS;

namespace ImproveGame.Common.GlobalItems;

public class KeybindItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (KeybindSystem.CopyItemNameKeybind.JustPressed)
        {
            if (KeybindSystem.CopyItemNameControlKeybind.Current)
                Platform.Get<IClipboard>().Value = (item.type < ItemID.Count ? "Terraria/" : "") + ItemID.Search.GetName(item.type);
            else
                Platform.Get<IClipboard>().Value = item.type < ItemID.Count ? ItemID.Search.GetName(item.type) : item.ModItem.Name;
        }
    }
}
