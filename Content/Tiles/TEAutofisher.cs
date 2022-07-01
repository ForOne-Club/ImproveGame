using ImproveGame.Common.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Tiles
{
    public class TEAutofisher : ModTileEntity
    {
        internal Item fishingPole = new();
        internal Item bait = new();
        internal Item[] fish = new Item[15];

        public override bool IsTileValidForEntity(int x, int y) {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Autofisher>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                //Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 2;
                int height = 2;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                //Sync the placement of the tile entity with other clients
                //The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
            }

            //ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            //Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            Point16 tileOrigin = new(1, 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }

        public override void OnNetPlace() {
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void LoadData(TagCompound tag) {
            fishingPole = tag.Get<Item>("fishingPole");
            bait = tag.Get<Item>("bait");
            for (int i = 0; i < 15; i++)
                if (tag.TryGet<Item>($"fish{i}", out var savedFish))
                    fish[i] = savedFish;
        }

        public override void SaveData(TagCompound tag) {
            tag["fishingPole"] = fishingPole;
            tag["bait"] = bait;
            for (int i = 0; i < 15; i++)
                tag[$"fish{i}"] = fish[i];
        }

        public override void NetSend(BinaryWriter writer) {
            ItemIO.Send(fishingPole, writer, true, false);
            ItemIO.Send(bait, writer, true, false);
            for (int i = 0; i < 15; i++)
                ItemIO.Send(fish[i], writer, true, false);
        }

        public override void NetReceive(BinaryReader reader) {
            ItemIO.Receive(fishingPole, reader, true, false);
            ItemIO.Receive(bait, reader, true, false);
            for (int i = 0; i < 15; i++)
                ItemIO.Receive(fish[i], reader, true, false);
        }
    }
}
