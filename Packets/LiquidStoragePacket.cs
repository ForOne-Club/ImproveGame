using ImproveGame.Common.ModPlayers;

namespace ImproveGame.Packets;

[AutoSync]
public class LiquidStoragePacket : NetModule
{
    private byte _whoAmI;
    private int _water;
    private int _lava;
    private int _honey;

    public static void Send(int to, int from, DataPlayer player, bool runLocally = false) =>
        Get(player).Send(to, from, runLocally: runLocally);

    public static LiquidStoragePacket Get(DataPlayer player)
    {
        var packet = ModContent.GetInstance<LiquidStoragePacket>();
        packet._whoAmI = (byte)player.Player.whoAmI;
        packet._water = player.LiquidWandWater;
        packet._lava = player.LiquidWandLava;
        packet._honey = player.LiquidWandHoney;
        return packet;
    }

    public override void Receive()
    {
        if (!DataPlayer.TryGet(Main.player[_whoAmI], out var player))
            return;
        player.LiquidWandWater = _water;
        player.LiquidWandLava = _lava;
        player.LiquidWandHoney = _honey;
    }
}