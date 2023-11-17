using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 属性类别
/// </summary>
public class BasePropertyCategory
{
    /// <summary>
    /// 判断是否来自其他 Mod
    /// </summary>
    public bool IsOtherMod { get; set; }

    public Vector2? UIPosition;

    /// <summary>
    /// 收藏
    /// </summary>
    public bool Favorite { get; set; }

    public List<BaseProperty> BaseProperties { get; private set; } = new();

    public BasePropertyCategory(Texture2D texture, string nameKey, bool isOtherMod = false)
    {
        Texture = texture;
        NameKey = nameKey;
        IsOtherMod = isOtherMod;
    }

    public Texture2D Texture { get; set; }

    public string NameKey { get; set; }

    public string Name => IsOtherMod ? Language.GetTextValue(NameKey) : GetText(NameKey);

    /// <summary>
    /// 创建卡片
    /// </summary>
    public PropertyCard CreateCard(out SUIImage image, out SUITitle title)
    {
        // 卡片主体
        PropertyCard card = new(this, UIColor.PanelBorder * 0f, UIColor.PanelBg * 0f, 10f, 2);
        card.OverflowHidden = true;

        // 标题图标
        image = new SUIImage(Texture)
        {
            ImagePercent = new Vector2(0.5f),
            ImageOrigin = new Vector2(0.5f),
            ImageScale = 0.8f,
            DragIgnore = true,
            TickSound = false
        };
        image.Width.Pixels = 20f;
        image.Height.Percent = 1f;
        image.Join(card.TitleView);

        // 标题文字
        title = new(Name, 0.36f)
        {
            HAlign = 0.5f
        };
        title.Height.Percent = 1f;
        title.Width.Pixels = title.TextSize.X;
        title.Join(card.TitleView);

        return card;
    }

    public void AppendProperties(PropertyCard card)
    {
        for (int i = 0; i < BaseProperties.Count; i++)
        {
            BaseProperty property = BaseProperties[i];
            PropertyBar propertyBar = new(property.Name, property.Value, property);

            if (property.Favorite)
            {
                card.Append(propertyBar);
            }
        }
    }

    public void AppendPropertiesForControl(View view, PropertyCard card)
    {
        for (int i = 0; i < BaseProperties.Count; i++)
        {
            BaseProperty property = BaseProperties[i];
            PropertyBar bar = new(property.Name, property.Value, property);

            bar.OnUpdate += (_) =>
            {
                Color red = new Color(1f, 0f, 0f);
                bar.BorderColor = (property.Parent.Favorite && property.Favorite) ? UIColor.ItemSlotBorderFav : Color.Transparent;
                bar.Border = (property.Parent.Favorite && property.Favorite) ? 2 : 0;
            };

            bar.OnLeftMouseDown += (_, _) =>
            {
                property.Favorite = !property.Favorite;

                foreach (var item in view.Children)
                {
                    if (item is PropertyCard innerCard && innerCard.PropertyCategory == card.PropertyCategory)
                    {
                        innerCard.RemoveAllChildren();
                        innerCard.Append(innerCard.TitleView);
                        innerCard.PropertyCategory.AppendProperties(innerCard);
                    }
                }
            };

            card.Console = true;
            card.Append(bar);
        }
    }
}
