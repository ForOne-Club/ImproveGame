using ImproveGame.Common.Conditions;
using ImproveGame.Common.GlobalItems;

namespace ImproveGame.Content.Items;

public class ShimmerBucket : ModItem, IConditionItem
{
    public Condition UseCondition => ConfigCondition.AvailableShimmerBucketC;
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.WaterBucket);
        base.SetDefaults();
    }
    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (!Main.GamepadDisableCursorItemIcon)
        {
            player.cursorItemIconEnabled = true;
            Main.ItemIconCacheUpdate(Item.type);
        }

        if (!player.ItemTimeIsZero || player.itemAnimation <= 0 || !player.controlUseItem)
            return;

        if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid == 0 || Main.tile[Player.tileTargetX, Player.tileTargetY].liquidType() == 3)
        {
            SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
            Main.tile[Player.tileTargetX, Player.tileTargetY].liquidType(3);
            Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = byte.MaxValue;
            WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);

            Item.stack--;
            player.PutItemInInventoryFromItemUsage(205, player.selectedItem);

            player.ApplyItemTime(Item);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
        }
        base.UseStyle(player, heldItemFrame);
    }
}
public class ShimmerBucketGlobalItem : GlobalItem
{
    public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
    {
        if (!AvailableConfig.AvailableShimmerBucket) return;
        if (item.type != ItemID.EmptyBucket) return;
        if (!player.ItemTimeIsZero || player.itemAnimation <= 0 || !player.controlUseItem)
            return;

        var tileTargetX = Player.tileTargetX;
        var tileTargetY = Player.tileTargetY;
        var sItem = item;

        int num = Main.tile[tileTargetX, tileTargetY].liquidType();
        int num2 = 0;
        for (int i = tileTargetX - 1; i <= tileTargetX + 1; i++)
        {
            for (int j = tileTargetY - 1; j <= tileTargetY + 1; j++)
            {
                if (Main.tile[i, j].liquidType() == num)
                    num2 += Main.tile[i, j].liquid;
            }
        }

        if (Main.tile[tileTargetX, tileTargetY].liquid <= 0 || num2 <= 100)
            return;

        int liquidType = Main.tile[tileTargetX, tileTargetY].liquidType();
        if (Main.tile[tileTargetX, tileTargetY].shimmer())
        {
            sItem.stack--;
            player.PutItemInInventoryFromItemUsage(ModContent.ItemType<ShimmerBucket>(), player.selectedItem);
        }

        SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
        player.ApplyItemTime(sItem);
        int num3 = Main.tile[tileTargetX, tileTargetY].liquid;
        Main.tile[tileTargetX, tileTargetY].liquid = 0;
        Main.tile[tileTargetX, tileTargetY].lava(lava: false);
        Main.tile[tileTargetX, tileTargetY].honey(honey: false);
        WorldGen.SquareTileFrame(tileTargetX, tileTargetY, resetFrame: false);
        if (Main.netMode == 1)
            NetMessage.sendWater(tileTargetX, tileTargetY);
        else
            Liquid.AddWater(tileTargetX, tileTargetY);

        if (num3 >= 255)
            return;

        for (int k = tileTargetX - 1; k <= tileTargetX + 1; k++)
        {
            for (int l = tileTargetY - 1; l <= tileTargetY + 1; l++)
            {
                if ((k != tileTargetX || l != tileTargetY) && Main.tile[k, l].liquid > 0 && Main.tile[k, l].liquidType() == num)
                {
                    int num4 = Main.tile[k, l].liquid;
                    if (num4 + num3 > 255)
                        num4 = 255 - num3;

                    num3 += num4;
                    Main.tile[k, l].liquid -= (byte)num4;
                    Main.tile[k, l].liquidType(liquidType);
                    if (Main.tile[k, l].liquid == 0)
                    {
                        Main.tile[k, l].lava(lava: false);
                        Main.tile[k, l].honey(honey: false);
                    }

                    WorldGen.SquareTileFrame(k, l, resetFrame: false);
                    if (Main.netMode == 1)
                        NetMessage.sendWater(k, l);
                    else
                        Liquid.AddWater(k, l);
                }
            }
        }

        base.UseStyle(item, player, heldItemFrame);
    }
}