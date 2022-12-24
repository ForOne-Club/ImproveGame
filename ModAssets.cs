using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using System.Reflection;

namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> BetterFiltering;
        internal static Asset<Effect> Fork;
        internal static Asset<Effect> Line;
        internal static Asset<Effect> Round;
        internal static Asset<Effect> RoundRect;
        internal static Asset<Effect> RoundRect2;
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Effect> Transform;
        internal static Asset<Effect> BezierCurves;
        internal static Asset<Texture2D> Perlin;

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            BetterFiltering = null;
            Fork = null;
            Line = null;
            Round = null;
            RoundRect = null;
            RoundRect2 = null;
            BoxShader = null;
            ItemEffect = null;
            LiquidSurface = null;
            Transform = null;
            Perlin = null;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            BezierCurves = GetEffect("BezierCurves");
            BetterFiltering = GetEffect("BetterFiltering");
            Fork = GetEffect("Fork");
            Line = GetEffect("Line");
            Round = GetEffect("Round");
            RoundRect = GetEffect("RoundRect");
            RoundRect2 = GetEffect("RoundRect2");
            BoxShader = GetEffect("Box");
            ItemEffect = GetEffect("item");
            LiquidSurface = GetEffect("LiquidSurface");
            Transform = GetEffect("Transform");
            Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");

            // 修改原版 UI 的绘制
            /*On.Terraria.GameContent.UI.Elements.UIPanel.DrawSelf += UIPanel_DrawSelf;
            On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawBar += (_, _, _, _, _, _) => { };
            On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawSelf += UIScrollbar_DrawSelf;*/
            // 替换游戏内原来的 Utils.DrawInvBG
            // On.Terraria.Utils.DrawInvBG_SpriteBatch_int_int_int_int_Color += Utils_DrawInvBG_SpriteBatch_int_int_int_int_Color;
        }

        private void Utils_DrawInvBG_SpriteBatch_int_int_int_int_Color(On.Terraria.Utils.orig_DrawInvBG_SpriteBatch_int_int_int_int_Color orig, SpriteBatch sb, int x, int y, int w, int h, Color c)
        {
            PixelShader.DrawRoundRect(new Vector2(x, y), new Vector2(w, h), 10, c, 3, UIColor.PackgePanelBorder);
        }

        // 原版滚动条绘制，在 DrawBar 返回空阻止原版滚动条绘制。然后通过反射获取滚动条位置进行绘制。
        private void UIScrollbar_DrawSelf(On.Terraria.GameContent.UI.Elements.UIScrollbar.orig_DrawSelf orig, UIScrollbar self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            orig.Invoke(self, spriteBatch);
            PixelShader.DrawRoundRect(pos, size, MathF.Min(size.X, size.Y) / 2, UIColor.ScrollBarBackground, 3, UIColor.PanelBorder);
            MethodInfo methodInfo = self.GetType().GetMethod("GetHandleRectangle", BindingFlags.NonPublic | BindingFlags.Instance);
            Rectangle rectangle = (Rectangle)methodInfo.Invoke(self, null);
            pos = rectangle.TopLeft();
            pos += new Vector2(5f);
            size = rectangle.Size() - new Vector2(10f);
            PixelShader.DrawRoundRect(pos, size, MathF.Min(size.X, size.Y) / 2, new(220, 220, 220));
        }

        private void UIPanel_DrawSelf(On.Terraria.GameContent.UI.Elements.UIPanel.orig_DrawSelf orig, UIPanel self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 12, self.BackgroundColor, 3, self.BorderColor);
        }
    }
}
