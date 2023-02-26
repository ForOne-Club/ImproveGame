using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;

namespace ImproveGame.Common.GlobalItems;

internal class GrabAndPickup : GlobalItem
{
    // 两个单词
    // Grab 抓住
    // Pickup 捡起

    public override void Load()
    {
        // 抓取速度
        On.Terraria.Player.PullItem_Common += (orig, player, item, xPullSpeed) =>
        {
            if (Config.GrabDistance == 0)
            {
                orig(player, item, xPullSpeed);
            }
            else
            {
                float playerSpeed = player.velocity.Length() + 5f;
                if (playerSpeed < 15f)
                {
                    playerSpeed = 15f;
                }

                Vector2 normalize = (player.Center - item.Center).SafeNormalize(Vector2.Zero);
                if (item.velocity.Length() + normalize.Length() > playerSpeed)
                {
                    item.velocity = normalize * playerSpeed;
                }
                else
                {
                    item.velocity = normalize * (item.velocity.Length() + 1f);
                }
            }
        };

        // 拾取的物品进入背包前
        On.Terraria.Player.PickupItem += (orig, player, playerIndex, worldItemArrayIndex, itemToPickUp) =>
        {
            itemToPickUp = orig(player, playerIndex, worldItemArrayIndex, itemToPickUp);

            if (itemToPickUp.IsAir)
            {
                return itemToPickUp;
            }

            var improvePlayer = player.GetModPlayer<ImprovePlayer>();

            // 背包溢出堆叠至其他容器
            if (!itemToPickUp.IsACoin)
            {
                // 大背包
                if (Config.SuperVault && player.GetModPlayer<UIPlayerSetting>().SuperVault_OverflowGrab)
                {
                    itemToPickUp.StackToArray(player.GetModPlayer<DataPlayer>().SuperVault);
                }
                if (itemToPickUp.IsAir) goto Finish;

                // 猪猪 保险箱 ...
                if (Config.SuperVoidVault)
                {
                    if (improvePlayer.HasPiggyBank)
                    {
                        itemToPickUp.StackToArray(player.bank.item);
                    }
                    if (itemToPickUp.IsAir) goto Finish;

                    if (improvePlayer.HasSafe)
                    {
                        itemToPickUp.StackToArray(player.bank2.item);
                    }
                    if (itemToPickUp.IsAir) goto Finish;

                    if (improvePlayer.HasDefendersForge)
                    {
                        itemToPickUp.StackToArray(player.bank3.item);
                    }
                }
            }

            Finish:
            Main.item[worldItemArrayIndex] = itemToPickUp;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldItemArrayIndex);
            }
            return itemToPickUp;
        };
    }

    public override bool OnPickup(Item self, Player player)
    {
        var improvePlayer = player.GetModPlayer<ImprovePlayer>();
        var dataPlayer = player.GetModPlayer<DataPlayer>();

        // 旗帜盒
        if (improvePlayer.BannerChest is not null && improvePlayer.BannerChest.AutoStorage && ItemToBanner(self) != -1)
        {
            Item item = self.Clone();
            improvePlayer.BannerChest.PutInPackage(ref self);
            if (self.stack < item.stack)
            {
                item.stack -= self.stack;
                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, item.stack);
            }
        }

        // 药水袋
        if (improvePlayer.PotionBag is not null && improvePlayer.PotionBag.AutoStorage && self.buffType > 0 && self.consumable)
        {
            Item item = self.Clone();
            improvePlayer.PotionBag.PutInPackage(ref self);
            if (self.stack < item.stack)
            {
                item.stack -= self.stack;
                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, item.stack);
            }
        }

        // 大背包
        if (!self.IsAir && Config.SuperVault && player.GetModPlayer<UIPlayerSetting>().SuperVault_SmartGrab && self.InArray(dataPlayer.SuperVault))
        {
            Item oldItem = self.Clone();
            self.StackToArray(dataPlayer.SuperVault);
            PickupPopup(oldItem, self);
        }

        // 虚空保险库 之 智能收纳
        if (Config.SmartVoidVault && !self.IsAir && !self.IsACoin)
        {
            // 虚空保险库
            if (player.IsVoidVaultEnabled && self.InArray(player.bank4.item))
            {
                Item oldItem = self.Clone();
                self.StackToArray(player.bank4.item);
                PickupPopup(oldItem, self);
            }

            // 猪猪 保险箱 ...
            if (Config.SuperVoidVault && !self.IsAir)
            {
                if (improvePlayer.HasPiggyBank && self.InArray(player.bank.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank.item);
                    PickupPopup(oldItem, self);
                }

                if (improvePlayer.HasSafe && !self.IsAir && self.InArray(player.bank2.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank2.item);
                    PickupPopup(oldItem, self);
                }

                if (improvePlayer.HasDefendersForge && !self.IsAir && self.InArray(player.bank3.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank3.item);
                    PickupPopup(oldItem, self);
                }
            }
        }
        return true;
    }

    private static void PickupPopup(Item oldItem, Item self)
    {
        if (self.stack < oldItem.stack)
        {
            PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, oldItem, oldItem.stack - self.stack);
        }
    }

    // 抓取距离
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        grabRange += Config.GrabDistance * 16;
    }

    // ItemSpace
    public override bool ItemSpace(Item item, Player player)
    {
        if (item.IsACoin)
        {
            return false;
        }

        // 大背包
        if (Config.SuperVault && player.TryGetModPlayer(out UIPlayerSetting uiPlayerSetting) && player.TryGetModPlayer(out DataPlayer dataPlayer))
        {
            if (uiPlayerSetting.SuperVault_OverflowGrab)
            {
                if (item.CanStackToArray(dataPlayer.SuperVault))
                {
                    return true;
                }
            }
            else if (uiPlayerSetting.SuperVault_SmartGrab)
            {
                if (item.CanStackToArray(dataPlayer.SuperVault) && item.InArray(dataPlayer.SuperVault))
                {
                    return true;
                }
            }
        }

        if (Config.SuperVoidVault && player.TryGetModPlayer<ImprovePlayer>(out var improvePlayer))
        {
            if (improvePlayer.HasPiggyBank = improvePlayer.HasPiggyBank && item.CanStackToArray(player.bank.item))
            {
                return true;
            }

            if (improvePlayer.HasSafe = improvePlayer.HasSafe && item.CanStackToArray(player.bank2.item))
            {
                return true;
            }

            if (improvePlayer.HasDefendersForge = improvePlayer.HasDefendersForge && item.CanStackToArray(player.bank3.item))
            {
                return true;
            }
        }

        return false;
    }
}
