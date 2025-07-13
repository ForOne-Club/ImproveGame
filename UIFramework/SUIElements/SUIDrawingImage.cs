using ImproveGame.UIFramework.BaseViews;

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
