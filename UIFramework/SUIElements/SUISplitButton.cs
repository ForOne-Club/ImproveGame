using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.UI;

namespace ImproveGame.UIFramework.SUIElements
{
    public class SUISplitButton : View
    {
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            //base.DrawSelf(spriteBatch);
            var dimension = GetDimensions();
            var pos = dimension.Position();
            var size = dimension.Size();

            float h = (pos + size * Vector2.UnitY * .25f).Y;
            var flip = Matrix.CreateTranslation(0, -h, 0) * Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(0, h, 0);

            SDFGraphics.HasBorderTriangleIsosceles(pos + size * Vector2.UnitY * .05f + size * Vector2.UnitX * .5f, new Vector2(.5f, 0), size * new Vector2(.75f, .4f), buttonColor, 1, IsUP ? UIStyle.SliderRoundHover : UIStyle.SliderRound, flip * GetMatrix(true));


            SDFGraphics.HasBorderTriangleIsosceles(pos + size * Vector2.UnitY * .55f + size * Vector2.UnitX * .5f, new Vector2(.5f, 0), size * new Vector2(.75f, .4f), buttonColor, 1, IsDown ? UIStyle.SliderRoundHover : UIStyle.SliderRound, GetMatrix(true));
        }
        public Color buttonColor;
        public Color buttonBorderColor;
        // Cy修改：添加了IgnoresMouseInteraction判断，在这个属性为true时，不会响应鼠标悬停事件（不会高光边框）
        public bool IsUP => !IgnoresMouseInteraction && IsMouseHovering && Main.mouseY < GetDimensions().ToRectangle().Center.Y;
        public bool IsDown => !IgnoresMouseInteraction && IsMouseHovering && Main.mouseY >= GetDimensions().ToRectangle().Center.Y;
    }
}
