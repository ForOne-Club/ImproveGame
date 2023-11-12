using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 米西米西
/// </summary>
public class Miximixi
{
    public Vector2? UIPosition;

    /// <summary>
    /// 收藏
    /// </summary>
    public bool Favorite { get; set; }

    public readonly List<Balabala> Balabalas = new();

    public Miximixi(Texture2D texture, string nameKey)
    {
        Texture = texture;
        NameKey = nameKey;
    }

    public Texture2D Texture { get; set; }

    public string NameKey { get; set; }

    public string Name => GetText(NameKey);

    /// <summary>
    /// 创建卡片
    /// </summary>
    public PropertyCard CreateCard(out TimerView titleView, out SUIImage image, out SUITitle title)
    {
        // 卡片主体
        PropertyCard card = new(this, UIColor.PanelBorder * 0f, UIColor.PanelBg * 0f, 10f, 2);
        card.UseImmediateMode = true;

        // 标题容器
        titleView = new()
        {
            Rounded = new Vector4(6f),
            BgColor = UIColor.TitleBg2,
            DragIgnore = true,
        };
        titleView.Width.Percent = 1f;
        titleView.Height.Pixels = 30f;
        titleView.SetPadding(8f, 0f);
        titleView.Join(card);

        // 标题图标
        image = new SUIImage(Texture)
        {
            ImagePercent = new Vector2(0.5f),
            ImageOrigin = new Vector2(0.5f),
            ImageScale = 0.8f,
            DragIgnore = true
        };
        image.Width.Pixels = 20f;
        image.Height.Percent = 1f;
        image.Join(titleView);

        // 标题文字
        title = new(Name, 0.36f)
        {
            HAlign = 0.5f
        };
        title.Height.Percent = 1f;
        title.Width.Pixels = title.TextSize.X;
        title.Join(titleView);

        return card;
    }

    public void AppendPropertys(PropertyCard card)
    {
        for (int i = 0; i < Balabalas.Count; i++)
        {
            Balabala balabala = Balabalas[i];
            PropertyBar propertyBar = new(balabala.Name, balabala.Value, balabala);

            if (balabala.Favorite)
            {
                card.Append(propertyBar);
            }
        }
    }

    public void AppendPropertysForControl(View view, PropertyCard card)
    {
        for (int i = 0; i < Balabalas.Count; i++)
        {
            Balabala bala = Balabalas[i];
            PropertyBar bar = new(bala.Name, bala.Value, bala);

            bar.OnUpdate += (_) =>
            {
                bar.BorderColor = bala.Favorite ? Color.Transparent : (Color.Red * 0.85f);
                bar.Border = bala.Favorite ? 0 : 2;
            };

            bar.OnLeftMouseDown += (_, _) =>
            {
                bala.Favorite = !bala.Favorite;

                foreach (var item in view.Children)
                {
                    if (item is PropertyCard innerCard && innerCard.Miximixi == card.Miximixi)
                    {
                        var first = innerCard.Children.First();
                        innerCard.RemoveAllChildren();
                        innerCard.Append(first);
                        innerCard.Miximixi.AppendPropertys(innerCard);
                    }
                }
            };

            card.Console = true;
            card.Append(bar);
        }
    }
}
