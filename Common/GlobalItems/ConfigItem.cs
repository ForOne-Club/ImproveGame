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
                //TODO: Draw cross
            }
        }
    }
}
