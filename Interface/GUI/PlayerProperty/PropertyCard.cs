using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

public class PropertyCard : SUIPanel
{
    public bool Console { get; set; }
    public Miximixi Miximixi { get; set; }
    public TimerView TitleView;

    public PropertyCard(Miximixi miximixi, Color borderColor, Color backgroundColor, float rounded = 12, float border = 2, bool draggable = false) : base(borderColor, backgroundColor, rounded, border, draggable)
    {
        Miximixi = miximixi;
        SetPadding(5);

        TitleView = new TimerView()
        {
            Rounded = new Vector4(6f),
            BgColor = UIColor.TitleBg2,
            DragIgnore = true,
        };
        TitleView.SetPadding(8f, 0f);
        TitleView.Width.Percent = 1f;
        TitleView.Height.Pixels = 30f;
        TitleView.Join(this);
    }

    public PropertyCard(Miximixi miximixi, Color backgroundColor, Color borderColor, Vector4 rounded, float border, bool draggable = false) : base(backgroundColor, borderColor, rounded, border, draggable)
    {
        Miximixi = miximixi;
        SetPadding(5);

        TitleView = new TimerView()
        {
            Rounded = new Vector4(6f),
            BgColor = UIColor.TitleBg2,
            DragIgnore = true,
        };
        TitleView.SetPadding(8f, 0f);
        TitleView.Width.Percent = 1f;
        TitleView.Height.Pixels = 160f;
        TitleView.Join(this);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Console)
        {
            List<UIElement> list = Children.ToList();
            HashSet<Balabala> appeared = new HashSet<Balabala>();

            bool recalculate = false;

            // 删除多余的
            for (int i = 0; i < list.Count; i++)
            {
                var uie = list[i];

                if (uie is PropertyBar bar)
                {
                    if (appeared.Contains(bar.Balabala))
                    {
                        recalculate = true;
                        uie.Remove();
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

            if (recalculate || HasChildCountChanges)
            {
                SetInnerPixels(160, (30 + 2) * Children.Count() - 2);
                Recalculate();
            }
        }

        base.Update(gameTime);
    }
}
