using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace ImproveGame.Common.GlobalItems;

public class ImprovePrefixItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public HashSet<PrefixInfo> Prefixs = new();
    internal static int RerollCounter; // 在一个方法内用完即弃，因此static

    public override void LoadData(Item item, TagCompound tag)
    {
        Prefixs = tag.Get<List<PrefixInfo>>("prefixInfos").ToHashSet();
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        if (Prefixs.Count > 0)
            tag["prefixInfos"] = Prefixs.ToList();
    }

    // 这个在重铸Roll循环开始前调用，因此可以在这里重设重铸Reroll计数器
    public override bool? PrefixChance(Item item, int pre, UnifiedRandom rand)
    {
        RerollCounter = 0;
        return base.PrefixChance(item, pre, rand);
    }

    public override bool AllowPrefix(Item item, int pre)
    {
        RerollCounter++;
        if (RerollCounter <= 10 && Prefixs.Any(i => i.PrefixId == pre))
            return false;

        return base.AllowPrefix(item, pre);
    }

    public override void PostReforge(Item item)
    {
        Prefixs.Add(new(item.prefix, item.value));
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
                info.PrefixId = ModContent.Find<ModPrefix>(info.ModName, info.ModPrefixName).Type;
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

