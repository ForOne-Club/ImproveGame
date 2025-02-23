using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.UI;
using static Terraria.NPC.NPCNameFakeLanguageCategoryPassthrough;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionVector2 : OptionObject
    {
        protected override void OnWrapOption(ModernConfigOption option)
        {
            //option.Width.Set(-120,1);
            base.OnWrapOption(option);
        }
        protected override void OnBind()
        {
            base.OnBind();
            OptionView.Width.Set(-120, 1);
            sliderX = OptionView.ListView.Elements[0] as OptionSlider;
            sliderY = OptionView.ListView.Elements[1] as OptionSlider;
            var panel = new View()
            {
                Left = new(-100, 1),
                Top = new(40, 0),
                Width = new(100, 0),
                Height = new(100, 0),
                RelativeMode = RelativeMode.None,
                Rounded = new(4.0f),
                BgColor = Color.Black * .3f

            };
            panel.JoinParent(this);

            double m = IncrementAttribute != null ? (sliderX.Max - sliderX.Min) / sliderX.Increment.Value : 7;
            int n = (int)m;

            for (int u = 0; u <= n; u++)
            {
                var icBarX = new View()
                {
                    Left = new(2 + 92 * u / (float)m, 0),
                    Width = new(4, 0),
                    Height = new(100, 0),
                    RelativeMode = RelativeMode.None,
                    Rounded = new(1.0f),
                    BgColor = Color.Black * .2f,
                    IgnoresMouseInteraction = true
                };
                var icBarY = new View()
                {
                    Top = new(2 + 92 * u / (float)m, 0),
                    Width = new(100, 0),
                    Height = new(4, 0),
                    RelativeMode = RelativeMode.None,
                    Rounded = new(1.0f),
                    BgColor = Color.Black * .2f,
                    IgnoresMouseInteraction = true
                };
                icBarX.JoinParent(panel);
                icBarY.JoinParent(panel);
            }


            var barX = new View()
            {
                Width = new(4, 0),
                Height = new(100, 0),
                RelativeMode = RelativeMode.None,
                Rounded = new(1.0f),
                BgColor = UIStyle.Cross * .5f
            };
            var barY = new View()
            {
                Width = new(100, 0),
                Height = new(4, 0),
                RelativeMode = RelativeMode.None,
                Rounded = new(1.0f),
                BgColor = UIStyle.Cross * .5f
            };
            barX.JoinParent(panel);
            barY.JoinParent(panel);
            barX.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem) return;
                dragging = true;
                lockY = true;
            };
            barX.OnLeftMouseUp += (evt, elem) =>
            {
                dragging = false;
                lockY = false;
            };
            barY.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem) return;
                dragging = true;
                lockX = true;
            };
            barY.OnLeftMouseUp += (evt, elem) =>
            {
                dragging = false;
                lockX = false;
            };
            var point = new View()
            {
                Left = new(50, 0),
                Top = new(50, 0),
                Width = new(8, 0),
                Height = new(8, 0),
                RelativeMode = RelativeMode.None,
                Rounded = new(2.0f),
                BgColor = UIStyle.Cross
            };
            point.JoinParent(panel);
            panel.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem) return;
                dragging = true;
                //startPos = Main.MouseScreen;
                var v = Main.MouseScreen - panel.GetDimensions().Position();
                if (MathF.Abs(v.X - controlBarX.Left.Pixels - controlBarX.Width.Pixels * .5f) < 16)
                    lockY = true;
                else if (MathF.Abs(v.Y - controlBarY.Top.Pixels - controlBarY.Height.Pixels * .5f) < 16)
                    lockX = true;
                else
                {
                    point.Left.Set(v.X, 0);
                    point.Top.Set(v.Y, 0);
                }
            };
            point.OnLeftMouseDown += (evt, elem) =>
            {
                if (evt.Target != elem) return;
                dragging = true;
                //startPos = Main.MouseScreen;
            };
            point.OnUpdate += (elem) =>
            {
                var d = point.GetDimensions();
                if (!dragging) return;
                Vector2 v = Main.MouseScreen - panel.GetDimensions().Position();
                v = Vector2.Clamp(v, default, new Vector2(100));
                point.SetPosPixels(v);
                if (!lockX)
                    sliderX.LerpValue = v.X * .01f;
                if (!lockY)
                    sliderY.LerpValue = v.Y * .01f;
            };
            point.OnLeftMouseUp += (evt, elem) =>
            {
                dragging = false;
                if (!lockX)
                    sliderX.OutSideEditEnd();
                if (!lockY)
                    sliderY.OutSideEditEnd();
            };
            panel.OnLeftMouseUp += (evt, elem) =>
            {
                dragging = false;
                lockX = lockY = false;
            };
            controlPoint = point;
            controlBarX = barX;
            controlBarY = barY;

            Recalculate();
        }
        public override void Update(GameTime gameTime)
        {
            controlPoint.SetPosPixels(new Vector2(sliderX.LerpValue, sliderY.LerpValue) * 92);//
            controlBarX.Left.Set(MathHelper.Lerp(2, 94, sliderX.LerpValue), 0);
            controlBarY.Top.Set(MathHelper.Lerp(2, 94, sliderY.LerpValue), 0);
            controlPoint.Recalculate();
            controlBarX.Recalculate();
            controlBarY.Recalculate();
            base.Update(gameTime);
        }
        protected override void OnSetDefault(object value)
        {
            Vector2 vec = (Vector2)value;
            sliderX.LerpValue = Utils.GetLerpValue((float)sliderX.Min, (float)sliderX.Max, vec.X);
            sliderY.LerpValue = Utils.GetLerpValue((float)sliderY.Min, (float)sliderY.Max, vec.Y);
            base.OnSetDefault(value);
        }
        protected override void CheckAttributes()
        {
            base.CheckAttributes();
            ShowStringValueInLabel = false;
        }
        OptionSlider sliderX;
        OptionSlider sliderY;
        View controlPoint;
        View controlBarX;
        View controlBarY;
        bool dragging;
        bool lockX;
        bool lockY;
        //Vector2 startPos;
    }
}
