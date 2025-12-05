using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    [CloneByReference] public Item Block = new Item();
    [CloneByReference] public Item Platform = new Item();
    [CloneByReference] public Item Workbench = new Item();
    [CloneByReference] public Item Table = new Item();
    [CloneByReference] public Item Chair = new Item();
    [CloneByReference] public Item Door = new Item();
    [CloneByReference] public Item Chest = new Item();
    [CloneByReference] public Item Bed = new Item();
    [CloneByReference] public Item Bookcase = new Item();
    [CloneByReference] public Item Bathtub = new Item();
    [CloneByReference] public Item Candelabra = new Item();
    [CloneByReference] public Item Candle = new Item();
    [CloneByReference] public Item Chandelier = new Item();
    [CloneByReference] public Item Clock = new Item();
    [CloneByReference] public Item Dresser = new Item();
    [CloneByReference] public Item Lamp = new Item();
    [CloneByReference] public Item Lantern = new Item();
    [CloneByReference] public Item Piano = new Item();
    [CloneByReference] public Item Sink = new Item();
    [CloneByReference] public Item Sofa = new Item();
    [CloneByReference] public Item Toilet = new Item();
    [CloneByReference] public Item Torch = new Item();
    [CloneByReference] public Item Campfire = new Item();
    [CloneByReference] public Item Wall = new Item();

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(Block)] = Block;
        tag[nameof(Platform)] = Platform;
        tag[nameof(Workbench)] = Workbench;
        tag[nameof(Table)] = Table;
        tag[nameof(Chair)] = Chair;
        tag[nameof(Door)] = Door;
        tag[nameof(Chest)] = Chest;
        tag[nameof(Bed)] = Bed;
        tag[nameof(Bookcase)] = Bookcase;
        tag[nameof(Bathtub)] = Bathtub;
        tag[nameof(Candelabra)] = Candelabra;
        tag[nameof(Candle)] = Candle;
        tag[nameof(Chandelier)] = Chandelier;
        tag[nameof(Clock)] = Clock;
        tag[nameof(Dresser)] = Dresser;
        tag[nameof(Lamp)] = Lamp;
        tag[nameof(Lantern)] = Lantern;
        tag[nameof(Piano)] = Piano;
        tag[nameof(Sink)] = Sink;
        tag[nameof(Sofa)] = Sofa;
        tag[nameof(Toilet)] = Toilet;
        tag[nameof(Torch)] = Torch;
        tag[nameof(Campfire)] = Campfire;
        tag[nameof(Wall)] = Wall;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(nameof(Block), out Item block))
            Block = block;
        if (tag.TryGet(nameof(Platform), out Item platform))
            Platform = platform;
        if (tag.TryGet(nameof(Workbench), out Item workbench))
            Workbench = workbench;
        if (tag.TryGet(nameof(Table), out Item table))
            Table = table;
        if (tag.TryGet(nameof(Chair), out Item chair))
            Chair = chair;
        if (tag.TryGet(nameof(Door), out Item door))
            Door = door;
        if (tag.TryGet(nameof(Chest), out Item chest))
            Chest = chest;
        if (tag.TryGet(nameof(Bed), out Item bed))
            Bed = bed;
        if (tag.TryGet(nameof(Bookcase), out Item bookcase))
            Bookcase = bookcase;
        if (tag.TryGet(nameof(Bathtub), out Item bathtub))
            Bathtub = bathtub;
        if (tag.TryGet(nameof(Candelabra), out Item candelabra))
            Candelabra = candelabra;
        if (tag.TryGet(nameof(Candle), out Item candle))
            Candle = candle;
        if (tag.TryGet(nameof(Chandelier), out Item chandelier))
            Chandelier = chandelier;
        if (tag.TryGet(nameof(Clock), out Item clock))
            Clock = clock;
        if (tag.TryGet(nameof(Dresser), out Item dresser))
            Dresser = dresser;
        if (tag.TryGet(nameof(Lamp), out Item lamp))
            Lamp = lamp;
        if (tag.TryGet(nameof(Lantern), out Item lantern))
            Lantern = lantern;
        if (tag.TryGet(nameof(Piano), out Item piano))
            Piano = piano;
        if (tag.TryGet(nameof(Sink), out Item sink))
            Sink = sink;
        if (tag.TryGet(nameof(Sofa), out Item sofa))
            Sofa = sofa;
        if (tag.TryGet(nameof(Toilet), out Item toliet))
            Toilet = toliet;
        if (tag.TryGet(nameof(Torch), out Item torch))
            Torch = torch;
        if (tag.TryGet(nameof(Campfire), out Item campfire))
            Campfire = campfire;
        if (tag.TryGet(nameof(Wall), out Item wall))
            Wall = wall;
    }

    public override void NetSend(BinaryWriter writer)
    {
        ItemIO.Send(Block,writer,true,true);
        ItemIO.Send(Platform,writer,true,true);
        ItemIO.Send(Workbench,writer,true,true);
        ItemIO.Send(Table,writer,true,true);
        ItemIO.Send(Chair,writer,true,true);
        ItemIO.Send(Door,writer,true,true);
        ItemIO.Send(Chest,writer,true,true);
        ItemIO.Send(Bed,writer,true,true);
        ItemIO.Send(Bookcase,writer,true,true);
        ItemIO.Send(Bathtub,writer,true,true);
        ItemIO.Send(Candelabra,writer,true,true);
        ItemIO.Send(Candle,writer,true,true);
        ItemIO.Send(Chandelier,writer,true,true);
        ItemIO.Send(Clock,writer,true,true);
        ItemIO.Send(Dresser,writer,true,true);
        ItemIO.Send(Lamp,writer,true,true);
        ItemIO.Send(Lantern,writer,true,true);
        ItemIO.Send(Piano,writer,true,true);
        ItemIO.Send(Sink,writer,true,true);
        ItemIO.Send(Sofa,writer,true,true);
        ItemIO.Send(Toilet,writer,true,true);
        ItemIO.Send(Torch,writer,true,true);
        ItemIO.Send(Campfire,writer,true,true);
        ItemIO.Send(Wall,writer,true,true);
    }

    public override void NetReceive(BinaryReader reader)
    {
        Block = ItemIO.Receive(reader,true,true);
        Platform = ItemIO.Receive(reader,true,true);
        Workbench = ItemIO.Receive(reader,true,true);
        Table = ItemIO.Receive(reader,true,true);
        Chair = ItemIO.Receive(reader,true,true);
        Door = ItemIO.Receive(reader,true,true);
        Chest = ItemIO.Receive(reader,true,true);
        Bed = ItemIO.Receive(reader,true,true);
        Bookcase = ItemIO.Receive(reader,true,true);
        Bathtub = ItemIO.Receive(reader,true,true);
        Candelabra = ItemIO.Receive(reader,true,true);
        Candle = ItemIO.Receive(reader,true,true);
        Chandelier = ItemIO.Receive(reader,true,true);
        Clock = ItemIO.Receive(reader,true,true);
        Dresser = ItemIO.Receive(reader,true,true);
        Lamp = ItemIO.Receive(reader,true,true);
        Lantern = ItemIO.Receive(reader,true,true);
        Piano = ItemIO.Receive(reader,true,true);
        Sink = ItemIO.Receive(reader,true,true);
        Sofa = ItemIO.Receive(reader,true,true);
        Toilet = ItemIO.Receive(reader,true,true);
        Torch = ItemIO.Receive(reader,true,true);
        Campfire = ItemIO.Receive(reader,true,true);
        Wall = ItemIO.Receive(reader,true,true);
    }
}
