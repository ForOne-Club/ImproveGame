namespace ImproveGame.Interface.BaseViews
{
    public class ListView : View
    {
        public ListView()
        {
            DragIgnore = true;
        }
        
        public override bool ContainsPoint(Vector2 point) => Parent.IsMouseHovering;

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