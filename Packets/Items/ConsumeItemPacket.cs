namespace ImproveGame.Packets.Items;

public class ConsumeItemPacket : NetModule
{
    private IEnumerable<KeyValuePair<int, int>> _itemsConsumed;

    public static ConsumeItemPacket Get(IEnumerable<KeyValuePair<int, int>> itemsConsumed)
    {
        var module = NetModuleLoader.Get<ConsumeItemPacket>();
        module._itemsConsumed = itemsConsumed;
        return module;
    }

    public static void Proceed(IEnumerable<KeyValuePair<int, int>> itemsConsumed, int player)
    {
        if (Main.netMode is not NetmodeID.Server)
            return;
        var module = Get(itemsConsumed);
        module.Send(player);
    }

    public override void Send(ModPacket p)
    {
        p.Write(_itemsConsumed.Count());
        foreach ((int key, int value) in _itemsConsumed)
        {
            p.Write(key);
            p.Write(value);
        }
    }

    public override void Read(BinaryReader r)
    {
        int count = r.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int type = r.ReadInt32();
            int amount = r.ReadInt32();

            for (int j = 0; j < 58; j++)
            {
                var item = Main.LocalPlayer.inventory[j];
                if (item.type != type)
                    continue;
                if (item.IsAir || !item.consumable || !ItemLoader.ConsumeItem(item, Main.LocalPlayer))
                    continue;

                int numConsumed = Math.Min(item.stack, amount);
                item.stack -= numConsumed;
                amount -= numConsumed;
                if (item.stack < 1)
                    item.TurnToAir();

                if (amount <= 0)
                    break;
            }
        }
    }

    // 逻辑已经放在Read里面了，虽然不规范，但是方便
    public override void Receive()
    {
    }
}