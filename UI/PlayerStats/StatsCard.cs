using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.PlayerStats;

public class StatsCard : SUIPanel
{
    public bool Console { get; set; }
    public BaseStatsCategory StatsCategory { get; set; }
    public TimerView TitleView;

    public StatsCard(BaseStatsCategory proCat, Color borderColor, Color backgroundColor, float rounded = 12, float border = 2, bool draggable = false) : base(borderColor, backgroundColor, rounded, border, draggable)
    {
        StatsCategory = proCat;
        SetPadding(5);

        TitleView = new TimerView()
        {
            Rounded = new Vector4(6f),
            BgColor = UIStyle.StatCategoryBg,
            DragIgnore = true,
        };
        TitleView.SetPadding(8f, 0f);
        TitleView.Width.Percent = 1f;
        TitleView.Height.Pixels = 30f;
        TitleView.JoinParent(this);
    }

    public StatsCard(BaseStatsCategory proCat, Color backgroundColor, Color borderColor, Vector4 rounded, float border, bool draggable = false) : base(backgroundColor, borderColor, rounded, border, draggable)
    {
        StatsCategory = proCat;
        SetPadding(5);

        TitleView = new TimerView()
        {
            Rounded = new Vector4(6f),
            BgColor = UIStyle.StatCategoryBg,
            DragIgnore = true,
        };
        TitleView.SetPadding(8f, 0f);
        TitleView.Width.Percent = 1f;
        TitleView.Height.Pixels = 160f;
        TitleView.JoinParent(this);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Console)
        {
            StatsCategory.UIPosition = PositionPixels;

            List<UIElement> list = Children.ToList();
            HashSet<BaseStat> appeared = new HashSet<BaseStat>();

            bool recalculate = false;

            // 删除多余的
            for (int i = 0; i < list.Count; i++)
            {
                var uie = list[i];

                if (uie is StatBar bar)
                {
                    if (appeared.Contains(bar.BaseStat))
                    {
                        recalculate = true;
                        uie.Remove();
                        break;
                    }

                    appeared.Add(bar.BaseStat);

                    if (!bar.BaseStat.Favorite)
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
