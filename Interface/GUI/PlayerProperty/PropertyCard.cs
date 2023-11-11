using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

public class PropertyCard : SUIPanel
{
    public bool Console { get; set; }
    public Miximixi Miximixi { get; set; }

    public PropertyCard(Miximixi miximixi, Color borderColor, Color backgroundColor, float rounded = 12, float border = 2, bool draggable = false) : base(borderColor, backgroundColor, rounded, border, draggable)
    {
        Miximixi = miximixi;
        SetPadding(5);
    }

    public PropertyCard(Miximixi miximixi, Color backgroundColor, Color borderColor, Vector4 rounded, float border, bool draggable = false) : base(backgroundColor, borderColor, rounded, border, draggable)
    {
        Miximixi = miximixi;
        SetPadding(5);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Console)
        {
            List<UIElement> list = Children.ToList();
            HashSet<Balabala> appeared = new HashSet<Balabala>();

            bool recalculate = false;

            for (int i = 0; i < list.Count; i++)
            {
                var uie = list[i];

                if (uie is PropertyBar bar)
                {
                    if (appeared.Contains(bar.Balabala))
                    {
                        recalculate = true;
                        appeared.Remove(bar.Balabala);
                        break;
                    }

                    appeared.Add(bar.Balabala);

                    if (!bar.Balabala.Favorite)
                    {
                        recalculate = true;
                        uie.Remove();
                    }
                }
            }

            if (recalculate)
            {
                SetInnerPixels(160, (30 + 2) * Children.Count() - 2);
                Recalculate();
            }
        }

        base.Update(gameTime);
    }
}
