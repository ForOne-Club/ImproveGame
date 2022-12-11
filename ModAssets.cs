using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using System.Reflection;

namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> Fork;
        internal static Asset<Effect> Line;
        internal static Asset<Effect> Round;
        internal static Asset<Effect> RoundRectShadow;
        internal static Asset<Effect> RoundRect;
        internal static Asset<Effect> RoundRect2;
        internal static Asset<Effect> RoundRectNoBorder;
        internal static Asset<Effect> RoundRectNoBorder2;
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Effect> Transform;
        internal static Asset<Effect> BezierCurves;
        internal static Asset<Texture2D> Perlin;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            BezierCurves = GetEffect("BezierCurves");
            Fork = GetEffect("Fork");
            Line = GetEffect("Line");
            Round = GetEffect("Round");
            RoundRectShadow = GetEffect("RoundRectShadow");
            RoundRect = GetEffect("RoundRect");
            RoundRect2 = GetEffect("RoundRect2");
            RoundRectNoBorder = GetEffect("RoundRectNoBorder");
            RoundRectNoBorder2 = GetEffect("RoundRectNoBorder2");
            BoxShader = GetEffect("Box");
            ItemEffect = GetEffect("item");
            LiquidSurface = GetEffect("LiquidSurface");
            Transform = GetEffect("Transform");
            Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");

            // 修改原版 UI 的绘制
            // On.Terraria.GameContent.UI.Elements.UIPanel.DrawSelf += UIPanel_DrawSelf;
            // On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawBar += (_, _, _, _, _, _) => { };
            // On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawSelf += UIScrollbar_DrawSelf;
        }

        /*private void UIScrollbar_DrawSelf(On.Terraria.GameContent.UI.Elements.UIScrollbar.orig_DrawSelf orig, UIScrollbar self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            orig.Invoke(self, spriteBatch);
            PixelShader.DrawRoundRect(pos, size, MathF.Min(size.X, size.Y) / 2, UIColor.Default.CheckBackground, 3, UIColor.Default.PanelBorder);
            Type type = self.GetType();
            MethodInfo methodInfo = type.GetMethod("GetHandleRectangle", BindingFlags.NonPublic | BindingFlags.Instance);
            Rectangle rectangle = (Rectangle)methodInfo.Invoke(self, null);
            pos = rectangle.TopLeft();
            pos += new Vector2(5f, 5);
            size = rectangle.Size() - new Vector2(10);
            PixelShader.DrawRoundRect(pos, size, MathF.Min(size.X, size.Y) / 2, new(220, 220, 220), 3, new(220, 220, 220));
        }

        private void UIPanel_DrawSelf(On.Terraria.GameContent.UI.Elements.UIPanel.orig_DrawSelf orig, UIPanel self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 12, self.BackgroundColor, 3, self.BorderColor);
        }*/

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            Fork = null;
            Line = null;
            Round = null;
            RoundRectShadow = null;
            RoundRect = null;
            RoundRect2 = null;
            RoundRectNoBorder = null;
            RoundRectNoBorder2 = null;
            BoxShader = null;
            ItemEffect = null;
            LiquidSurface = null;
            Transform = null;
            Perlin = null;
        }
    }
}
