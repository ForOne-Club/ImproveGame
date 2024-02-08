using ImproveGame.Common.ModSystems;
using ImproveGame.Packets;
using Terraria.GameInput;
using Terraria.UI.Chat;

namespace ImproveGame.Content.Functions
{
    public class RefreshTravelShopSystem : ModSystem
    {
        // 多人模式，由于发包服务器->服务器相应->发回客户端有延迟，所以会显示“刷新中...”
        internal static bool Refreshing = false;
        internal static float AnimationTimer = 0; // 三个点的Animation，0->1->2->3，到4时变回0
        private bool focused; // 上一帧是否鼠标hover在上面
        internal static bool OldMouseLeft { get; private set; } // 上一帧是否mouseLeft

        internal static string DisplayText
        {
            get
            {
                string text = GetText("Tips.Refresh");
                if (Refreshing)
                {
                    text = GetText("Tips.Refreshing");
                    for (int i = 0; i < (int)AnimationTimer; i++)
                    {
                        text += '.';
                    }
                }
                return text;
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.LocalPlayer.talkNPC != -1 && Main.npc[Main.LocalPlayer.talkNPC].type == NPCID.TravellingMerchant && Main.npcShop == 0 && !ModIntegrationsSystem.DialogueTweakLoaded && Config.TravellingMerchantRefresh)
            {
                AnimationTimer += 0.05f;
                if (AnimationTimer >= 4f)
                {
                    AnimationTimer = 0f;
                }
                if (focused && Main.mouseLeft && !OldMouseLeft)
                {
                    RefreshShopPacket.Get().Send(-1, -1, true);
                }
            }
            OldMouseLeft = Main.mouseLeft;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int dialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if (dialogIndex != -1)
            {
                // 直接替换原版的绘制层
                layers.Insert(dialogIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Travelling Merchant Refresh Button",
                    delegate {
                        if (Main.LocalPlayer.talkNPC != -1 && Main.npc[Main.LocalPlayer.talkNPC].type == NPCID.TravellingMerchant && Main.npcShop == 0 && !ModIntegrationsSystem.DialogueTweakLoaded && Config.TravellingMerchantRefresh)
                        {
                            Vector2 scale = new(0.9f);
                            string text = DisplayText; // 要显示的文本
                            int numLines = Main.instance._textDisplayCache.AmountOfLines;
                            Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, scale); // size of "Shop" with scale 0.9

                            Vector2 value2 = new(1f);
                            if (stringSize.X > 260f)
                                value2.X *= 260f / stringSize.X;

                            float posButton1 = 180 + (Main.screenWidth - 800) / 2;
                            float posButton2 = posButton1 + ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("LegacyInterface.28"), scale).X + 30f; // 28 是 商店
                            float posButton3 = posButton2 + ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("LegacyInterface.52"), scale).X + 30f; // 52 是 关闭
                            Vector2 position = new(posButton3, 130 + numLines * 30);

                            // if the player is hovering over the button
                            if (Main.MouseScreen.Between(position, position + stringSize * scale * value2.X) && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.LocalPlayer.mouseInterface = true;
                                Main.LocalPlayer.releaseUseItem = false;
                                scale *= 1.2f; // make button bigger

                                if (!focused)
                                {
                                    SoundEngine.PlaySound(SoundID.MenuTick);
                                }

                                focused = true;
                            }
                            else
                            {
                                if (focused)
                                {
                                    SoundEngine.PlaySound(SoundID.MenuTick);
                                }

                                focused = false;
                            }

                            ChatManager.DrawColorCodedStringWithShadow(spriteBatch: Main.spriteBatch,
                                font: FontAssets.MouseText.Value,
                                text: text,
                                position: position + stringSize * value2 * 0.5f,
                                baseColor: !focused ? new Color(228, 206, 114, Main.mouseTextColor / 2) : new Color(255, 231, 69),
                                shadowColor: (!focused) ? Color.Black : Color.Brown,
                                rotation: 0f,
                                origin: stringSize * 0.5f,
                                baseScale: scale);
                        }
                        return true;
                    }, InterfaceScaleType.UI));
            }
        }
    }
}
