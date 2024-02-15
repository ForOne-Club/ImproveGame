using ImproveGame.UIFramework.SUIElements;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ItemContainer.Elements;

public class ItemContainerItemSlot : GenericItemSlot
{
    private int _rightMouseTimer = 0;
    private int _superFastStackTimer = 0;

    public ItemContainerItemSlot(IList<Item> items, int index) : base(items, index)
    {
        DisplayItemInfo = true;
        DisplayItemStack = true;

        ItemIconScale = 0.8f;
        SetSizePixels(52f * 0.8f, 52f * 0.8f);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        bool mouseItemIsAir = Main.mouseItem.IsAir;

        base.LeftMouseDown(evt);

        if (!Item.IsAir && mouseItemIsAir)
        {
            SetCursor();
            MouseDown_ItemSlot();
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (IsMouseHovering && Main.mouseRight)
        {
            if (!Item.IsAir)
            {
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
        }
        else
        {
            _rightMouseTimer = 0;
            _superFastStackTimer = 0;
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        /*CalculatedStyle dimensions = GetDimensions();

        if (!Item.IsAir)
        {
            BigBagItemSlot.DrawItemIcon(spriteBatch, Item, Color.White, dimensions);
        }

        if (!Item.IsAir && Item.stack > 1)
        {
            Vector2 textSize = GetFontSize(Item.stack) * 0.75f;
            Vector2 textPos = dimensions.Position() + new Vector2(52 * 0.18f, (52 - textSize.Y) * 0.9f);
            TrUtils.DrawBorderString(spriteBatch, Item.stack.ToString(), textPos, Color.White, 0.75f);
        }

        if (IsMouseHovering)
        {
            Main.hoverItemName = Item.Name + " 123";
            Main.HoverItem = Item.Clone();
            SetCursor();

            if (Main.mouseItem.IsAir) return;

            if (!ItemContainerGUI.Instace.Container.MeetEntryCriteria(Main.mouseItem))
            {
                UICommon.TooltipMouseText(GetText("PackageGUI.Incompatible"));
            }
        }*/
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