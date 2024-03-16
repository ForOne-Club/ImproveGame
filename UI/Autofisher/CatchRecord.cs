using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace ImproveGame.UI.Autofisher;

public class CatchRecord : ModSystem
{
    private class CatchData(Item item) : TagSerializable
    {
        public static readonly Func<TagCompound, CatchData> DESERIALIZER = Load;

        internal readonly Item Item = item;

        public TagCompound SerializeData()
        {
            var tag = new TagCompound();

            if (Item.type <= 0)
                return tag;

            if (Item.ModItem == null)
            {
                tag.Set("mod", "Terraria");
                tag.Set("id", Item.netID);
            }
            else
            {
                tag.Set("mod", Item.ModItem.Mod.Name);
                tag.Set("name", Item.ModItem.Name);
            }

            return tag;
        }

        public static CatchData Load(TagCompound tag)
        {
            var item = new Item();
            string modName = tag.GetString("mod");
            if (modName == "")
            {
                item.netDefaults(0);
                return new CatchData(item);
            }

            if (modName == "Terraria")
            {
                item.netDefaults(tag.GetInt("id"));
            }
            else
            {
                if (ModContent.TryFind(modName, tag.GetString("name"), out ModItem modItem))
                {
                    item.SetDefaults(modItem.Type);
                }
                else
                {
                    item.SetDefaults(ModContent.ItemType<UnloadedItem>());
                    ((UnloadedItem)item.ModItem).Setup(tag);
                }
            }

            return new CatchData(item);
        }

        public override int GetHashCode() => Item.GetHashCode();
    }

    private static List<Item> _itemsCaught = [];

    public static void AddCatch(int type)
    {
        if (_itemsCaught.All(i => i.type != type))
            _itemsCaught.Add(new Item(type));
    }

    public static List<Item> GetRecordedCatches => _itemsCaught;

    public override void ClearWorld()
    {
        _itemsCaught = [];
    }

    public override void SaveWorldData(TagCompound tag)
    {
        var catches = _itemsCaught.Select(item => new CatchData(item)).ToList();
        tag["catches"] = catches;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        var catchData = tag.Get<List<CatchData>>("catches") ?? [];
        _itemsCaught = catchData.Select(data => data.Item).ToList();
    }
}