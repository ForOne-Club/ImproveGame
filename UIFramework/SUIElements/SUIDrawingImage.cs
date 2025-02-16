using ImproveGame.UIFramework.BaseViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.UIFramework.SUIElements
{
    public class SUIDrawingImage(Action<View> drawingFunction) : View
    {
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            drawingFunction?.Invoke(this);
        }
    }
}
