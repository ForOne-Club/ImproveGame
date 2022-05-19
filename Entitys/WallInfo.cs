using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.Entitys
{
    public class WallInfo
    {
        public int type;
        public int x;
        public int y;

        public WallInfo(int type, int x, int y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }
    }
}
