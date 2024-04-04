using ImproveGame.Core;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.ChainedAmmo;

public class AmmoChain : TagSerializable, ICloneable
{
    public List<Ammo> Chain;
    public Color Color;
    
    public AmmoChain()
    {
        Chain = [];
        Color = Color.White;
    }

    public record Ammo(ItemTypeData ItemData, int Times) : TagSerializable
    {
        public int Times = Times;
        public ItemTypeData ItemData = ItemData;
        
        // tML反射获取叫这个的Field，获取不到就报错，不能删啊
        public static Func<TagCompound, Ammo> DESERIALIZER = s => DeserializeAmmo(s);

        public static Ammo DeserializeAmmo(TagCompound tag) =>
            new (tag.Get<ItemTypeData>("item"), tag.GetInt("times"));

        public TagCompound SerializeData() => new()
        {
            ["item"] = ItemData,
            ["times"] = Times
        };
    }

    // tML反射获取叫这个的Field，获取不到就报错，不能删啊
    public static Func<TagCompound, AmmoChain> DESERIALIZER = s => DeserializeData(s);

    public static bool IsTagInvalid(TagCompound tag)
    {
        return !tag.ContainsKey("chain") || !tag.ContainsKey("color");
    }

    public static AmmoChain DeserializeData(TagCompound tag)
    {
        var output = new AmmoChain();
        tag.TryGet("chain", out output.Chain);
        tag.TryGet("color", out output.Color);

        return output;
    }

    public TagCompound SerializeData() => new()
    {
        ["chain"] = Chain,
        ["color"] = Color
    };

    public object Clone() =>
        new AmmoChain
        {
            Chain = Chain.ToList(),
            Color = Color
        };
}