namespace ImproveGame.Interface
{
    public interface IUseEventTrigger
    {
        public bool ToPrimary(UIElement target);
        public bool CanOccupyCursor(UIElement target);
    }
}