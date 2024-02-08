using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions;
using Terraria.Chat;

namespace ImproveGame.Packets.WorldFeatures;

[AutoSync]
public class FestivalPacket : NetModule
{
    private byte _festivalType; // 0: 万圣 | 1: 圣诞
    private byte _setMode; // 0: 关 | 1: 开 | 2: 切换

    public static void SetHalloween(bool enabled)
    {
        var module = NetModuleLoader.Get<FestivalPacket>();
        module._festivalType = 0;
        module._setMode = (byte)enabled.ToInt();
        module.Send(runLocally: Main.netMode is NetmodeID.SinglePlayer);
    }

    public static void SetXMas(bool enabled)
    {
        var module = NetModuleLoader.Get<FestivalPacket>();
        module._festivalType = 1;
        module._setMode = (byte)enabled.ToInt();
        module.Send(runLocally: Main.netMode is NetmodeID.SinglePlayer);
    }

    public override void Receive()
    {
        if (!NetPasswordSystem.Permitted(Sender))
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.ImproveGame.UI.WorldFeature.NotPermitted"),
                Color.Red, Sender);
            return;
        }

        ref bool featureFlag = ref GetFestivalFeatureFlag();
        featureFlag = _setMode switch
        {
            0 => false,
            1 => true,
            2 => !featureFlag,
            _ => featureFlag
        };

        // 转发
        if (Main.netMode is NetmodeID.Server)
        {
            _setMode = (byte)featureFlag.ToInt();
            Send();
        }
    }

    private ref bool GetFestivalFeatureFlag()
    {
        switch (_festivalType)
        {
            case 0:
                return ref ForceFestivalSystem.ForceHalloween;
            case 1:
                return ref ForceFestivalSystem.ForceXMas;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}