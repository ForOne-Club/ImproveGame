using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Tiles;
using Terraria.DataStructures;

namespace ImproveGame.Common.GlobalItems;

public class AutofishItemListener : GlobalItem
{
    internal static TEAutofisher ListeningAutofisher;

    // 监听有没有额外生成的物品
    public override void OnSpawn(Item item, IEntitySource source)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient || ListeningAutofisher is null)
            return;

        ListeningAutofisher.DirectAddItemToStorage(item);
        item.TurnToAir();
    }

    // 给饰品添加加成显示工具提示
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        // 处理特殊标识，这个标识在AutofisherGUI中被应用到Main.HoverItem身上，用于特异性识别要添加tooltip的物品
        if (item.playerIndexTheItemIsReservedFor is not 254)
            return;
        if (AutofishPlayer.LocalPlayer.Autofisher is not { } autofisher)
            return;

        bool accLegal = ModIntegrationsSystem.FishingStatLookup.TryGetValue(item.type, out FishingStat stat);
        if (!accLegal)
            return;

        int iconId = stat.LavaFishing ? ItemID.LavaproofTackleBag : ItemID.AnglerTackleBag;
        // 将stat.SpeedMultiplier转为百分比形式
        string speed = stat.SpeedMultiplier.ToString("P0");
        string text = GetTextWith("UI.Autofisher.AccBoost", new { IconID = iconId, Speed = speed, stat.Power });
        if (stat.TackleBox)
            text += $"\n{GetText("UI.Autofisher.AccBoostTackle")}";
        if (stat.LavaFishing)
            text += $"\n{GetText("UI.Autofisher.AccBoostLava")}";
        tooltips.Add(new TooltipLine(Mod, "AutofisherAccBoost", text) {OverrideColor = Color.Pink});
    }
}