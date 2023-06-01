using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ImproveGame.QuickAssetReference;
public static class ModAssets_Texture2D
{
    public static Asset<Texture2D> iconAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(iconPath);
    public static Asset<Texture2D> iconImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(iconPath, AssetRequestMode.ImmediateLoad);
    public static string iconPath = "icon";
    public static Asset<Texture2D> icon_workshopAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(icon_workshopPath);
    public static Asset<Texture2D> icon_workshopImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(icon_workshopPath, AssetRequestMode.ImmediateLoad);
    public static string icon_workshopPath = "icon_workshop";
    public static class Assets
    {
        public static class Images
        {
            public static Asset<Texture2D> Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Path);
            public static Asset<Texture2D> ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Path, AssetRequestMode.ImmediateLoad);
            public static string Path = "Assets/Images/255";
            public static Asset<Texture2D> BankAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BankPath);
            public static Asset<Texture2D> BankImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BankPath, AssetRequestMode.ImmediateLoad);
            public static string BankPath = "Assets/Images/Bank";
            public static Asset<Texture2D> CloseAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ClosePath);
            public static Asset<Texture2D> CloseImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ClosePath, AssetRequestMode.ImmediateLoad);
            public static string ClosePath = "Assets/Images/Close";
            public static Asset<Texture2D> EnchantedAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(EnchantedPath);
            public static Asset<Texture2D> EnchantedImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(EnchantedPath, AssetRequestMode.ImmediateLoad);
            public static string EnchantedPath = "Assets/Images/Enchanted";
            public static Asset<Texture2D> prisonAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPath);
            public static Asset<Texture2D> prisonImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPath, AssetRequestMode.ImmediateLoad);
            public static string prisonPath = "Assets/Images/prison";
            public static Asset<Texture2D> prison2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prison2Path);
            public static Asset<Texture2D> prison2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prison2Path, AssetRequestMode.ImmediateLoad);
            public static string prison2Path = "Assets/Images/prison2";
            public static Asset<Texture2D> prison3Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prison3Path);
            public static Asset<Texture2D> prison3ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prison3Path, AssetRequestMode.ImmediateLoad);
            public static string prison3Path = "Assets/Images/prison3";
            public static Asset<Texture2D> prisonPreViewAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreViewPath);
            public static Asset<Texture2D> prisonPreViewImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreViewPath, AssetRequestMode.ImmediateLoad);
            public static string prisonPreViewPath = "Assets/Images/prisonPreView";
            public static Asset<Texture2D> prisonPreView2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreView2Path);
            public static Asset<Texture2D> prisonPreView2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreView2Path, AssetRequestMode.ImmediateLoad);
            public static string prisonPreView2Path = "Assets/Images/prisonPreView2";
            public static Asset<Texture2D> prisonPreView3Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreView3Path);
            public static Asset<Texture2D> prisonPreView3ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(prisonPreView3Path, AssetRequestMode.ImmediateLoad);
            public static string prisonPreView3Path = "Assets/Images/prisonPreView3";
            public static Asset<Texture2D> Shader_1Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_1Path);
            public static Asset<Texture2D> Shader_1ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_1Path, AssetRequestMode.ImmediateLoad);
            public static string Shader_1Path = "Assets/Images/Shader_1";
            public static Asset<Texture2D> Shader_2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_2Path);
            public static Asset<Texture2D> Shader_2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_2Path, AssetRequestMode.ImmediateLoad);
            public static string Shader_2Path = "Assets/Images/Shader_2";
            public static Asset<Texture2D> Shader_3Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_3Path);
            public static Asset<Texture2D> Shader_3ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Shader_3Path, AssetRequestMode.ImmediateLoad);
            public static string Shader_3Path = "Assets/Images/Shader_3";
            public static class GIFs
            {
                public static Asset<Texture2D> ExplodePlaceAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExplodePlacePath);
                public static Asset<Texture2D> ExplodePlaceImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExplodePlacePath, AssetRequestMode.ImmediateLoad);
                public static string ExplodePlacePath = "Assets/Images/GIFs/ExplodePlace";
                public static Asset<Texture2D> PlaceStructureAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlaceStructurePath);
                public static Asset<Texture2D> PlaceStructureImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlaceStructurePath, AssetRequestMode.ImmediateLoad);
                public static string PlaceStructurePath = "Assets/Images/GIFs/PlaceStructure";
                public static Asset<Texture2D> SaveStructureEnAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SaveStructureEnPath);
                public static Asset<Texture2D> SaveStructureEnImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SaveStructureEnPath, AssetRequestMode.ImmediateLoad);
                public static string SaveStructureEnPath = "Assets/Images/GIFs/SaveStructureEn";
                public static Asset<Texture2D> SaveStructureZhAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SaveStructureZhPath);
                public static Asset<Texture2D> SaveStructureZhImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SaveStructureZhPath, AssetRequestMode.ImmediateLoad);
                public static string SaveStructureZhPath = "Assets/Images/GIFs/SaveStructureZh";
            }

            public static class UI
            {
                public static Asset<Texture2D> BannerAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BannerPath);
                public static Asset<Texture2D> BannerImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BannerPath, AssetRequestMode.ImmediateLoad);
                public static string BannerPath = "Assets/Images/UI/Banner";
                public static Asset<Texture2D> Buff_HoverBorderAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Buff_HoverBorderPath);
                public static Asset<Texture2D> Buff_HoverBorderImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Buff_HoverBorderPath, AssetRequestMode.ImmediateLoad);
                public static string Buff_HoverBorderPath = "Assets/Images/UI/Buff_HoverBorder";
                public static Asset<Texture2D> Button_CloseAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_ClosePath);
                public static Asset<Texture2D> Button_CloseImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_ClosePath, AssetRequestMode.ImmediateLoad);
                public static string Button_ClosePath = "Assets/Images/UI/Button_Close";
                public static Asset<Texture2D> Button_DepostAllAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_DepostAllPath);
                public static Asset<Texture2D> Button_DepostAllImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_DepostAllPath, AssetRequestMode.ImmediateLoad);
                public static string Button_DepostAllPath = "Assets/Images/UI/Button_DepostAll";
                public static Asset<Texture2D> Button_DepostAll_HoverAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_DepostAll_HoverPath);
                public static Asset<Texture2D> Button_DepostAll_HoverImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_DepostAll_HoverPath, AssetRequestMode.ImmediateLoad);
                public static string Button_DepostAll_HoverPath = "Assets/Images/UI/Button_DepostAll_Hover";
                public static Asset<Texture2D> Button_LootAllAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_LootAllPath);
                public static Asset<Texture2D> Button_LootAllImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_LootAllPath, AssetRequestMode.ImmediateLoad);
                public static string Button_LootAllPath = "Assets/Images/UI/Button_LootAll";
                public static Asset<Texture2D> Button_LootAll_HoverAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_LootAll_HoverPath);
                public static Asset<Texture2D> Button_LootAll_HoverImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Button_LootAll_HoverPath, AssetRequestMode.ImmediateLoad);
                public static string Button_LootAll_HoverPath = "Assets/Images/UI/Button_LootAll_Hover";
                public static Asset<Texture2D> PotionAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PotionPath);
                public static Asset<Texture2D> PotionImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PotionPath, AssetRequestMode.ImmediateLoad);
                public static string PotionPath = "Assets/Images/UI/Potion";
                public static Asset<Texture2D> PutAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PutPath);
                public static Asset<Texture2D> PutImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PutPath, AssetRequestMode.ImmediateLoad);
                public static string PutPath = "Assets/Images/UI/Put";
                public static Asset<Texture2D> QuickAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(QuickPath);
                public static Asset<Texture2D> QuickImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(QuickPath, AssetRequestMode.ImmediateLoad);
                public static string QuickPath = "Assets/Images/UI/Quick";
                public static Asset<Texture2D> ReplenishAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ReplenishPath);
                public static Asset<Texture2D> ReplenishImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ReplenishPath, AssetRequestMode.ImmediateLoad);
                public static string ReplenishPath = "Assets/Images/UI/Replenish";
                public static Asset<Texture2D> ResizeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ResizePath);
                public static Asset<Texture2D> ResizeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ResizePath, AssetRequestMode.ImmediateLoad);
                public static string ResizePath = "Assets/Images/UI/Resize";
                public static class Architecture
                {
                    public static Asset<Texture2D> BedAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BedPath);
                    public static Asset<Texture2D> BedImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BedPath, AssetRequestMode.ImmediateLoad);
                    public static string BedPath = "Assets/Images/UI/Architecture/Bed";
                    public static Asset<Texture2D> BlockAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BlockPath);
                    public static Asset<Texture2D> BlockImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BlockPath, AssetRequestMode.ImmediateLoad);
                    public static string BlockPath = "Assets/Images/UI/Architecture/Block";
                    public static Asset<Texture2D> ChairAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ChairPath);
                    public static Asset<Texture2D> ChairImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ChairPath, AssetRequestMode.ImmediateLoad);
                    public static string ChairPath = "Assets/Images/UI/Architecture/Chair";
                    public static Asset<Texture2D> PlatformAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlatformPath);
                    public static Asset<Texture2D> PlatformImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlatformPath, AssetRequestMode.ImmediateLoad);
                    public static string PlatformPath = "Assets/Images/UI/Architecture/Platform";
                    public static Asset<Texture2D> TorchAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TorchPath);
                    public static Asset<Texture2D> TorchImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TorchPath, AssetRequestMode.ImmediateLoad);
                    public static string TorchPath = "Assets/Images/UI/Architecture/Torch";
                    public static Asset<Texture2D> WallAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallPath);
                    public static Asset<Texture2D> WallImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallPath, AssetRequestMode.ImmediateLoad);
                    public static string WallPath = "Assets/Images/UI/Architecture/Wall";
                    public static Asset<Texture2D> WorkbenchAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WorkbenchPath);
                    public static Asset<Texture2D> WorkbenchImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WorkbenchPath, AssetRequestMode.ImmediateLoad);
                    public static string WorkbenchPath = "Assets/Images/UI/Architecture/Workbench";
                }

                public static class Autofisher
                {
                    public static Asset<Texture2D> SelectPoolOffAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SelectPoolOffPath);
                    public static Asset<Texture2D> SelectPoolOffImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SelectPoolOffPath, AssetRequestMode.ImmediateLoad);
                    public static string SelectPoolOffPath = "Assets/Images/UI/Autofisher/SelectPoolOff";
                    public static Asset<Texture2D> SelectPoolOnAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SelectPoolOnPath);
                    public static Asset<Texture2D> SelectPoolOnImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SelectPoolOnPath, AssetRequestMode.ImmediateLoad);
                    public static string SelectPoolOnPath = "Assets/Images/UI/Autofisher/SelectPoolOn";
                    public static Asset<Texture2D> Slot_AccessoryAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_AccessoryPath);
                    public static Asset<Texture2D> Slot_AccessoryImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_AccessoryPath, AssetRequestMode.ImmediateLoad);
                    public static string Slot_AccessoryPath = "Assets/Images/UI/Autofisher/Slot_Accessory";
                    public static Asset<Texture2D> Slot_BaitAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_BaitPath);
                    public static Asset<Texture2D> Slot_BaitImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_BaitPath, AssetRequestMode.ImmediateLoad);
                    public static string Slot_BaitPath = "Assets/Images/UI/Autofisher/Slot_Bait";
                    public static Asset<Texture2D> Slot_FishingPoleAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_FishingPolePath);
                    public static Asset<Texture2D> Slot_FishingPoleImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Slot_FishingPolePath, AssetRequestMode.ImmediateLoad);
                    public static string Slot_FishingPolePath = "Assets/Images/UI/Autofisher/Slot_FishingPole";
                }

                public static class AutoTrash
                {
                    public static Asset<Texture2D> SettingAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SettingPath);
                    public static Asset<Texture2D> SettingImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SettingPath, AssetRequestMode.ImmediateLoad);
                    public static string SettingPath = "Assets/Images/UI/AutoTrash/Setting";
                    public static Asset<Texture2D> SettingHoverAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SettingHoverPath);
                    public static Asset<Texture2D> SettingHoverImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SettingHoverPath, AssetRequestMode.ImmediateLoad);
                    public static string SettingHoverPath = "Assets/Images/UI/AutoTrash/SettingHover";
                    public static Asset<Texture2D> TrashAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TrashPath);
                    public static Asset<Texture2D> TrashImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TrashPath, AssetRequestMode.ImmediateLoad);
                    public static string TrashPath = "Assets/Images/UI/AutoTrash/Trash";
                }

                public static class Brust
                {
                    public static Asset<Texture2D> BackgroundAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BackgroundPath);
                    public static Asset<Texture2D> BackgroundImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BackgroundPath, AssetRequestMode.ImmediateLoad);
                    public static string BackgroundPath = "Assets/Images/UI/Brust/Background";
                    public static Asset<Texture2D> FixedModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FixedModePath);
                    public static Asset<Texture2D> FixedModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FixedModePath, AssetRequestMode.ImmediateLoad);
                    public static string FixedModePath = "Assets/Images/UI/Brust/FixedMode";
                    public static Asset<Texture2D> FreeModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FreeModePath);
                    public static Asset<Texture2D> FreeModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FreeModePath, AssetRequestMode.ImmediateLoad);
                    public static string FreeModePath = "Assets/Images/UI/Brust/FreeMode";
                    public static Asset<Texture2D> HoverAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HoverPath);
                    public static Asset<Texture2D> HoverImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HoverPath, AssetRequestMode.ImmediateLoad);
                    public static string HoverPath = "Assets/Images/UI/Brust/Hover";
                    public static Asset<Texture2D> TileModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TileModePath);
                    public static Asset<Texture2D> TileModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(TileModePath, AssetRequestMode.ImmediateLoad);
                    public static string TileModePath = "Assets/Images/UI/Brust/TileMode";
                    public static Asset<Texture2D> WallModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallModePath);
                    public static Asset<Texture2D> WallModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallModePath, AssetRequestMode.ImmediateLoad);
                    public static string WallModePath = "Assets/Images/UI/Brust/WallMode";
                }

                public static class Construct
                {
                    public static Asset<Texture2D> BackAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BackPath);
                    public static Asset<Texture2D> BackImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BackPath, AssetRequestMode.ImmediateLoad);
                    public static string BackPath = "Assets/Images/UI/Construct/Back";
                    public static Asset<Texture2D> CloseAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ClosePath);
                    public static Asset<Texture2D> CloseImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ClosePath, AssetRequestMode.ImmediateLoad);
                    public static string ClosePath = "Assets/Images/UI/Construct/Close";
                    public static Asset<Texture2D> ExplodeAndPlaceAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExplodeAndPlacePath);
                    public static Asset<Texture2D> ExplodeAndPlaceImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExplodeAndPlacePath, AssetRequestMode.ImmediateLoad);
                    public static string ExplodeAndPlacePath = "Assets/Images/UI/Construct/ExplodeAndPlace";
                    public static Asset<Texture2D> FolderAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FolderPath);
                    public static Asset<Texture2D> FolderImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FolderPath, AssetRequestMode.ImmediateLoad);
                    public static string FolderPath = "Assets/Images/UI/Construct/Folder";
                    public static Asset<Texture2D> LoadAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LoadPath);
                    public static Asset<Texture2D> LoadImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LoadPath, AssetRequestMode.ImmediateLoad);
                    public static string LoadPath = "Assets/Images/UI/Construct/Load";
                    public static Asset<Texture2D> PlaceOnlyAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlaceOnlyPath);
                    public static Asset<Texture2D> PlaceOnlyImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PlaceOnlyPath, AssetRequestMode.ImmediateLoad);
                    public static string PlaceOnlyPath = "Assets/Images/UI/Construct/PlaceOnly";
                    public static Asset<Texture2D> RefreshAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(RefreshPath);
                    public static Asset<Texture2D> RefreshImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(RefreshPath, AssetRequestMode.ImmediateLoad);
                    public static string RefreshPath = "Assets/Images/UI/Construct/Refresh";
                    public static Asset<Texture2D> SaveAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SavePath);
                    public static Asset<Texture2D> SaveImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SavePath, AssetRequestMode.ImmediateLoad);
                    public static string SavePath = "Assets/Images/UI/Construct/Save";
                    public static Asset<Texture2D> Tutorial_PreviewAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_PreviewPath);
                    public static Asset<Texture2D> Tutorial_PreviewImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_PreviewPath, AssetRequestMode.ImmediateLoad);
                    public static string Tutorial_PreviewPath = "Assets/Images/UI/Construct/Tutorial_Preview";
                    public static Asset<Texture2D> Tutorial_StructList_EnAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_StructList_EnPath);
                    public static Asset<Texture2D> Tutorial_StructList_EnImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_StructList_EnPath, AssetRequestMode.ImmediateLoad);
                    public static string Tutorial_StructList_EnPath = "Assets/Images/UI/Construct/Tutorial_StructList_En";
                    public static Asset<Texture2D> Tutorial_StructList_ZhAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_StructList_ZhPath);
                    public static Asset<Texture2D> Tutorial_StructList_ZhImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Tutorial_StructList_ZhPath, AssetRequestMode.ImmediateLoad);
                    public static string Tutorial_StructList_ZhPath = "Assets/Images/UI/Construct/Tutorial_StructList_Zh";
                }

                public static class ExtremeStorage
                {
                    public static Asset<Texture2D> IconsAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(IconsPath);
                    public static Asset<Texture2D> IconsImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(IconsPath, AssetRequestMode.ImmediateLoad);
                    public static string IconsPath = "Assets/Images/UI/ExtremeStorage/Icons";
                    public static Asset<Texture2D> ToolIconsAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ToolIconsPath);
                    public static Asset<Texture2D> ToolIconsImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ToolIconsPath, AssetRequestMode.ImmediateLoad);
                    public static string ToolIconsPath = "Assets/Images/UI/ExtremeStorage/ToolIcons";
                }

                public static class LiquidSlot
                {
                    public static Asset<Texture2D> BorderAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BorderPath);
                    public static Asset<Texture2D> BorderImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BorderPath, AssetRequestMode.ImmediateLoad);
                    public static string BorderPath = "Assets/Images/UI/LiquidSlot/Border";
                    public static Asset<Texture2D> HighlightAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HighlightPath);
                    public static Asset<Texture2D> HighlightImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HighlightPath, AssetRequestMode.ImmediateLoad);
                    public static string HighlightPath = "Assets/Images/UI/LiquidSlot/Highlight";
                }

                public static class PlayerInfo
                {
                    public static Asset<Texture2D> AggroAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AggroPath);
                    public static Asset<Texture2D> AggroImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AggroPath, AssetRequestMode.ImmediateLoad);
                    public static string AggroPath = "Assets/Images/UI/PlayerInfo/Aggro";
                    public static Asset<Texture2D> ArmorPenetrationAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ArmorPenetrationPath);
                    public static Asset<Texture2D> ArmorPenetrationImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ArmorPenetrationPath, AssetRequestMode.ImmediateLoad);
                    public static string ArmorPenetrationPath = "Assets/Images/UI/PlayerInfo/ArmorPenetration";
                    public static Asset<Texture2D> EnduranceAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(EndurancePath);
                    public static Asset<Texture2D> EnduranceImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(EndurancePath, AssetRequestMode.ImmediateLoad);
                    public static string EndurancePath = "Assets/Images/UI/PlayerInfo/Endurance";
                    public static Asset<Texture2D> Endurance2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Endurance2Path);
                    public static Asset<Texture2D> Endurance2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Endurance2Path, AssetRequestMode.ImmediateLoad);
                    public static string Endurance2Path = "Assets/Images/UI/PlayerInfo/Endurance2";
                    public static Asset<Texture2D> Endurance3Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Endurance3Path);
                    public static Asset<Texture2D> Endurance3ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Endurance3Path, AssetRequestMode.ImmediateLoad);
                    public static string Endurance3Path = "Assets/Images/UI/PlayerInfo/Endurance3";
                    public static Asset<Texture2D> FishingSkillAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FishingSkillPath);
                    public static Asset<Texture2D> FishingSkillImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FishingSkillPath, AssetRequestMode.ImmediateLoad);
                    public static string FishingSkillPath = "Assets/Images/UI/PlayerInfo/FishingSkill";
                    public static Asset<Texture2D> FlyingAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FlyingPath);
                    public static Asset<Texture2D> FlyingImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FlyingPath, AssetRequestMode.ImmediateLoad);
                    public static string FlyingPath = "Assets/Images/UI/PlayerInfo/Flying";
                    public static Asset<Texture2D> LifeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LifePath);
                    public static Asset<Texture2D> LifeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LifePath, AssetRequestMode.ImmediateLoad);
                    public static string LifePath = "Assets/Images/UI/PlayerInfo/Life";
                    public static Asset<Texture2D> LuckAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LuckPath);
                    public static Asset<Texture2D> LuckImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LuckPath, AssetRequestMode.ImmediateLoad);
                    public static string LuckPath = "Assets/Images/UI/PlayerInfo/Luck";
                    public static Asset<Texture2D> MagicAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MagicPath);
                    public static Asset<Texture2D> MagicImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MagicPath, AssetRequestMode.ImmediateLoad);
                    public static string MagicPath = "Assets/Images/UI/PlayerInfo/Magic";
                    public static Asset<Texture2D> ManaRegenAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ManaRegenPath);
                    public static Asset<Texture2D> ManaRegenImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ManaRegenPath, AssetRequestMode.ImmediateLoad);
                    public static string ManaRegenPath = "Assets/Images/UI/PlayerInfo/ManaRegen";
                    public static Asset<Texture2D> MeleeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MeleePath);
                    public static Asset<Texture2D> MeleeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MeleePath, AssetRequestMode.ImmediateLoad);
                    public static string MeleePath = "Assets/Images/UI/PlayerInfo/Melee";
                    public static Asset<Texture2D> Melee2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Melee2Path);
                    public static Asset<Texture2D> Melee2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Melee2Path, AssetRequestMode.ImmediateLoad);
                    public static string Melee2Path = "Assets/Images/UI/PlayerInfo/Melee2";
                    public static Asset<Texture2D> OpenAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(OpenPath);
                    public static Asset<Texture2D> OpenImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(OpenPath, AssetRequestMode.ImmediateLoad);
                    public static string OpenPath = "Assets/Images/UI/PlayerInfo/Open";
                    public static Asset<Texture2D> RangedAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(RangedPath);
                    public static Asset<Texture2D> RangedImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(RangedPath, AssetRequestMode.ImmediateLoad);
                    public static string RangedPath = "Assets/Images/UI/PlayerInfo/Ranged";
                    public static Asset<Texture2D> Ranged2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Ranged2Path);
                    public static Asset<Texture2D> Ranged2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Ranged2Path, AssetRequestMode.ImmediateLoad);
                    public static string Ranged2Path = "Assets/Images/UI/PlayerInfo/Ranged2";
                    public static Asset<Texture2D> SummonAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SummonPath);
                    public static Asset<Texture2D> SummonImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SummonPath, AssetRequestMode.ImmediateLoad);
                    public static string SummonPath = "Assets/Images/UI/PlayerInfo/Summon";
                }

                public static class SpaceWand
                {
                    public static Asset<Texture2D> FreeModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FreeModePath);
                    public static Asset<Texture2D> FreeModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(FreeModePath, AssetRequestMode.ImmediateLoad);
                    public static string FreeModePath = "Assets/Images/UI/SpaceWand/FreeMode";
                    public static Asset<Texture2D> HVModeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HVModePath);
                    public static Asset<Texture2D> HVModeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(HVModePath, AssetRequestMode.ImmediateLoad);
                    public static string HVModePath = "Assets/Images/UI/SpaceWand/HVMode";
                    public static Asset<Texture2D> platformAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(platformPath);
                    public static Asset<Texture2D> platformImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(platformPath, AssetRequestMode.ImmediateLoad);
                    public static string platformPath = "Assets/Images/UI/SpaceWand/platform";
                    public static Asset<Texture2D> railAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(railPath);
                    public static Asset<Texture2D> railImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(railPath, AssetRequestMode.ImmediateLoad);
                    public static string railPath = "Assets/Images/UI/SpaceWand/rail";
                    public static Asset<Texture2D> ropeAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ropePath);
                    public static Asset<Texture2D> ropeImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ropePath, AssetRequestMode.ImmediateLoad);
                    public static string ropePath = "Assets/Images/UI/SpaceWand/rope";
                    public static Asset<Texture2D> soildAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(soildPath);
                    public static Asset<Texture2D> soildImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(soildPath, AssetRequestMode.ImmediateLoad);
                    public static string soildPath = "Assets/Images/UI/SpaceWand/soild";
                }

            }

        }

    }

    public static class Content
    {
        public static class Items
        {
            public static Asset<Texture2D> BannerChestAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BannerChestPath);
            public static Asset<Texture2D> BannerChestImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(BannerChestPath, AssetRequestMode.ImmediateLoad);
            public static string BannerChestPath = "Content/Items/BannerChest";
            public static Asset<Texture2D> ConstructWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ConstructWandPath);
            public static Asset<Texture2D> ConstructWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ConstructWandPath, AssetRequestMode.ImmediateLoad);
            public static string ConstructWandPath = "Content/Items/ConstructWand";
            public static Asset<Texture2D> CreateWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(CreateWandPath);
            public static Asset<Texture2D> CreateWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(CreateWandPath, AssetRequestMode.ImmediateLoad);
            public static string CreateWandPath = "Content/Items/CreateWand";
            public static Asset<Texture2D> DummyAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(DummyPath);
            public static Asset<Texture2D> DummyImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(DummyPath, AssetRequestMode.ImmediateLoad);
            public static string DummyPath = "Content/Items/Dummy";
            public static Asset<Texture2D> LiquidWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LiquidWandPath);
            public static Asset<Texture2D> LiquidWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(LiquidWandPath, AssetRequestMode.ImmediateLoad);
            public static string LiquidWandPath = "Content/Items/LiquidWand";
            public static Asset<Texture2D> MagickWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MagickWandPath);
            public static Asset<Texture2D> MagickWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MagickWandPath, AssetRequestMode.ImmediateLoad);
            public static string MagickWandPath = "Content/Items/MagickWand";
            public static Asset<Texture2D> MoveChestAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MoveChestPath);
            public static Asset<Texture2D> MoveChestImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(MoveChestPath, AssetRequestMode.ImmediateLoad);
            public static string MoveChestPath = "Content/Items/MoveChest";
            public static Asset<Texture2D> PaintWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PaintWandPath);
            public static Asset<Texture2D> PaintWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PaintWandPath, AssetRequestMode.ImmediateLoad);
            public static string PaintWandPath = "Content/Items/PaintWand";
            public static Asset<Texture2D> PotionBagAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PotionBagPath);
            public static Asset<Texture2D> PotionBagImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(PotionBagPath, AssetRequestMode.ImmediateLoad);
            public static string PotionBagPath = "Content/Items/PotionBag";
            public static Asset<Texture2D> SpaceWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SpaceWandPath);
            public static Asset<Texture2D> SpaceWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(SpaceWandPath, AssetRequestMode.ImmediateLoad);
            public static string SpaceWandPath = "Content/Items/SpaceWand";
            public static Asset<Texture2D> StarburstWandAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(StarburstWandPath);
            public static Asset<Texture2D> StarburstWandImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(StarburstWandPath, AssetRequestMode.ImmediateLoad);
            public static string StarburstWandPath = "Content/Items/StarburstWand";
            public static Asset<Texture2D> WallPlaceAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallPlacePath);
            public static Asset<Texture2D> WallPlaceImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallPlacePath, AssetRequestMode.ImmediateLoad);
            public static string WallPlacePath = "Content/Items/WallPlace";
            public static class Coin
            {
                public static Asset<Texture2D> CoinOneAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(CoinOnePath);
                public static Asset<Texture2D> CoinOneImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(CoinOnePath, AssetRequestMode.ImmediateLoad);
                public static string CoinOnePath = "Content/Items/Coin/CoinOne";
            }

            public static class Placeable
            {
                public static Asset<Texture2D> AlchemyCollectorAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AlchemyCollectorPath);
                public static Asset<Texture2D> AlchemyCollectorImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AlchemyCollectorPath, AssetRequestMode.ImmediateLoad);
                public static string AlchemyCollectorPath = "Content/Items/Placeable/AlchemyCollector";
                public static Asset<Texture2D> AutofisherAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AutofisherPath);
                public static Asset<Texture2D> AutofisherImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AutofisherPath, AssetRequestMode.ImmediateLoad);
                public static string AutofisherPath = "Content/Items/Placeable/Autofisher";
                public static Asset<Texture2D> ExtremeStorageAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStoragePath);
                public static Asset<Texture2D> ExtremeStorageImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStoragePath, AssetRequestMode.ImmediateLoad);
                public static string ExtremeStoragePath = "Content/Items/Placeable/ExtremeStorage";
            }

        }

        public static class NPCs
        {
            public static Asset<Texture2D> DummyNPCAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(DummyNPCPath);
            public static Asset<Texture2D> DummyNPCImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(DummyNPCPath, AssetRequestMode.ImmediateLoad);
            public static string DummyNPCPath = "Content/NPCs/DummyNPC";
        }

        public static class Projectiles
        {
            public static Asset<Texture2D> KillProjAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(KillProjPath);
            public static Asset<Texture2D> KillProjImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(KillProjPath, AssetRequestMode.ImmediateLoad);
            public static string KillProjPath = "Content/Projectiles/KillProj";
            public static Asset<Texture2D> WallRobotAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobotPath);
            public static Asset<Texture2D> WallRobotImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobotPath, AssetRequestMode.ImmediateLoad);
            public static string WallRobotPath = "Content/Projectiles/WallRobot";
            public static Asset<Texture2D> WallRobot2Asset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobot2Path);
            public static Asset<Texture2D> WallRobot2ImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobot2Path, AssetRequestMode.ImmediateLoad);
            public static string WallRobot2Path = "Content/Projectiles/WallRobot2";
            public static Asset<Texture2D> WallRobot_GlowAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobot_GlowPath);
            public static Asset<Texture2D> WallRobot_GlowImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(WallRobot_GlowPath, AssetRequestMode.ImmediateLoad);
            public static string WallRobot_GlowPath = "Content/Projectiles/WallRobot_Glow";
        }

        public static class Tiles
        {
            public static Asset<Texture2D> AlchemyCollectorAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AlchemyCollectorPath);
            public static Asset<Texture2D> AlchemyCollectorImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AlchemyCollectorPath, AssetRequestMode.ImmediateLoad);
            public static string AlchemyCollectorPath = "Content/Tiles/AlchemyCollector";
            public static Asset<Texture2D> AutofisherAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AutofisherPath);
            public static Asset<Texture2D> AutofisherImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(AutofisherPath, AssetRequestMode.ImmediateLoad);
            public static string AutofisherPath = "Content/Tiles/Autofisher";
            public static Asset<Texture2D> Autofisher_HighlightAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Autofisher_HighlightPath);
            public static Asset<Texture2D> Autofisher_HighlightImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(Autofisher_HighlightPath, AssetRequestMode.ImmediateLoad);
            public static string Autofisher_HighlightPath = "Content/Tiles/Autofisher_Highlight";
            public static Asset<Texture2D> ExtremeStorageAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStoragePath);
            public static Asset<Texture2D> ExtremeStorageImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStoragePath, AssetRequestMode.ImmediateLoad);
            public static string ExtremeStoragePath = "Content/Tiles/ExtremeStorage";
            public static Asset<Texture2D> ExtremeStorage_BloomAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_BloomPath);
            public static Asset<Texture2D> ExtremeStorage_BloomImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_BloomPath, AssetRequestMode.ImmediateLoad);
            public static string ExtremeStorage_BloomPath = "Content/Tiles/ExtremeStorage_Bloom";
            public static Asset<Texture2D> ExtremeStorage_GlowAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_GlowPath);
            public static Asset<Texture2D> ExtremeStorage_GlowImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_GlowPath, AssetRequestMode.ImmediateLoad);
            public static string ExtremeStorage_GlowPath = "Content/Tiles/ExtremeStorage_Glow";
            public static Asset<Texture2D> ExtremeStorage_HighlightAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_HighlightPath);
            public static Asset<Texture2D> ExtremeStorage_HighlightImmediateAsset => ModAssets_Utils.Mod.Assets.Request<Texture2D>(ExtremeStorage_HighlightPath, AssetRequestMode.ImmediateLoad);
            public static string ExtremeStorage_HighlightPath = "Content/Tiles/ExtremeStorage_Highlight";
        }

    }

}

