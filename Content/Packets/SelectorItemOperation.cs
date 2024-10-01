using ImproveGame.Content.Items;
using ImproveGame.Core;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Packets;

[AutoSync]
public class SelectorItemOperation : NetModule
{
    private Point _start;
    private Point _end;
    private TagCompound _tag;

    public static SelectorItemOperation Get(Point start, Point end)
    {
        var module = NetModuleLoader.Get<SelectorItemOperation>();
        module._start = start;
        module._end = end;
        return module;
    }

    public static void Proceed(Point start, Point end)
    {
        if (Main.netMode is NetmodeID.Server)
            return;
        var module = Get(start, end);
        module.Send();
    }

    public override void Send(ModPacket p)
    {
        if (Main.LocalPlayer.HeldItem?.ModItem is not SelectorItem selector)
            return;
        selector.NetSend(p);
    }

    public override void Read(BinaryReader r)
    {
        if (Main.player[Sender].HeldItem?.ModItem is not SelectorItem selector)
            return;
        selector.NetReceive(r);
    }

    public override void Receive()
    {
        var player = Main.player[Sender];
        if (player.HeldItem?.ModItem is not SelectorItem selector)
            return;
        var rectangle = new Rectangle((int)MathF.Min(_start.X, _end.X), (int)MathF.Min(_start.Y, _end.Y),
            (int)MathF.Abs(_start.X - _end.X) + 1, (int)MathF.Abs(_start.Y - _end.Y) + 1);
        CoroutineSystem.TileRunner.Run(selector.ModifyTiles(player, rectangle));
    }
}