using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace ImproveGame.Common.GlobalItems;

public class ImprovePrefixItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public HashSet<PrefixInfo> Prefixs = new();

    public override void LoadData(Item item, TagCompound tag)
    {
        Prefixs = tag.Get<List<PrefixInfo>>("prefixInfos").ToHashSet();
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        if (Prefixs.Count > 0)
            tag["prefixInfos"] = Prefixs.ToList();
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        writer.Write((ushort)Prefixs.Count);
        foreach (var prefixInfo in Prefixs) {
            writer.Write((ushort)prefixInfo.PrefixId);
            writer.Write(prefixInfo.Cost);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        Prefixs.Clear();
        ushort prefixCount = reader.ReadUInt16();
        for (int i = 0; i < prefixCount; i++) {
            var prefixId = reader.ReadUInt16();
            var cost = reader.ReadInt32();
            Prefixs.Add(new PrefixInfo(prefixId, cost));
        }
    }

    public override void PostReforge(Item item)
    {
        if (Prefixs.Count >= ushort.MaxValue) return;
        Prefixs.Add(new PrefixInfo(item.prefix, item.value));
    }
}

public class PrefixInfo : TagSerializable
{
    public static readonly Func<TagCompound, PrefixInfo> DESERIALIZER = Load;

    public int PrefixId;
    public int Cost;
    public string ModPrefixName;
    public string ModName;

    public PrefixInfo(int prefixId, int cost)
    {
        PrefixId = prefixId;
        Cost = cost;
        ModPrefixName = PrefixId >= PrefixID.Count ? PrefixLoader.GetPrefix(PrefixId)?.Name : "";
        ModName = PrefixId >= PrefixID.Count ? PrefixLoader.GetPrefix(PrefixId)?.Mod.Name : "Terraria";
    }

    public TagCompound SerializeData()
    {
        var tag = new TagCompound
        {
            ["prefixId"] = PrefixId,
            ["cost"] = Cost
        };
        if (PrefixId >= PrefixID.Count && !string.IsNullOrEmpty(ModName) && !string.IsNullOrEmpty(ModPrefixName))
        {
            tag["modPrefixName"] = ModPrefixName;
            tag["modName"] = ModName;
        }
        return tag;
    }

    public static PrefixInfo Load(TagCompound tag)
    {
        var info = new PrefixInfo(tag.GetInt("prefixId"), tag.GetInt("cost"))
        {
            ModPrefixName = tag.GetString("modPrefixName"),
            ModName = tag.GetString("modName")
        };

        if (info.PrefixId >= PrefixID.Count && !string.IsNullOrEmpty(info.ModName) && !string.IsNullOrEmpty(info.ModPrefixName) && ModLoader.HasMod(info.ModName))
        {
            try
            {
                info.PrefixId = ModContent.Find<ModPrefix>(info.ModName, info.ModPrefixName)?.Type ?? -1;
            }
            catch
            {
                info.PrefixId = -1;
            }
        }

        return info;
    }
    
    public override bool Equals(object obj)
    {
        return obj is PrefixInfo info && info.PrefixId == PrefixId;
    }

    public override int GetHashCode()
    {
        return PrefixId.GetHashCode();
    }
}

