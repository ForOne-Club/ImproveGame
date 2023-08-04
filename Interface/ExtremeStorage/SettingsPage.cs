using ImproveGame.Common.Packets.NetStorager;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.ExtremeStorage
{
    public class SettingsPage : View
    {
        private TEExtremeStorage Storage => ExtremeStorageGUI.Storage;
        private readonly LongSwitch _recipesSwitch;
        private readonly LongSwitch _buffsSwitch;
        private readonly LongSwitch _stationsSwitch;
        private readonly LongSwitch _bannersSwitch;

        public SettingsPage()
        {
            Left.Set(ExtremeStorageGUI.CurrentGroup is ItemGroup.Setting ? 0f : -9999f, 0f);
            Width.Set(0f, 1f);
            Height.Set(0f, 1f);
            
            _recipesSwitch = new LongSwitch(() => Storage.UseForCrafting,
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
            _recipesSwitch.Join(this);
            
            _buffsSwitch = new LongSwitch(
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
            _buffsSwitch.Join(this);
            
            _stationsSwitch = new LongSwitch(
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
            _stationsSwitch.Join(this);
            
            _bannersSwitch = new LongSwitch(
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
            _bannersSwitch.Join(this);
            
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