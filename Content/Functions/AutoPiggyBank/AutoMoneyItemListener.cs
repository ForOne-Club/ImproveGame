using ImproveGame.Content.BuilderToggles;
using Terraria.GameContent.UI;

namespace ImproveGame.Content.Functions.AutoPiggyBank;

public class AutoMoneyItemListener : GlobalItem
{
    public override bool OnPickup(Item item, Player player)
    {
        if (item.type is ItemID.PiggyBank or ItemID.ChesterPetItem or ItemID.MoneyTrough &&
            player.TryGetModPlayer<AutoMoneyPlayerListener>(out var listener) && !listener.AutoSaveUnlocked)
            listener.AutoSaveUnlocked = true;

        if (PiggyToggle.AutoSaveEnabled is 0 || (PiggyToggle.AutoSaveEnabled is 1 && item.ModItem != null))
            return base.OnPickup(item, player);

        // 没存成就执行原来的Pickup
        return !TryDepositCustomCurrency(item, player);
    }

    /// <summary>
    /// 存一个自定义货币，注意不能存钱币
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="player">玩家</param>
    /// <returns>是否成功</returns>
    public static bool TryDepositCustomCurrency(Item item, Player player)
    {
        if (!CustomCurrencyManager.IsCustomCurrency(item))
            return false;

        // 无空位
        if (!player.bank.item.Any(i =>
        {
            if (item.type == i.type && i.stack < i.maxStack)
                return true;

            return i.IsAir;
        }))
            return false;

        int type = item.type;

        PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
        SoundEngine.PlaySound(SoundID.Grab, player.position);
        item.StackToArray(player.bank.item);
        return item.IsAir;
    }
}