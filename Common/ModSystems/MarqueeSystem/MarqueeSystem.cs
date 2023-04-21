using ImproveGame.Common.Animations;
using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public class MarqueeSystem : ModSystem
{
    public readonly MarqueeLayer MarqueeLayer;

    public MarqueeSystem()
    {
        MarqueeLayer = new MarqueeLayer(this);

        PositionChangeTimer = new AnimationTimer(3);
        PositionChangeTimer.Timer = PositionChangeTimer.TimerMax;

        SizeChangeTimer = new AnimationTimer(3);
        SizeChangeTimer.Timer = SizeChangeTimer.TimerMax;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layers => layers.Name == "Vanilla: Ruler");

        if (index++ > -1)
        {
            layers.Insert(index, MarqueeLayer);
        }
    }

    public Vector2 LastPosition;
    public Vector2 LastSize;

    public AnimationTimer PositionChangeTimer;
    public AnimationTimer SizeChangeTimer;

    public Vector2 AnimationBeginPosition;
    public Vector2 AnimationBeginSize;

    public void BaseDraw()
    {
        Player player = Main.LocalPlayer;
        Item item = player.HeldItem;

        if (item.ModItem is IMarqueeItem marqueeItem && marqueeItem.CanDraw())
        {
            RectangleF rectangle = marqueeItem.GetMarquee();
            Vector2 position = rectangle.Position;
            Vector2 size = rectangle.Size;

            Color borderColor = marqueeItem.GetBorderColor();
            Color backgroundColor = marqueeItem.GetBackgroundColor();

            bool sizeChange = LastSize != size;
            bool positionChange = LastPosition != position;

            if (sizeChange)
            {
                SizeChange(size);
            }

            if (positionChange)
            {
                PositionChange(position);
            }

            position = PositionChangeTimer.Lerp(AnimationBeginPosition, position);
            size = SizeChangeTimer.Lerp(AnimationBeginSize, size);

            bool drawVanilla = true;

            marqueeItem.PreDraw(ref drawVanilla, rectangle, backgroundColor, borderColor);

            if (drawVanilla)
            {
                SDFRectangle.HasBorder(
                    position - new Vector2(2) - Main.screenPosition,
                    size + new Vector2(4),
                    new Vector4(2),
                    backgroundColor, 2f, borderColor, false);

                marqueeItem.PostDraw(rectangle, backgroundColor, borderColor);
            }

            LastPosition = position;
            LastSize = size;
        }
        else
        {
            AnimationBeginSize = Vector2.Zero;
        }
    }

    public void SizeChange(Vector2 newSize)
    {
        AnimationBeginSize = SizeChangeTimer.Lerp(AnimationBeginSize, newSize);
        SizeChangeTimer.OpenAndReset();
    }

    public void PositionChange(Vector2 newPosition)
    {
        AnimationBeginPosition = PositionChangeTimer.Lerp(AnimationBeginPosition, newPosition);
        PositionChangeTimer.OpenAndReset();
    }
}

public class MarqueeLayer : GameInterfaceLayer
{
    public readonly MarqueeSystem MarqueeSystem;

    public MarqueeLayer(MarqueeSystem marqueeSystem) : base("ImproveGame: Marquee", InterfaceScaleType.Game)
    {
        MarqueeSystem = marqueeSystem;
    }

    public override bool DrawSelf()
    {
        MarqueeSystem.PositionChangeTimer.Update();
        MarqueeSystem.SizeChangeTimer.Update();
        MarqueeSystem.BaseDraw();
        return true;
    }
}
