using ImproveGame.Common.ModHooks;
using ImproveGame.Content.BuilderToggles;
using ImproveGame.Core;
using Terraria.GameContent.UI;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.AutoPiggyBank;

// 试存钱币槽
public class AutoMoneyPlayerListener : ModPlayer, IHookPostSetup
{
    public static AutoMoneyPlayerListener LocalPlayer => Main.LocalPlayer.GetModPlayer<AutoMoneyPlayerListener>();
    public List<ItemTypeData> ExcludedItems = [];
    public static List<Item> AllCurrencies = [];
    internal bool AutoSaveUnlocked;
    private int _detectCd;

    public override void SaveData(TagCompound tag)
    {
        tag["unlocked"] = AutoSaveUnlocked;
        tag["excludedItems"] = ExcludedItems;
    }

    public override void LoadData(TagCompound tag)
    {
        AutoSaveUnlocked = tag.GetBool("unlocked");
        ExcludedItems = tag.Get<List<ItemTypeData>>("excludedItems") ?? [];
    }

    public void PostSetupContent()
    {
        AllCurrencies = [new Item(ItemID.GoldCoin)];
        AllCurrencies.AddRange(ContentSamples.ItemsByType
            .Where(i => CustomCurrencyManager.IsCustomCurrency(i.Value))
            .Select(i => new Item(i.Key)) // 实例化一个新的，直接用ContentSamples.ItemsByType里面的会出问题
            .ToList());
    }

    public override void PostUpdate()
    {
        _detectCd++;

        if (Main.myPlayer != Player.whoAmI)
            return;

        // 看看能不能解锁自动存钱
        if (!AutoSaveUnlocked && _detectCd % 90 == 0)
        {
            AutoSaveUnlocked = InventoryHasItemFast(Main.LocalPlayer,
                ItemID.PiggyBank, ItemID.ChesterPetItem, ItemID.MoneyTrough);
        }

        if (!PiggyToggle.AutoSaveEnabled)
            return;

        // 10帧检测一次，仅存钱币槽
        if (_detectCd % 10 == 0 && !ExcludedItems.Any(i => i.Item.type is ItemID.GoldCoin))
            DetectCoins();
        // 60帧检测一次，自定义钱币
        if (_detectCd % 60 == 0)
            DetectCustomCurrency();
        // 30帧更新一下铂金最大堆叠
        if (_detectCd % 30 == 0)
            CoinUtils.PlatinumMaxStack = new Item(ItemID.PlatinumCoin).maxStack;
    }

    private void DetectCoins()
    {
        bool isDepositSucceed = false;
        for (var i = 50; i <= 53; i++)
        {
            var item = Player.inventory[i];
            if (!item.IsAir && item.IsACoin && AutoMoneyItemListener.TryDepositACoin(item, Player))
            {
                isDepositSucceed = true;
                item.TurnToAir();
            }
        }

        if (isDepositSucceed)
            Recipe.FindRecipes();
    }

    private void DetectCustomCurrency()
    {
        bool isDepositSucceed = false;
        for (var i = 0; i <= 49; i++)
        {
            var item = Player.inventory[i];
            bool disabled = ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, item));
            if (!item.IsAir && CustomCurrencyManager.IsCustomCurrency(item) && !disabled &&
                AutoMoneyItemListener.TryDepositACoin(item, Player))
            {
                isDepositSucceed = true;
                item.TurnToAir();
            }
        }

        if (isDepositSucceed)
            Recipe.FindRecipes();
    }

    // 返回的是物品禁用状态，true就是没禁用，false就是禁用了
    public bool ToggleItem(Item item)
    {
        bool alreadyExcluded = ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, item));
        if (alreadyExcluded)
        {
            ExcludedItems.RemoveAll(i => ItemExtensions.IsSameItem(i.Item, item));
            return true;
        }

        ExcludedItems.Add(new ItemTypeData(item));
        return false;
    }
}