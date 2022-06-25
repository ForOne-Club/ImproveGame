using ImproveGame.Common.GlobalItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ImproveGame.Common.GlobalBuffs
{
    public class HideGlobalBuff : GlobalBuff
    {
        /// <summary>
        /// 本帧被隐藏的Buff数量，便于后面的Buff重设绘制坐标
        /// </summary>
        public static int HidedBuffCountThisFrame;

        public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
            if (ApplyBuffItem.BuffTypesShouldHide.Contains(type)) {
                // 不管开不开都有的功能 —— 隐藏剩余时间文本
                drawParams.TextPosition = new Vector2(-100f, -100f);
                if (MyUtils.Config.HideNoConsumeBuffs) {
                    // 干掉鼠标和文本
                    drawParams.MouseRectangle = Rectangle.Empty;
                    HidedBuffCountThisFrame++;
                    return false;
                }
            }
            if (HidedBuffCountThisFrame > 0) {
                int i = buffIndex - HidedBuffCountThisFrame;
                int x = 32 + i * 38;
                int y = 76;
                if (i >= 11) { // 一行
                    x = 32 + Math.Abs(i % 11) * 38;
                    y += 50 * (i / 11);
                }
                // 重设各种参数
                drawParams.Position = new Vector2(x, y);
                int width = drawParams.Texture.Width;
                int height = drawParams.Texture.Height;
                drawParams.TextPosition = new Vector2(x, y + height);
                drawParams.MouseRectangle = new Rectangle(x, y, width, height);
            }
            return base.PreDraw(spriteBatch, type, buffIndex, ref drawParams);
        }
    }
}
