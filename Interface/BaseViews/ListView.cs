namespace ImproveGame.Interface.BaseViews
{
    public class ListView : TimerView
    {
        public ListView()
        {
            DragIgnore = true;
        }

        public override bool ContainsPoint(Vector2 point)
        {
            Vector2 parentPos = Parent.GetDimensions().Position();
            Vector2 parentSize = Parent.GetDimensions().Size();

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Vector2 iPos = new Vector2(
                Math.Clamp(pos.X, parentPos.X, parentPos.X + parentSize.X),
                Math.Clamp(pos.Y, parentPos.Y, parentPos.Y + parentSize.Y));
            Vector2 iSize = new Vector2(
                Math.Clamp(size.X, 0, parentPos.X + parentSize.X - iPos.X),
                Math.Clamp(size.Y, 0, parentPos.Y + parentSize.Y - iPos.Y));

            return point.X > iPos.X && point.Y > iPos.Y && point.X < (iPos.X + iSize.X) && point.Y < (iPos.Y + iSize.Y);
        }

        public override void DrawChildren(SpriteBatch spriteBatch)
        {
            Vector2 pos = Parent.GetDimensions().Position();
            Vector2 size = Parent.GetDimensions().Size();
            foreach (var uie in from uie in Elements let dimensions2 = uie.GetDimensions() let position2 =
                         dimensions2.Position() let size2 = dimensions2.Size()
                                where Collision.CheckAABBvAABBCollision(pos, size, position2, size2)
                                select uie)
            {
                uie.Draw(spriteBatch);
            }
        }
    }
}