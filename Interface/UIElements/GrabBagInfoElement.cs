using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace ImproveGame.Interface.UIElements
{
    /// <summary>
    /// 一个单元的掉率信息
    /// </summary>
    public class GrabBagInfoPanel : UIPanel
    {
        public DropRateInfo DropRateInfo { get; private set; }

        public GrabBagInfoPanel(DropRateInfo info, float width)
        {
            this.SetSize(new Vector2(width, 40f));
            BorderColor = new(89, 116, 213);
            DropRateInfo = info;

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

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var item = new Item(DropRateInfo.itemId);
            if (IsMouseHovering)
            {
                BorderColor = Main.OurFavoriteColor;
                Main.HoverItem = item;
                Main.hoverItemName = item.Name;
                SetBestiaryNotesOnItemCache(DropRateInfo, item);
            }
            else
            {
                BorderColor = new(89, 116, 213);
            }

            base.DrawSelf(spriteBatch);

            var dimensions = GetDimensions();
            dimensions.Width = 40f;
            ItemSlot_BigBag.DrawItem(sb: spriteBatch,
                                   Item: item,
                                   lightColor: Color.White,
                                   dimensions: dimensions,
                                   ItemSize: 28);


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
}
