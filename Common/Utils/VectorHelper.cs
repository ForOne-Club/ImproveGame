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
            // 获取相对屏幕中心的向量(一定要在调节xy前获取)
            float oppositeX = (x - Main.screenWidth / 2) / Main.UIScale;
            float oppositeY = (y - Main.screenHeight / 2) / Main.UIScale;
            x = (int)(x / Main.UIScale) + (int)(oppositeX * (Main.GameZoomTarget - 1f));
            y = (int)(y / Main.UIScale) + (int)(oppositeY * (Main.GameZoomTarget - 1f));
            return new(x, y);
        }
    }
}
