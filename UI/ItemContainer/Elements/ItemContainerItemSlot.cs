using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ItemContainer.Elements;

public class ItemContainerItemSlot(List<Item> items, int index) : View
{
    private Item Item
    {
        get => _index >= 0 && _index < _items.Count ? _items[_index] : _airItem;
        set => _items[_index] = value;
    }
    private readonly List<Item> _items = items;
    private readonly int _index = index;

    private readonly Item _airItem = new Item();

    private int _rightMouseTimer = 0;
    private int _superFastStackTimer = 0;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        bool mouseItemIsAir = Main.mouseItem.IsAir;
        base.LeftMouseDown(evt);
        if (Item.IsAir || !mouseItemIsAir)
            return;

        SetCursor();
        MouseDown_ItemSlot();
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        base.RightMouseDown(evt);
        if (Item.IsAir)
            return;
        _rightMouseTimer = 0;
        _superFastStackTimer = 0;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // 右键长按物品持续拿出
        if (!Main.mouseRight || !IsMouseHovering || Item.IsAir)
        {
            return;
        }

        switch (_rightMouseTimer)
        {
            case >= 60:
            case >= 30 when _rightMouseTimer % 3 == 0:
            case >= 15 when _rightMouseTimer % 6 == 0:
            case 1:
                int stack = _superFastStackTimer + 1;
                stack = Math.Min(stack, Item.stack);
                TakeSlotItemToMouseItem(stack);
                break;
        }

        if (_rightMouseTimer >= 60 && _rightMouseTimer % 2 == 0 && _superFastStackTimer < 40)
            _superFastStackTimer++;

        _rightMouseTimer++;
    }

    private static Texture2D Banner => ModAsset.Banner.Value;
    private static Texture2D Potion => ModAsset.Potion.Value;

    public override void DrawSelf(SpriteBatch sb)
    {
        base.DrawSelf(sb);
        CalculatedStyle dimensions = GetDimensions();

        if (Item.IsAir)
        {
            switch (ItemContainerGUI.StorageType)
            {
                case StorageType.Banners:
                    BigBagItemSlot.DrawItemIcon(sb, Banner, Color.White * 0.5f, dimensions);
                    break;
                case StorageType.Potions:
                    BigBagItemSlot.DrawItemIcon(sb, Potion, Color.White * 0.5f, dimensions);
                    break;
            }
        }
        else
        {
            BigBagItemSlot.DrawItemIcon(sb, Item, Color.White, dimensions);
        }

        if (!Item.IsAir && Item.stack > 1)
        {
            Vector2 textSize = GetFontSize(Item.stack) * 0.75f;
            Vector2 textPos = dimensions.Position() + new Vector2(52 * 0.18f, (52 - textSize.Y) * 0.9f);
            TrUtils.DrawBorderString(sb, Item.stack.ToString(), textPos, Color.White, 0.75f);
        }

        if (!IsMouseHovering)
        {
            return;
        }

        Main.hoverItemName = Item.Name;
        Main.HoverItem = Item.Clone();
        SetCursor();

        if (Main.mouseItem.IsAir) return;

        switch (ItemContainerGUI.StorageType)
        {
            // 旗帜收纳箱, 药水袋子.
            case StorageType.Banners when ItemToBanner(Main.mouseItem) == -1:
            case StorageType.Potions when Main.mouseItem.buffType <= 0 || !Main.mouseItem.consumable:
                UICommon.TooltipMouseText(GetText("PackageGUI.Incompatible"));
                break;
        }
    }

    /// <summary>
    /// 拿物品槽内物品到鼠标物品上
    /// </summary>
    protected void TakeSlotItemToMouseItem(int stack)
    {
        if ((!Main.mouseItem.IsTheSameAs(Item) || !ItemLoader.CanStack(Main.mouseItem, Item)) &&
             Main.mouseItem.type is not ItemID.None || Main.mouseItem.stack >= Main.mouseItem.maxStack &&
                                                         Main.mouseItem.type is not ItemID.None)
        {
            return;
        }

        if (Main.mouseItem.type is ItemID.None)
        {
            Main.mouseItem = ItemLoader.TransferWithLimit(Item, stack);
            ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(Item, ItemSlot.Context.InventoryItem, ItemSlot.Context.MouseItem));
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        else
        {
            ItemLoader.StackItems(Main.mouseItem, Item, out _, numToTransfer: stack);
            if (Item.stack <= 0)
                Item.SetDefaults();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private static void SetCursor()
    {
        // 快速取出
        if (ItemSlot.ShiftInUse)
        {
            Main.cursorOverride = CursorOverrideID.ChestToInventory; // 快捷放回物品栏图标
        }

        // 放入聊天框
        if (Main.keyState.IsKeyDown(Main.FavoriteKey) && Main.drawingPlayerChat)
        {
            Main.cursorOverride = CursorOverrideID.Magnifiers;
        }
    }

    /// <summary>
    /// 左键点击物品
    /// </summary>
    private void MouseDown_ItemSlot()
    {
        if (Main.LocalPlayer.ItemAnimationActive)
            return;

        switch (Main.cursorOverride)
        {
            // 放大镜图标 - 输入到聊天框
            case CursorOverrideID.Magnifiers:
                {
                    if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(Item),
                            Vector2.One))
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    return;
                }
            // 放回物背包图标
            case CursorOverrideID.ChestToInventory:
                Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item,
                    GetItemSettings.InventoryEntityToPlayerInventorySettings);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
        }

        if (!Main.mouseItem.IsAir)
        {
            return;
        }

        Main.mouseItem = Item;
        Item = new Item();
        SoundEngine.PlaySound(SoundID.Grab);
    }
}