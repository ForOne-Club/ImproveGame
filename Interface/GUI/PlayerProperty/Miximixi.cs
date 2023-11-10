using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 米西米西
/// </summary>
public class Miximixi
{
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
    public SUIPanel CreateCard(out TimerView titleView, out SUIImage image, out SUITitle title)
    {
        // 卡片主体
        SUIPanel card = new(UIColor.PanelBorder * 0.85f, UIColor.PanelBg * 0.85f, 10f, 2);
        card.SetPadding(5);
        card.SetInnerPixels(160, (30 + 4) * (Balabalas.Count + 1) - 4);

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

    public void AppendPropertys(UIElement uie)
    {
        for (int i = 0; i < Balabalas.Count; i++)
        {
            Balabala balabala = Balabalas[i];
            var ppi = new PlayerPropertyCard(balabala.Name, balabala.Value);
            uie.Append(ppi);
        }
    }
}
