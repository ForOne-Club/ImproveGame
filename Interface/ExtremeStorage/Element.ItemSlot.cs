using ImproveGame.Common.Packets.NetStorager;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.ExtremeStorage
{
    /// <summary>
    /// 继承 BigBagItemSlot，但是有修改，以适合作为箱子的物品槽
    /// </summary>
    public class ItemSlot : BigBagItemSlot
    {
        private bool ChestBeingUsed => StorageGrids.ChestsThatBeingUsedCache is not null &&
                                       StorageGrids.ChestsThatBeingUsedCache.Contains(_chestIndex) &&
                                       Main.LocalPlayer.chest != _chestIndex;

        private readonly int _chestIndex;

        public ItemSlot(Item[] items, int index, int chestIndex) : base(items, index)
        {
            _chestIndex = chestIndex;
            FavoriteAllowed = false;
            Spacing = new Vector2(4f);
        }

        // 覆写父类的方法，以添加多人相关操作
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Interactable)
                return;

            CallBaseLeftMouseDown(evt);

            SetCursorOverride();
            switch (Main.netMode)
            {
                case NetmodeID.MultiplayerClient when Main.cursorOverride is CursorOverrideID.Magnifiers:
                case NetmodeID.SinglePlayer:
                    MouseClickSlot();
                    ExtremeStorageGUI.RefreshCachedAllItems();
                    Recipe.FindRecipes();
                    break;
                case NetmodeID.MultiplayerClient:
                    switch (Main.cursorOverride)
                    {
                        case CursorOverrideID.ChestToInventory:
                            // 左键全部拿取
                            SoundEngine.PlaySound(SoundID.Grab);
                            RemoveItemOperationPacket.Send(_chestIndex, Index,
                                RemoveItemOperationPacket.RemovedItemDestination.Inventory);
                            break;
                        case CursorOverrideID.TrashCan:
                            SoundEngine.PlaySound(SoundID.Grab);
                            RemoveItemOperationPacket.Send(_chestIndex, Index,
                                RemoveItemOperationPacket.RemovedItemDestination.Trash);
                            break;
                        default:
                            Item chestItem = Main.chest[_chestIndex].item[Index];
                            if (Main.mouseItem.IsAir && chestItem.IsAir)
                            {
                                break;
                            }
                            else if (Main.mouseItem.IsAir)
                            {
                                // 拿走
                                RemoveItemOperationPacket.Send(_chestIndex, Index,
                                    RemoveItemOperationPacket.RemovedItemDestination.Mouse);
                            }
                            else if (chestItem.IsAir)
                            {
                                // 放入
                                AddItemOperationPacket.Send(_chestIndex, Index);
                            }
                            else if (Main.mouseItem.type == chestItem.type && chestItem.stack < chestItem.maxStack)
                            {
                                // 放入一部分
                                AddItemOperationPacket.Send(_chestIndex, Index);
                            }
                            else
                            {
                                // 交换
                                SwapItemOperationPacket.Send(_chestIndex, Index);
                            }

                            SoundEngine.PlaySound(SoundID.Grab);

                            break;
                    }

                    break;
            }
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.RightMouseUp(evt);

            if (Main.netMode is not NetmodeID.SinglePlayer) return;
            ExtremeStorageGUI.RefreshCachedAllItems();
            Recipe.FindRecipes();
        }

        // 覆写父类的方法，让宝藏袋右键不开启而是拿取
        public override void RightMouseDown(UIMouseEvent evt)
        {
            CallBaseRightMouseDown(evt);

            if (!Interactable)
                return;

            if (Item.IsAir)
                return;

            RightMouseDownTimer = 0;
        }

        // 覆写父类的方法，让宝藏袋右键不开启而是拿取，以及添加多人相关操作
        public override void Update(GameTime gameTime)
        {
            // base.Update(gameTime);
            // 跳过父类的 Update 方法，直接调用 UIElement 的
            base.CallBaseUpdate(gameTime);

            if (ChestBeingUsed)
                return;

            // 以防万一的设置，虽然可能不需要
            Item.favorited = false;

            // 右键长按物品持续拿出
            if (!Main.mouseRight || !IsMouseHovering || Item.IsAir || !Interactable)
                return;

            switch (RightMouseDownTimer)
            {
                case >= 60:
                case >= 30 when RightMouseDownTimer % 3 == 0:
                case >= 15 when RightMouseDownTimer % 6 == 0:
                case 0:
                    switch (Main.netMode)
                    {
                        case NetmodeID.SinglePlayer:
                            TakeSlotItemToMouseItem();
                            // 移动到RightMouseUp中，防止卡顿
                            // ExtremeStorageGUI.RefreshCachedAllItems();
                            // Recipe.FindRecipes();
                            break;
                        // 本地先保证可以拿出物品，然后再发送给服务器
                        case NetmodeID.MultiplayerClient when Main.mouseItem.IsAir ||
                                                              (Main.mouseItem.type == Item.type &&
                                                               Main.mouseItem.stack < Main.mouseItem.maxStack):
                            RemoveItemOperationPacket.Send(_chestIndex, Index,
                                RemoveItemOperationPacket.RemovedItemDestination.Mouse, 1);
                            SoundEngine.PlaySound(SoundID.MenuTick);
                            break;
                    }

                    break;
            }

            RightMouseDownTimer++;
        }

        public override void ModifyDrawColor()
        {
            if (!Interactable || Item.IsAir || !ExtremeStorageGUI.ChestSlotsGlowHue.ContainsKey(_chestIndex)) return;
            
            var hueArray = ExtremeStorageGUI.ChestSlotsGlowHue[_chestIndex];
            float hue = hueArray[Index];
            
            if (hue is -1f) return;
            
            Color color = Main.hslToRgb(hue, 1f, 0.5f);
            float opacity = ExtremeStorageGUI.ChestSlotsGlowTimer / 300f;
            opacity *= opacity;
            BgColor = Color.Lerp(UIColor.ItemSlotBg, color, opacity / 2f);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            if (!Interactable && IsMouseHovering)
            {
                Main.instance.MouseText(GetText("UI.ExtremeStorage.ChestBeingUsed"));
            }
        }

        public override bool Interactable => !ChestBeingUsed && !Main.LocalPlayer.ItemAnimationActive;
    }
}