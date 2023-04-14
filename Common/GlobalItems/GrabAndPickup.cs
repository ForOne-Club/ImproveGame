using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;

namespace ImproveGame.Common.GlobalItems;

internal class GrabAndPickup : GlobalItem
{
    // 两个单词
    // Grab 抓住（游戏内指代使物品飞向玩家的操作）
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
                // 浓缩的都是精华
                item.velocity = Vector2.Normalize(player.Center - item.Center) * Math.Clamp(item.velocity.Length() + 1f, 0f, Math.Max(player.velocity.Length() + 5f, 15f));
            }
        };

        // 拾取的物品溢出背包后
        On.Terraria.Player.PickupItem += (orig, player, playerIndex, worldItemArrayIndex, itemToPickUp) =>
        {
            itemToPickUp = orig(player, playerIndex, worldItemArrayIndex, itemToPickUp);

            if (itemToPickUp.IsAir)
            {
                return itemToPickUp;
            }

            Item cloneItem = itemToPickUp.Clone();

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
                if (Config.SuperVoidVault && player.TryGetModPlayer(out ImprovePlayer improvePlayer))
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

            if (itemToPickUp.stack < cloneItem.stack)
            {
                SoundEngine.PlaySound(SoundID.Grab);
            }

            Main.item[worldItemArrayIndex] = itemToPickUp;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldItemArrayIndex);
            }
            return itemToPickUp;
        };
    }

    private static void PickupPopupText(Item cloneItem, Item self)
    {
        if (self.stack < cloneItem.stack)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, cloneItem, cloneItem.stack - self.stack);
        }
    }

    /// <summary>
    /// 允许你在玩家捡到一项物品时做一些特殊的事情 <br/>
    /// 返回 <see langword="false"/> 以阻止该项目进入玩家的 inventoy，默认情况下返回 true。
    /// </summary>
    /// <param name="self"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override bool OnPickup(Item self, Player player)
    {
        if (!player.TryGetModPlayer(out ImprovePlayer improvePlayer))
        {
            return true;
        }

        // 旗帜盒
        if (improvePlayer.BannerChest is not null && improvePlayer.BannerChest.AutoStorage && ItemToBanner(self) != -1)
        {
            Item oldItem = self.Clone();
            improvePlayer.BannerChest.PutInPackage(ref self);
            PickupPopupText(oldItem, self);
        }

        if (self.IsAir) return false;

        // 药水袋
        if (improvePlayer.PotionBag is not null && improvePlayer.PotionBag.AutoStorage && self.buffType > 0 && self.consumable)
        {
            Item item = self.Clone();
            improvePlayer.PotionBag.PutInPackage(ref self);
            PickupPopupText(item, self);
        }

        if (self.IsAir) return false;

        // 大背包
        if (!self.IsAir && Config.SuperVault &&
            player.GetModPlayer<UIPlayerSetting>().SuperVault_SmartGrab &&
            player.TryGetModPlayer(out DataPlayer dataPlayer) &&
            self.InArray(dataPlayer.SuperVault))
        {
            Item oldItem = self.Clone();
            self.StackToArray(dataPlayer.SuperVault);
            PickupPopupText(oldItem, self);
        }

        if (self.IsAir) return false;

        // 虚空保险库 之 智能收纳
        if (Config.SmartVoidVault && !self.IsAir && !self.IsACoin)
        {
            // 虚空保险库
            if (player.IsVoidVaultEnabled && self.InArray(player.bank4.item))
            {
                Item oldItem = self.Clone();
                self.StackToArray(player.bank4.item);
                PickupPopupText(oldItem, self);
            }

            if (self.IsAir) return false;

            // 猪猪 保险箱 ...
            if (Config.SuperVoidVault && !self.IsAir)
            {
                if (improvePlayer.HasPiggyBank && self.InArray(player.bank.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank.item);
                    PickupPopupText(oldItem, self);
                }

                if (self.IsAir) return false;

                if (improvePlayer.HasSafe && !self.IsAir && self.InArray(player.bank2.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank2.item);
                    PickupPopupText(oldItem, self);
                }

                if (self.IsAir) return false;

                if (improvePlayer.HasDefendersForge && !self.IsAir && self.InArray(player.bank3.item))
                {
                    Item oldItem = self.Clone();
                    self.StackToArray(player.bank3.item);
                    PickupPopupText(oldItem, self);
                }

                if (self.IsAir) return false;
            }
        }

        return true;
    }

    // 抓取距离
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        grabRange += Config.GrabDistance * 16;
    }

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
