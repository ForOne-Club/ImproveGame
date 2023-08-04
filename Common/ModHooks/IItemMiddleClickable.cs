using Microsoft.Xna.Framework.Input;

namespace ImproveGame.Common.ModHooks;

/// <summary>
/// 必须与IItemOverrideHover一起使用
/// </summary>
public interface IItemMiddleClickable
{
    private class MiddleClickReset : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            _isHoveringInInventory = false;
        }
    }
    
    private static bool _oldMiddlePressed; // 上一帧的状态
    private static bool _isHoveringInInventory; // 物品的context是否是InventoryItem，给ModifyTooltips用的

    void OnMiddleClicked(Item item);

    bool MiddleClickable(Item item) => true;

    void ManageHoverTooltips(Item item, List<TooltipLine> tooltips) {}

    /// <summary>
    /// 在IItemOverrideHover的OverrideHover中调用该方法
    /// </summary>
    public void HandleHover(Item[] inventory, int context, int slot)
    {
        var item = inventory[slot];

        if (context is not ItemSlot.Context.InventoryItem || !MiddleClickable(item))
            return;

        _isHoveringInInventory = true;

        MouseState mouseState = Mouse.GetState();
        if (_oldMiddlePressed)
        {
            _oldMiddlePressed = mouseState.MiddleButton is ButtonState.Pressed;
        }

        if (mouseState.MiddleButton is ButtonState.Pressed && !_oldMiddlePressed)
        {
            _oldMiddlePressed = true;
            // 防止玩家把物品买出来，中键使用，然后原价退还。这里从shopSellbackHelper中移除这个物品
            Main.shopSellbackHelper.Remove(item);

            OnMiddleClicked(item);
        }
    }

    /// <summary>
    /// 在ModifyTooltips中调用该方法
    /// </summary>
    /// <param name="item"></param>
    /// <param name="tooltips"></param>
    public void HandleTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (_isHoveringInInventory && MiddleClickable(item))
        {
            ManageHoverTooltips(item, tooltips);
        }
    }
}