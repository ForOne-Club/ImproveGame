using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria;

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
            Rounded = new Vector4(8f),
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

    public void AppendPropertys(PropertyCard card, Func<Balabala, PropertyBar, bool> isConsole)
    {
        for (int i = 0; i < Balabalas.Count; i++)
        {
            Balabala balabala = Balabalas[i];
            PropertyBar propertyBar = new(balabala.Name, balabala.Value, balabala);

            if (isConsole?.Invoke(balabala, propertyBar) ?? false)
            {
                card.Console = true;
                card.Append(propertyBar);
            }
            else if (balabala.Favorite)
            {
                card.Append(propertyBar);
            }
        }
    }
}
