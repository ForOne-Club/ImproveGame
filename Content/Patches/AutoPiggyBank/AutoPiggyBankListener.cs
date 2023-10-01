namespace ImproveGame.Content.Patches.AutoPiggyBank;

public class AutoPiggyBankListener : GlobalItem
{
    public override bool OnPickup(Item item, Player player)
    {
        if (!Config.AutoSaveMoney || !Utils.HasItem(GetAllInventoryItemsList(player, "safe forge"), ItemID.PiggyBank, ItemID.ChesterPetItem, ItemID.MoneyTrough))
            return base.OnPickup(item, player);

        int type = item.type;

        if (type is ItemID.DefenderMedal)
        {
            PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
            SoundEngine.PlaySound(SoundID.Grab, player.position);
            item.StackToArray(player.bank.item);
            return false;
        }

        if (!Utils.IsCoin(type))
            return base.OnPickup(item, player);

        ulong totalMoney = Utils.CalculateCoinValue(type, (uint) item.stack);
        totalMoney = player.bank.item.Aggregate(totalMoney, (current, bItem) => current + Utils.CalculateCoinValue(bItem.type, (uint)bItem.stack));

        List<Item> toPlace = Utils.ConvertCopperValueToCoins(totalMoney);
        Utils.ReplaceOrPlaceIntoChest(player.bank, toPlace);

        PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
        SoundEngine.PlaySound(SoundID.CoinPickup, player.position);
        return false;
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (!Config.AutoSaveMoney || item.type is not ItemID.MoneyTrough and not ItemID.ChesterPetItem and not ItemID.PiggyBank)
            return base.UseItem(item, player);

        ulong totalMoney = 0;
        foreach (var t in player.inventory) {
            if (t.favorited)
                continue;

            if (t.type is ItemID.DefenderMedal)
                t.StackToArray(player.bank.item);

            if (!Utils.IsCoin(t.type))
                continue;

            totalMoney += Utils.CalculateCoinValue(t.type, (uint) t.stack);
            t.TurnToAir();
        }

        totalMoney = player.bank.item.Aggregate(totalMoney, (current, bItem) => current + Utils.CalculateCoinValue(bItem.type, (uint)bItem.stack));

        if (totalMoney == 0)
            return base.UseItem(item, player);

        List<Item> toPlace = Utils.ConvertCopperValueToCoins(totalMoney);
        Utils.ReplaceOrPlaceIntoChest(player.bank, toPlace);

        return base.UseItem(item, player);
    }
}