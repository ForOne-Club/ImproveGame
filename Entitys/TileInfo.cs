using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.Entitys
{
    public class TileInfo
    {
        public int type;
        public int style;
        public int x;
        public int y;

        public TileInfo(int type, int style, int x, int y)
        {
            this.type = type;
            this.style = style;
            this.x = x;
            this.y = y;
        }
    }
}
