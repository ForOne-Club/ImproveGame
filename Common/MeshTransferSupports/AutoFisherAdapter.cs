using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetAutofisher;
using ImproveGame.UIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    public class AutoFisherAdapter
    {
        public const int BassSpeedingCap = 100;

        public void TakeItem(int x, int y, object slot, int amount)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return;
            }
            int slotIndex = (int)slot;
            Item[] fish = teFisher.fish;
            Item item = fish[slotIndex];
            bool isAir = false;
            if (item.stack <= amount)
            {
                item.stack = 0;
                item.TurnToAir();
                isAir = true;
            }
            else
            {
                item.stack -= amount;
            }
            SyncItem(isAir ? 0 : -amount, teFisher, (byte)slotIndex);
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                yield break;
            }
            // maybe use reflection?
            Item[] fishes = teFisher.fish;
            int bassExists = 0;

            for (int i = 0; i < fishes.Length; i++)
            {
                Item fish = fishes[i];
                if (fish.stack > 0)
                {
                    if (fish.type == ItemID.Bass && bassExists < BassSpeedingCap)
                    {
                        bassExists += fish.stack;
                        continue;
                    }
                    yield return new Tuple<Item, object>(fish, i);
                }
            }
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return false;
            }

            if (item.bait == 0)
            {
                return false;
            }

            Item baitSlot = teFisher.bait;
            // 用于判断是否注入了鱼饵(HasValue),且是否需要完整同步
            bool? fromAir = null;
            if (!teFisher.HasBait)
            {
                baitSlot.SetDefaults(item.type);
                item.stack--;
                fromAir = true;
            }
            else if (baitSlot.IsTheSameAs(item))
            {
                if (baitSlot.stack < baitSlot.maxStack)
                {
                    baitSlot.stack++;
                    item.stack--;
                    fromAir = true;
                }
            }
            if(fromAir is bool air)
            {
                SyncItem(air ? 0 : 1, teFisher, ItemSyncPacket.Bait);
            }

            return fromAir.HasValue;
        }

        public static bool TryGetTEAutofisher(int x, int y, out TEAutofisher teFisher)
        {

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
            {
                teFisher = null;
                return false;
            }

            int originX = x - (tile.TileFrameX % 36) / 18;
            int originY = y - (tile.TileFrameY % 36) / 18;

            Tile origin = Main.tile[originX, originY];
            if (origin == null || !origin.HasTile)
            {
                teFisher = null;
                return false;
            }

            Point16 position = new Point16(originX, originY);
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity te) || te is not TEAutofisher fisher)
            {
                teFisher = null;
                return false;
            }

            teFisher = fisher;
            return true;
        }

        public static void SyncItem(int amount, TEAutofisher fisher , byte slotIndex)
        {
            if (Main.netMode is NetmodeID.Server)
            {
                // 这包是给开着钓鱼机的玩家用的，只给开着的发包就行了
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    var client = Main.player[p];
                    if (client.active && !client.DeadOrGhost &&
                        client.GetModPlayer<AutofishPlayer>().IsAutofisherOpened)
                    {
                        if (amount != 0)
                        {
                            ItemsStackChangePacket.Get(fisher.ID, (byte)slotIndex, amount).Send(p);
                        }
                        else
                        {
                            ItemSyncPacket.Get(fisher.ID, slotIndex).Send(p);
                        }
                    }
                }


                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    UISystem.Instance.AutofisherGUI?.RefreshItems();
                }
            }
        }
    }
}
