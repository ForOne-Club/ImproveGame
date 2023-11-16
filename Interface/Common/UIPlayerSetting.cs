using ImproveGame.Interface.GUI.PlayerProperty;
using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.Common
{
    public class UIPlayerSetting : ModPlayer
    {
        public bool SuperVault_HeCheng;
        public bool SuperVault_SmartGrab;
        public bool SuperVault_OverflowGrab;
        public bool FuzzySearch;
        public bool SearchTooltip;

        public TagCompound ProCatsPos = new TagCompound();
        public TagCompound ProCatsFav = new TagCompound();
        public TagCompound ProFavs = new TagCompound();

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(nameof(SuperVault_HeCheng), out SuperVault_HeCheng);
            tag.TryGet(nameof(SuperVault_SmartGrab), out SuperVault_SmartGrab);
            tag.TryGet(nameof(SuperVault_OverflowGrab), out SuperVault_OverflowGrab);
            tag.TryGet(nameof(FuzzySearch), out FuzzySearch);
            tag.TryGet(nameof(SearchTooltip), out SearchTooltip);

            tag.TryGet("ProCatsPos", out ProCatsPos);
            tag.TryGet("ProCatsFav", out ProCatsFav);
            tag.TryGet("ProFavs", out ProFavs);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Set(nameof(SuperVault_HeCheng), SuperVault_HeCheng);
            tag.Set(nameof(SuperVault_SmartGrab), SuperVault_SmartGrab);
            tag.Set(nameof(SuperVault_OverflowGrab), SuperVault_OverflowGrab);
            tag.Set(nameof(FuzzySearch), FuzzySearch);
            tag.Set(nameof(SearchTooltip), SearchTooltip);

            TagCompound proCatsPos = new TagCompound();
            TagCompound proCatsFav = new TagCompound();
            TagCompound proFavs = new TagCompound();

            foreach (var proCat in PlayerPropertySystem.Instance.PropertyCategorys)
            {
                proCatsPos.Set(proCat.Key, proCat.Value.UIPosition);
                proCatsFav.Set(proCat.Key, proCat.Value.Favorite);

                TagCompound proFav = new TagCompound();

                foreach (var property in proCat.Value.BasePropertys)
                {
                    proFav.Set(property.Name, property.Favorite);
                }

                proFavs.Set(proCat.Key, proFav);
            }

            tag.Set("ProCatsPos", proCatsPos);
            tag.Set("ProCatsFav", proCatsFav);
            tag.Set("ProFavs", proFavs);
        }
    }
}
