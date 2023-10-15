using ImproveGame.Common.Packets.NetStorager;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.ExtremeStorage
{
    public class SettingsPage : View
    {
        private TEExtremeStorage Storage => ExtremeStorageGUI.Storage;

        public SettingsPage()
        {
            Left.Set(ExtremeStorageGUI.CurrentGroup is ItemGroup.Setting ? 0f : -9999f, 0f);
            Width.Set(0f, 1f);
            Height.Set(0f, 1f);
            
            LongSwitch recipesSwitch = new(() => Storage.UseForCrafting,
                state =>
                {
                    Storage.UseForCrafting = state;
                    SyncDataPacket.Get(Storage.ID).Send();
                },
                "UI.ExtremeStorage.CountForCrafting")
            {
                First = true,
                Relative = RelativeMode.Vertical
            };
            recipesSwitch.Join(this);
            
            LongSwitch buffsSwitch = new(
                () => Storage.UseUnlimitedBuffs,
                state =>
                {
                    Storage.UseUnlimitedBuffs = state;
                    SyncDataPacket.Get(Storage.ID).Send();
                },
                "UI.ExtremeStorage.UseUnlimitedPotion")
            {
                Relative = RelativeMode.Vertical
            };
            buffsSwitch.Join(this);
            
            LongSwitch stationsSwitch = new(
                () => Storage.UsePortableStations,
                state =>
                {
                    Storage.UsePortableStations = state;
                    SyncDataPacket.Get(Storage.ID).Send();
                    Recipe.FindRecipes();
                },
                "UI.ExtremeStorage.UsePortableStation")
            {
                Relative = RelativeMode.Vertical
            };
            stationsSwitch.Join(this);
            
            LongSwitch bannersSwitch = new(
                () => Storage.UsePortableBanner,
                state =>
                {
                    Storage.UsePortableBanner = state;
                    SyncDataPacket.Get(Storage.ID).Send();
                    Recipe.FindRecipes();
                },
                "UI.ExtremeStorage.UsePortableBanner")
            {
                Relative = RelativeMode.Vertical
            };
            bannersSwitch.Join(this);
            
            var uiText = new UIText(GetText("UI.ExtremeStorage.BasicIntroduction"))
            {
                // IsWrapped = true, // 为了看着舒服，使用手动换行
                TextOriginX = 0f,
                Top = StyleDimension.FromPixels(190f),
                Width = StyleDimension.FromPixels(430f),
                Left = StyleDimension.FromPixels(8f)
            };
            Append(uiText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ExtremeStorageGUI.CurrentGroup is not ItemGroup.Setting && Left.Pixels != -9999f)
            {
                Left.Set(-9999f, 0f);
                Recalculate();
            }

            if (ExtremeStorageGUI.CurrentGroup is ItemGroup.Setting && Left.Pixels != 0f)
            {
                Left.Set(0f, 0f);
                Recalculate();
            }
        }
    }
}