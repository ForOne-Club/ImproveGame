using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace ImproveGame.Core;

public class ItemTypeData(Item item) : TagSerializable
{
    public static readonly Func<TagCompound, ItemTypeData> DESERIALIZER = Load;

    internal readonly Item Item = item;

    public TagCompound SerializeData()
    {
        var tag = new TagCompound();

        if (Item.type <= ItemID.None)
            return tag;

        if (Item.ModItem is null)
        {
            tag.Set("mod", "Terraria");
            tag.Set("id", Item.netID);
        }
        else
        {
            if (Item.ModItem is UnloadedItem unloadedItem)
            {
                if (string.IsNullOrEmpty(unloadedItem.ModName) || string.IsNullOrEmpty(unloadedItem.ItemName))
                    return tag;
                tag.Set("mod", unloadedItem.ModName);
                tag.Set("name", unloadedItem.ItemName);
            }
            else
            {
                tag.Set("mod", Item.ModItem.Mod.Name);
                tag.Set("name", Item.ModItem.Name);
            }
        }

        return tag;
    }

    public static ItemTypeData Load(TagCompound tag)
    {
        var item = new Item();
        string modName = tag.GetString("mod");
        if (string.IsNullOrEmpty(modName))
        {
            item.netDefaults(0);
            return new ItemTypeData(item);
        }

        if (modName == "Terraria")
        {
            item.netDefaults(tag.GetInt("id"));
        }
        else
        {
            var itemName = tag.GetString("name");
            if (string.IsNullOrEmpty(itemName))
            {
                item.netDefaults(0);
                return new ItemTypeData(item);
            }

            if (ModContent.TryFind(modName, itemName, out ModItem modItem))
            {
                item.SetDefaults(modItem.Type);
            }
            else
            {
                item.SetDefaults(ModContent.ItemType<UnloadedItem>());
                ((UnloadedItem)item.ModItem).Setup(tag);
            }
        }

        return new ItemTypeData(item);
    }

    public override int GetHashCode() => Item.GetHashCode();
}

public static class ItemTypeDataExtensions
{
    public static void Write(this BinaryWriter w, ItemTypeData data)
    {
        var tag = data.SerializeData();
        string modName = tag.GetString("mod");
        w.Write(modName);

        if (modName == "Terraria")
        {
            ushort type = (ushort)tag.GetInt("id");
            w.Write(type);
        }
        else
        {
            var itemName = tag.GetString("name");
            w.Write(itemName);
        }
    }

    public static ItemTypeData ReadItemTypeData(this BinaryReader r)
    {
        var tag = new TagCompound();
        string modName = r.ReadString();
        tag.Set("mod", modName);

        if (modName == "Terraria")
        {
            tag.Set("id", (int)r.ReadUInt16());
        }
        else
        {
            tag.Set("name", r.ReadString());
        }

        return ItemTypeData.Load(tag);
    }

    public static void Write(this BinaryWriter w, List<ItemTypeData> data)
    {
        w.Write(data.Count);
        foreach (var d in data)
            w.Write(d);
    }

    public static List<ItemTypeData> ReadListItemTypeData(this BinaryReader r)
    {
        int count = r.ReadInt32();
        List<ItemTypeData> data = [];
        for (int i = 0; i < count; i++)
            data.Add(r.ReadItemTypeData());
        return data;
    }
}