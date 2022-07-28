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
        public static Point TransformToUIPosition(ref int x, ref int y)
        {
            // 获取相对屏幕中心的向量(一定要在调节xy前获取)
            float oppositeX = (x - Main.screenWidth / 2) / Main.UIScale;
            float oppositeY = (y - Main.screenHeight / 2) / Main.UIScale;
            x = (int)(x / Main.UIScale) + (int)(oppositeX * (Main.GameZoomTarget - 1f));
            y = (int)(y / Main.UIScale) + (int)(oppositeY * (Main.GameZoomTarget - 1f));
            return new(x, y);
        }

        /// <summary>
        /// 缩放修复（这公式自己测的，没有游戏依据）
        /// 将屏幕坐标转换为UI坐标
        /// </summary>
        public static Vector2 TransformToUIPosition(Vector2 vector)
        {
            // 获取相对屏幕中心的向量(一定要在调节xy前获取)
            float oppositeX = (vector.X - Main.screenWidth / 2) / Main.UIScale;
            float oppositeY = (vector.Y - Main.screenHeight / 2) / Main.UIScale;
            vector.X = (int)(vector.X / Main.UIScale) + (int)(oppositeX * (Main.GameZoomTarget - 1f));
            vector.Y = (int)(vector.Y / Main.UIScale) + (int)(oppositeY * (Main.GameZoomTarget - 1f));
            return new(vector.X, vector.Y);
        }

        public static Vector2 MouseScreenUI => TransformToUIPosition(Main.MouseScreen);
        public static Vector2 MouseScreenOffset(float offset) => Main.MouseScreen + new Vector2(offset);
        public static Vector2 MouseScreenOffset(float x, float y) => Main.MouseScreen + new Vector2(x, y);
        public static Vector2 MouseScreenOffset(Vector2 offset) => Main.MouseScreen + offset;
    }
}
