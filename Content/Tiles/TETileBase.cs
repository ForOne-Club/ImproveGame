using Terraria.DataStructures;
using Terraria.ObjectData;

namespace ImproveGame.Content.Tiles
{
    public abstract class TETileBase : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            ModifyObjectData();
            ModTileEntity tileEntity = GetTileEntity();
            if (tileEntity is not null)
                TileObjectData.newTile.HookPostPlaceMyPlayer =
                    new PlacementHook(tileEntity.Hook_AfterPlacement, -1, 0, false);
            else
                TileObjectData.newTile.HookPostPlaceMyPlayer =
                    new PlacementHook(Hook_AfterPlacement_NoEntity, -1, 0, false);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            int alternateStyle = 0;
            if (ModifyObjectDataAlternate(ref alternateStyle))
                TileObjectData.addAlternate(alternateStyle);

            TileObjectData.addTile(Type);

            // We don't need to call SetDefault() on CreateMapEntryName()'s return value if we have .hjson files.
            AddMapEntry(new Color(153, 107, 61), CreateMapEntryName());

            DustType = 7;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
        }

        public virtual int Hook_AfterPlacement_NoEntity(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 2, 2);
            }

            return 0;
        }

        public virtual bool OnRightClick(int i, int j) => false;

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!OnRightClick(i, j))
                return base.RightClick(i, j);

            Main.mouseRightRelease = false;

            if (player.sign > -1)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = string.Empty;
            }

            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1,
                    NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }

            if (player.talkNPC > -1)
            {
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = string.Empty;
            }

            if (player.chest != -1)
            {
                player.chest = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }

            return base.RightClick(i, j);
        }

        public virtual void ModifyObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.LavaDeath = false;
        }

        public virtual bool ModifyObjectDataAlternate(ref int alternateStyle) => false;

        public abstract ModTileEntity GetTileEntity();

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            GetTileEntity()?.Kill(i, j);
        }
    }
}