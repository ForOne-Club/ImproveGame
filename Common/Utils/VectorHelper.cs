using Microsoft.Xna.Framework;
using Terraria;

namespace ImproveGame
{
    partial class MyUtils
    {
        /// <summary>
        /// 缩放修复（这公式自己测的，没有游戏依据）
        /// 将屏幕坐标转换为UI坐标
        /// </summary>
        public static Point TransformToUIPosition(ref int x, ref int y) {
            x = (int)(x / Main.UIScale);
            x += (int)((x - Main.screenWidth * 0.5f) * Main.GameZoomTarget);
            y = (int)(y / Main.UIScale);
            y += (int)((y - Main.screenHeight * 0.5f) * Main.GameZoomTarget);
            return new(x, y);
        }
    }
}
