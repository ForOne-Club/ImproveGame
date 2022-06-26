using ImproveGame.Common.GlobalItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
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
                // 装备栏下方绘制
                if (Main.playerInventory) {
                    int mH = 0;
                    if (Main.mapEnabled && !Main.mapFullscreen && Main.mapStyle == 1) {
                        mH = 256;
                    }
                    if (mH + Main.instance.RecommendedEquipmentAreaPushUp > Main.screenHeight)
                        mH = Main.screenHeight - Main.instance.RecommendedEquipmentAreaPushUp;
                    int num23 = Main.screenWidth - 92;
                    int num24 = mH + 174;
                    num24 += 247;
                    num23 += 8;
                    int num29 = 3;
                    int num30 = 260;
                    if (Main.screenHeight > 630 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    if (Main.screenHeight > 680 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    if (Main.screenHeight > 730 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    int num31 = 46;

                    int num32 = i / num29;
                    int num33 = i % num29;
                    x = num23 + num32 * -num31;
                    y = num24 + num33 * num31;
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
