using ImproveGame.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.Common.GlobalTiles
{
    public class ContainerProtectGlobalTile : GlobalTile
    {

        public override void Load()
        {
            On.Terraria.WorldGen.CanPoundTile += ModifyCanPoundTile;
            On.Terraria.WorldGen.IsAContainer += ModifyIsAContainer;
            On.Terraria.Projectile.CanExplodeTile += ModifyCanExplode;
        }

        private bool ModifyCanExplode(On.Terraria.Projectile.orig_CanExplodeTile orig, Projectile self, int x, int y)
        {
            int tileType = Main.tile[x, y].TileType;
            var modTile = ModContent.GetModTile(tileType);
            if (modTile is not null && modTile is ITileContainer)
            {
                TileID.Sets.BasicChest[tileType] = true;
                bool flag = orig.Invoke(self, x, y);
                TileID.Sets.BasicChest[tileType] = false;
                return flag;
            }
            return orig.Invoke(self, x, y);
        }

        private bool ModifyCanPoundTile(On.Terraria.WorldGen.orig_CanPoundTile orig, int x, int y)
        {
            int tileType = Main.tile[x, y - 1].TileType;
            var modTile = ModContent.GetModTile(tileType);
            if (modTile is not null && modTile is ITileContainer)
            {
                TileID.Sets.BasicChest[tileType] = true;
                bool flag = orig.Invoke(x, y);
                TileID.Sets.BasicChest[tileType] = false;
                return flag;
            }
            return orig.Invoke(x, y);
        }

        private bool ModifyIsAContainer(On.Terraria.WorldGen.orig_IsAContainer orig, Tile t)
        {
            var modTile = ModContent.GetModTile(t.TileType);
            if (modTile is not null && modTile is ITileContainer)
                return true;
            return orig.Invoke(t);
        }
    }
}
