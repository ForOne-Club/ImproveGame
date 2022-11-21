using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using System.Reflection;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> Fork;
        internal static Asset<Effect> RoundRectangle;
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Fork = GetEffect(nameof(Fork));
                RoundRectangle = GetEffect(nameof(RoundRectangle));
                BoxShader = GetEffect("Box");
                ItemEffect = GetEffect("item");
                LiquidSurface = GetEffect("LiquidSurface");
                Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
            }
            // On.Terraria.GameContent.UI.Elements.UIPanel.DrawSelf += UIPanel_DrawSelf;
            // On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawBar += UIScrollbar_DrawBar;
            // On.Terraria.GameContent.UI.Elements.UIScrollbar.DrawSelf += UIScrollbar_DrawSelf;
        }

        private void UIScrollbar_DrawSelf(On.Terraria.GameContent.UI.Elements.UIScrollbar.orig_DrawSelf orig, UIScrollbar self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            orig.Invoke(self, spriteBatch);
            PixelShader.DrawBox(Main.UIScaleMatrix, pos, size, MathF.Min(size.X, size.Y) / 2, 3, UIColor.Default.PanelBorder, UIColor.Default.CheckBackground);
            Type type = self.GetType();
            MethodInfo methodInfo = type.GetMethod("GetHandleRectangle", BindingFlags.NonPublic | BindingFlags.Instance);
            Rectangle rectangle = (Rectangle)methodInfo.Invoke(self, null);
            pos = rectangle.TopLeft();
            pos += new Vector2(5f, 5);
            size = rectangle.Size() - new Vector2(10);
            PixelShader.DrawBox(Main.UIScaleMatrix, pos, size, MathF.Min(size.X, size.Y) / 2, 3, new(220, 220, 220), new(220, 220, 220));
        }

        private void UIScrollbar_DrawBar(On.Terraria.GameContent.UI.Elements.UIScrollbar.orig_DrawBar orig, UIScrollbar self, SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color) { return; }

        private void UIPanel_DrawSelf(On.Terraria.GameContent.UI.Elements.UIPanel.orig_DrawSelf orig, UIPanel self, SpriteBatch spriteBatch)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();
            PixelShader.DrawBox(Main.UIScaleMatrix, pos, size, 12, 3, self.BorderColor, self.BackgroundColor);
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                Fork = null;
                RoundRectangle = null;
                BoxShader = null;
                ItemEffect = null;
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
