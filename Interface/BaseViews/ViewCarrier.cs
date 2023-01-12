namespace ImproveGame.Interface.BaseViews
{
    public class ViewCarrier : View
    {
        public ViewCarrier()
        {
            Width.Percent = 1f;
            Height.Percent = 1f;
            base.Recalculate();
        }
    }
}