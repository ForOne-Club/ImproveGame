using ImproveGame.Common.Players;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ImproveGame.Common.GlobalItems
{
    public class TagItem : GlobalItem
    {

        private static readonly Dictionary<string, List<int>> CombinedBuffs = new() {
            { "Battle", new() { ItemID.PeaceCandle, ItemID.WaterCandle, ItemID.BattlePotion, ItemID.Sunflower, ItemID.CalmingPotion} }
        };

        /// <summary>
        /// 为药水物品简介添加Tag标题文本
        /// </summary>
        public static void ModifyBuffTooltips(Mod Mod, int itemType, int buffType, List<TooltipLine> tooltips) {
            bool buffEnabled = InfBuffPlayer.Get(Main.LocalPlayer).CheckInfBuffEnable(buffType);

            if (itemType == ItemID.GardenGnome)
            {
                tooltips.Add(new(Mod, "TagDetailed.GardenGnome", MyUtils.GetText("Tips.TagDetailed.GardenGnome"))
                {
                    OverrideColor = Color.SkyBlue
                });
                AddShiftForMoreTooltip(tooltips);
                return;
            }

            // 是否被增益追踪器禁用
            if (!buffEnabled)
            {
                tooltips.Add(new(Mod, "TagDetailed.Disabled", MyUtils.GetText("Tips.TagDetailed.Disabled"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }
            else
            {
                tooltips.Add(new(Mod, "TagDetailed.Enabled", MyUtils.GetText("Tips.TagDetailed.Enabled"))
                {
                    OverrideColor = Color.SkyBlue
                });
                // 图标是否被隐藏
                if (MyUtils.Config.HideNoConsumeBuffs)
                {
                    tooltips.Add(new(Mod, "TagDetailed.Hided", MyUtils.GetText("Tips.TagDetailed.Hided"))
                    {
                        OverrideColor = Color.LightGreen
                    });
                }
                else
                {
                    tooltips.Add(new(Mod, "TagDetailed.NonHided", MyUtils.GetText("Tips.TagDetailed.NonHided"))
                    {
                        OverrideColor = Color.LightGreen
                    });
                }
            }

            // 可能的组合增益
            foreach (var dict in CombinedBuffs)
            {
                if (dict.Value.Contains(itemType))
                {
                    tooltips.Add(new(Mod, $"CombinedBuff.{dict.Key}", MyUtils.GetText("Tips.TagDetailed.CombinedBuff"))
                    {
                        OverrideColor = Color.Cyan
                    });
                }
            }

            AddShiftForMoreTooltip(tooltips);
        }
        
        public static void AddShiftForMoreTooltip(List<TooltipLine> tooltips)
        {
            // Shift显示更多信息
            if (!ItemSlot.ShiftInUse)
                tooltips.Add(new(ImproveGame.Instance, "Tag.ShiftEnable", MyUtils.GetText("Tips.Tag.ShiftEnable"))
                {
                    OverrideColor = Color.Orange
                });
        }

        /// <summary>
        /// 根据物品原Tooltip中的Tag标题文本生成详细文本列表
        /// </summary>=
        public static List<TooltipLine> GenerateDetailedTags(Mod Mod, ReadOnlyCollection<TooltipLine> tooltips, object arg = null) {
            List<TooltipLine> list = new();
            foreach (TooltipLine line in tooltips) {
                if (line.Mod == Mod.Name) {
                    // 一般的Tag提示
                    if (line.Name.StartsWith("TagDetailed")) {
                        RegularTagSetup();
                        // Tag详细信息
                        string[] parts = line.Name.Split('.');
                        AddToList($"Tips.{parts[0]}Tip.{parts[1]}");
                    }
                    // 增益组合提示
                    if (line.Name.StartsWith("CombinedBuff")) {
                        RegularTagSetup();
                        // Tag详细信息
                        AddToList($"Tips.TagDetailedTip.{line.Name}");
                    }

                    // 设置基本的Tag信息
                    void RegularTagSetup() {
                        // 一行空位隔开
                        if (list.Count > 0) {
                            list.Add(new(Mod, "Empty", ""));
                        }
                        // Tag名称
                        list.Add(new(Mod, line.Name, line.Text) {
                            OverrideColor = line.OverrideColor
                        });
                    }

                    // 添加到list里面
                    void AddToList(string key) {
                        if (arg is not null) {
                            list.Add(new(Mod, key, MyUtils.GetTextWith(key, arg)));
                        }
                        else {
                            list.Add(new(Mod, key, MyUtils.GetText(key)));
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 绘制生成好的Tag详细文本
        /// </summary>
        /// <param name="tooltips">原物品Tooltip</param>
        /// <param name="tagTooltips">Tag Tooltip文本</param>
        /// <param name="x">原Tooltip文本绘制起始点 X 坐标</param>
        /// <param name="y">原Tooltip文本绘制起始点 Y 坐标</param>
        public static void DrawTagTooltips(ReadOnlyCollection<TooltipLine> tooltips, List<TooltipLine> tagTooltips, int x, int y) {
            var font = FontAssets.MouseText.Value;
            int widthOffset = 14;
            int heightOffset = 9;

            float length = 0f;
            foreach (TooltipLine line in tooltips) {
                length = Math.Max(length, ChatManager.GetStringSize(font, line.Text, Vector2.One).X);
            }
            x += (int)length + widthOffset * 2 + 6;

            float lengthY = 0f;
            length = 0f;
            foreach (TooltipLine line in tagTooltips) {
                if (line.Name == "Empty") {
                    lengthY += 24;
                    continue;
                }
                var stringSize = ChatManager.GetStringSize(font, line.Text, Vector2.One);
                length = Math.Max(length, stringSize.X + 8);
                lengthY += stringSize.Y;
            }
            if (Main.SettingsEnabled_OpaqueBoxBehindTooltips) {
                Terraria.Utils.DrawInvBG(Main.spriteBatch, new Rectangle(x - widthOffset, y - heightOffset, (int)length + widthOffset * 2, (int)lengthY + heightOffset + heightOffset / 2), new Color(23, 25, 81, 255) * 0.925f);
            }

            foreach (TooltipLine line in tagTooltips) {
                if (line.Name == "Empty") {
                    y += 24;
                    continue;
                }
                int drawX = x;
                if (!line.Name.StartsWith("TagDetailed") && !line.Name.StartsWith("CombinedBuff")) {
                    drawX += 8;
                }
                Color color = line.OverrideColor ?? new(0.7f, 0.7f, 0.7f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, line.Text, new(drawX, y), color, 0f, Vector2.Zero, Vector2.One, spread: 1.6f);
                y += (int)ChatManager.GetStringSize(font, line.Text, Vector2.One).Y;
            }
        }
    }
}
