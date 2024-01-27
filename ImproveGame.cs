using ImproveGame.Common;
using ImproveGame.Common.ModSystems;
using MonoMod.Cil;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace ImproveGame;

public class ImproveGame : Mod
{
    // 额外BUFF槽
    public override uint ExtraPlayerBuffSlots => (uint)Config.ExtraPlayerBuffSlots;
    public static ImproveGame Instance => ModContent.GetInstance<ImproveGame>();

    public override void Load()
    {
        AddContent<NetModuleLoader>();
        ChatManager.Register<BgItemTagHandler>("bgitem");
        ChatManager.Register<CenteredItemTagHandler>("centeritem");

        // On_Main.DrawSettingButton += On_Main_DrawSettingButton;

        // ban 掉原版设置按钮
        IL_Main.DrawInterface_29_SettingsButton += il =>
        {
            new ILCursor(il).EmitRet();
        };
    }

    /*private void On_Main_DrawSettingButton(On_Main.orig_DrawSettingButton orig, ref bool mouseOver, ref float scale, int posX, int posY, string text, string textSizeMatcher, Action clickAction)
    {
        Vector2 val = FontAssets.MouseText.Value.MeasureString(textSizeMatcher);
        Vector2 vector2 = FontAssets.MouseText.Value.MeasureString(text);
        Vector2 vector3 = FontAssets.DeathText.Value.MeasureString(text);
        float num = val.X / vector2.X;
        if (mouseOver)
        {
            if ((double)scale < 0.96)
            {
                scale += 0.02f;
            }
        }
        else if ((double)scale > 0.8)
        {
            scale -= 0.02f;
        }
        UILinkPointNavigator.SetPosition(308, new Vector2((float)posX, (float)posY));
        for (int i = 0; i < 5; i++)
        {
            int num2 = 0;
            int num3 = 0;
            Color color = Color.Black;
            if (i == 0)
            {
                num2 = -2;
            }
            if (i == 1)
            {
                num2 = 2;
            }
            if (i == 2)
            {
                num3 = -2;
            }
            if (i == 3)
            {
                num3 = 2;
            }
            if (i == 4)
            {
                color = Color.White;
            }
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, text, new Vector2((float)(posX + num2), (float)(posY + num3)), color, 0f, new Vector2(vector3.X / 2f, vector3.Y / 2f), (scale - 0.2f) * num, (SpriteEffects)0, 0f);
        }
        if (Main.mouseX > posX - vector3.X / 2f && Main.mouseX < posX + vector3.X / 2f && Main.mouseY > posY - vector3.Y / 2f && Main.mouseY < posY + vector3.Y / 2f - 10f && !Main.LocalPlayer.mouseInterface)
        {
            if (!PlayerInput.IgnoreMouseInterface)
            {
                if (!mouseOver)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
                mouseOver = true;
                Main.player[Main.myPlayer].mouseInterface = true;
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    mouseOver = false;
                    scale = 0.8f;
                    clickAction();
                }
            }
        }
        else
        {
            mouseOver = false;
        }
    }*/

    public override void Unload()
    {
        Config = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);

    public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
}