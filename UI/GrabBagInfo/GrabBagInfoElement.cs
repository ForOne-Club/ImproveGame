using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameContent.ItemDropRules;

namespace ImproveGame.UI.GrabBagInfo;

/// <summary>
/// 一个单元的掉率信息
/// </summary>
public class GrabBagInfoPanel : TimerView
{
    public DropRateInfo DropRateInfo { get; private set; }

    public GrabBagInfoPanel(DropRateInfo info)
    {
        this.SetSize(-4, 40f, 0.5f, 0f);
        DropRateInfo = info;

        RelativeMode = RelativeMode.Horizontal;
        Spacing = new Vector2(8f);
        Border = 2;
        Rounded = new Vector4(12f);
        PreventOverflow = true;
        DragIgnore = true;

        GetDropInfo(DropRateInfo, out string stackRange, out string droprate);
        if (!string.IsNullOrEmpty(stackRange))
            droprate = stackRange + " " + droprate;

        UITextPanel<string> dropInfo = new(droprate, 1f)
        {
            IgnoresMouseInteraction = true,
            DrawPanel = false,
            HAlign = 1f,
            VAlign = 0.5f
        };

        Append(dropInfo);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        var item = new Item(DropRateInfo.itemId);
        if (IsMouseHovering)
        {
            Main.HoverItem = item;
            Main.hoverItemName = item.Name;
            SetBestiaryNotesOnItemCache(DropRateInfo, item);
        }

        BorderColor = HoverTimer.Lerp(UIStyle.PanelBg, UIStyle.ItemSlotBorderFav);
        BgColor = UIStyle.PanelBg;

        base.DrawSelf(spriteBatch);

        var dimensions = GetDimensions();
        dimensions.Width = 40f;
        BigBagItemSlot.DrawItemIcon(sb: spriteBatch,
            item: item,
            lightColor: Color.White,
            dimensions: dimensions,
            maxSize: 28);
    }

    public static void GetDropInfo(DropRateInfo dropRateInfo, out string stackRange, out string droprate)
    {
        if (dropRateInfo.stackMin != dropRateInfo.stackMax)
            stackRange = $" ({dropRateInfo.stackMin}-{dropRateInfo.stackMax})";
        else if (dropRateInfo.stackMin == 1)
            stackRange = "";
        else
            stackRange = " (" + dropRateInfo.stackMin + ")";

        string originalFormat = "P";
        if ((double)dropRateInfo.dropRate < 0.001)
            originalFormat = "P4";

        droprate = dropRateInfo.dropRate is not 1f ? Utils.PrettifyPercentDisplay(dropRateInfo.dropRate, originalFormat) : "100%";
    }

    public static void SetBestiaryNotesOnItemCache(DropRateInfo info, Item displayItem)
    {
        List<string> list = new();
        if (info.conditions == null)
            return;

        foreach (IItemDropRuleCondition condition in info.conditions)
        {
            if (condition != null)
            {
                string conditionDescription = condition.GetConditionDescription();
                if (!string.IsNullOrWhiteSpace(conditionDescription))
                    list.Add(conditionDescription);
            }
        }

        displayItem.BestiaryNotes = string.Join("\n", list);
    }
}