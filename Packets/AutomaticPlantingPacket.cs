namespace ImproveGame.Packets;

[AutoSync]
public class AutomaticPlantingPacket : NetModule
{
    private int _tileCoordX;
    private int _tileCoordY;
    private byte _tileStyle;
    public static AutomaticPlantingPacket Get(int coordX, int coordY, byte style)
    {
        var module = NetModuleLoader.Get<AutomaticPlantingPacket>();
        module._tileCoordX = coordX;
        module._tileCoordY = coordY;
        module._tileStyle = style;
        return module;
    }

    public override void Receive()
    {
        WorldGen.KillTile(_tileCoordX, _tileCoordY);
        WorldGen.PlaceTile(
            _tileCoordX,
            _tileCoordY,
            TileID.ImmatureHerbs,
            true,
            false,
            -1,
            _tileStyle);

        // 转发
        if (Main.netMode is NetmodeID.Server)
        {
            var packet = Get(_tileCoordX, _tileCoordY, _tileStyle);
            packet.Send(-1, packet.Sender, false);
        }
    }
}
