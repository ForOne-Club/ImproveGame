namespace ImproveGame.Common.GlobalItems
{
    public interface IConditionItem
    {
        public Condition UseCondition { get; }
    }
    public class ConfigItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.ModItem is IConditionItem config && !config.UseCondition.IsMet())
                return false;
            return base.CanUseItem(item, player);
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.ModItem is IConditionItem config && !config.UseCondition.IsMet())
            {
                // TODO: Draw cross
                // 如果有更好的图片替换这里就行了
                // 原版 UI 绘制的地方尽量不使用 SDF，除非这个地方没有使用合批（基本没这个情况）
                // SDF 通常用于一个单独的空间或者 UI 的背景绘制
                var texture2d = ModAsset.Images_Close.Value;
                spriteBatch.Draw(texture2d, position, null, Color.White * 0.75f, 0f, new Vector2(texture2d.Width, texture2d.Height) / 2f, 1f, 0, 0f);
            }
        }
    }
}
