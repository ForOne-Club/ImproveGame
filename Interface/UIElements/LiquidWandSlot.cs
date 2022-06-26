using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    public class LiquidWandSlot : ModImageButton
    {
        public readonly byte LiquidID;
        private readonly float _hoverColor;
        private readonly float _normalColor;
        private float _liquidAmount;
        public float CurrentColor;
        public int HoverTimer;

        // 悬停到别的按钮时这个的百分比文本也要虚化
        public bool IsAltHovering;
        public float AltHoverTimer;

        public LiquidWandSlot(byte liquidID, float hoverColor, float normalColor) : base(TextureAssets.Liquid[0], Color.White, Color.White) {
            LiquidID = liquidID;
            _hoverColor = hoverColor;
            _normalColor = normalColor;
            SetHoverImage(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/LiquidSlot_Highlight"));
            SetBackgroundImage(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/LiquidSlot_Border"));
        }

        /// <summary>
        /// 用其他地方的的液体给当前槽存液体
        /// </summary>
        /// <param name="amount">存液体的数量，这个值会根据可存数量的多少减少</param>
        public void StoreLiquid(ref byte amount) {
        // 液体添加量
            float addAmount = MyUtils.LiquidAmountToFloat(amount);
            // 还可以存入的液体量
            float amountAvailable = 1f - _liquidAmount;
            // 取相对少的，毕竟如果我可以给你0.5%，你却只剩下0.25%，我只能填给你0.25%
            float amountAddition = Math.Min(addAmount, amountAvailable);

            // 应用上去
            amount -= (byte)MyUtils.LiquidAmountToInt(amountAddition);
            _liquidAmount += amountAddition;
        }

        /// <summary>
        /// 把当前槽的液体放出去，供应到其他地方
        /// </summary>
        /// <param name="amount">拿液体的数量，这个值会根据可存数量的多少增加</param>
        public void TakeLiquid(ref byte amount) {
            // 液体需求量，应是最大值即255减去当前值
            float needAmount = MyUtils.LiquidAmountToFloat(255 - amount);
            // 取相对少的，毕竟你要是太多我也拿不下，太少我也补不完
            float amountReduction = Math.Min(needAmount, _liquidAmount);

            amount += (byte)MyUtils.LiquidAmountToInt(amountReduction);
            _liquidAmount -= amountReduction;
        }

        public void SetLiquidAmount(float amount) => _liquidAmount = amount;

        public float GetLiquidAmount() => _liquidAmount;

        public override void Update(GameTime gameTime) {
            if (IsMouseHovering)
                HoverTimer++;
            else
                HoverTimer--;

            if (IsAltHovering)
                AltHoverTimer -= 0.1f;
            else
                AltHoverTimer += 0.1f;

            CurrentColor = MathHelper.SmoothStep(_normalColor, _hoverColor, HoverTimer / 15f);

            HoverTimer = Math.Clamp(HoverTimer, 0, 15);
            AltHoverTimer = Math.Clamp(AltHoverTimer, 0.5f, 1f);

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            CalculatedStyle dimensions = GetDimensions();

            // 考虑边框
            float scale = (dimensions.Width - 4f) / 16f;
            Vector2 drawPosition = dimensions.Position() + new Vector2(2f, 2f);

            // 原帧数，就是拼凑起来该有的
            var originalFrame = Texture.Frame(17, 1, (int)Main.wFrame, 0); // 根据风速的动态效果
            originalFrame.Width = 16;
            int waterStyle = Main.waterStyle; // 根据环境的水样式
            // 岩浆和蜂蜜是没有帧的
            if (LiquidID != Terraria.ID.LiquidID.Water) {
                originalFrame = new(0, 0, 16, 16);
                waterStyle = LiquidID == Terraria.ID.LiquidID.Lava ? 1 : 11;
            }

            // 水面高度效果，Y坐标添加是向上取整
            drawPosition.Y += (int)Math.Ceiling(originalFrame.Height * (1 - _liquidAmount) * scale);
            originalFrame.Height = (int)(originalFrame.Height * _liquidAmount);

            var mainColor = new Color(CurrentColor, CurrentColor, CurrentColor);
            if (DrawColor is not null) {
                mainColor = DrawColor.Invoke();
            }

            #region 液体绘制 代码好乱我下次来写就看不懂了
            byte fillLimit = (byte)(6f / scale);
            if (originalFrame.Height > fillLimit) {
                // 下部frame矩形，用于直接填充下部分纯色区域
                Rectangle downFrame = originalFrame;
                downFrame.Y += 6;
                downFrame.Height -= 6;
                // 一个我不明白的Bug，麻了，不管了
                if (downFrame.Height == 0)
                    downFrame.Height = 1;

                // 改变绘制位置
                Vector2 downPosition = drawPosition;
                downPosition.Y += fillLimit;

                // 计算需要填充的高度
                int heightShouldFill = (int)(dimensions.Y + dimensions.Height) - (int)downPosition.Y;
                // 这里的float强制转换不能移除，不然会有精度问题，尽管vs说可以
                Vector2 fillScale = new(scale, (float)heightShouldFill / (float)downFrame.Height);
                DrawLiquid(spriteBatch, waterStyle, downPosition, downFrame, mainColor, fillScale);
            }

            // 上部frame矩形，用于上方绘制，因为我想要较高清的样子而不是直接放大糊一团像素
            Rectangle upFrame = originalFrame;
            upFrame.Height = Math.Min(originalFrame.Height, 6);

            // 分为两块绘制
            float splitScale = scale * 0.5f;
            DrawLiquid(spriteBatch, waterStyle, drawPosition, upFrame, mainColor, new(splitScale));

            // 已被填充的区域，-8去掉两边边框
            int widthFilled = (int)(originalFrame.Width * splitScale);
            drawPosition.X += widthFilled - 1; // -1防止中间分开了，然而我并不知道是为什么
            DrawLiquid(spriteBatch, waterStyle, drawPosition, upFrame, mainColor, new(splitScale + 0.125f, splitScale)); // Xscale变大一点，防止右边留有空缺
            #endregion

            // 边框
            if (BackgroundTexture is not null) {
                spriteBatch.Draw(BackgroundTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // 选中时的高光
            if (WandSystem.LiquidMode == LiquidID && BorderTexture is not null) {
                spriteBatch.Draw(BorderTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // 绘制一个文本，液体剩余多少
            // 转换为百分数，保留后一位，来自: https://www.jianshu.com/p/3f88338bde60
            string text = $"{_liquidAmount:p1}";
            Vector2 origin = FontAssets.DeathText.Value.MeasureString(text) / 2f;

            Vector2 stringPosition = dimensions.Position();
            stringPosition.X += dimensions.Width / 2f + 2f;
            stringPosition.Y += dimensions.Height + 22f;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value,
                text, stringPosition, Color.White * AltHoverTimer, 0f, origin, new Vector2(0.4f), -1, 1);
        }

        public void DrawLiquid(SpriteBatch spriteBatch, int waterStyle, Vector2 position, Rectangle frame, Color mainColor, Vector2 scale) {
            // 原版绘制改的
            float alpha = 1f;
            if (LiquidID == Terraria.ID.LiquidID.Water) {
                for (int j = 0; j < 13; j++) {
                    if (Main.IsLiquidStyleWater(j) && Main.liquidAlpha[j] > 0f && j != waterStyle) {
                        spriteBatch.Draw(TextureAssets.Liquid[j].Value, position, frame, mainColor * Main.liquidAlpha[j], 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        alpha = Main.liquidAlpha[waterStyle];
                    }
                }
            }
            else {
                alpha = 1f;
            }
            spriteBatch.Draw(TextureAssets.Liquid[waterStyle].Value, position, frame, mainColor * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
