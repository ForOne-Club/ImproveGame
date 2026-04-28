using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions.Construction;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    private class BuildingRegisterSystem : ModSystem
    {
        public static readonly Queue<BuildingLoadData> _loadQueue = [];
        private static bool _isWaitingPreview;
        private static string _tempFilePath;
        private static BuildingData _currentWaitingData;
        private static void CreateShowcase(string directory)
        {
            var building = ModAsset.PrisonComplex.Value;
            var prison1 = ModAsset.Prison1.Value;
            var prison2 = ModAsset.Prison2.Value;
            var prison3 = ModAsset.Prison3.Value;
            using FileStream fsShowcase = new FileStream(Path.Combine(directory, "buildingShowcase.png"), FileMode.Create);
            using FileStream fs1 = new FileStream(Path.Combine(directory, "Prison1.png"), FileMode.Create);
            using FileStream fs2 = new FileStream(Path.Combine(directory, "Prison2.png"), FileMode.Create);
            using FileStream fs3 = new FileStream(Path.Combine(directory, "Prison3.png"), FileMode.Create);

            building.SaveAsPng(
                fsShowcase,
                building.Width,
                building.Height
                );
            prison1.SaveAsPng(
                fs1,
                prison1.Width,
                prison1.Height
                );
            prison2.SaveAsPng(
                fs2,
                prison2.Width,
                prison2.Height
                );
            prison3.SaveAsPng(
                fs3,
                prison3.Width,
                prison3.Height
                );
        }
        public override void Load()
        {
            if (Main.dedServ)
                return;

            _loadQueue.Clear();
            // 把读取放到主线程上
            Main.QueueMainThreadAction(() =>
            {
                string directory = Path.Combine(Main.SavePath, "Mods", "ImproveGame", "CreateWand");
                if (!Directory.Exists(directory)
                    || (Directory.GetFiles(directory) is { Length: 1 } files 
                        && Path.GetFileNameWithoutExtension(files[0]) is "buildingShowcase"))
                {
                    Directory.CreateDirectory(directory);
                    CreateShowcase(directory);
                }
                foreach (var file in Directory.GetFiles(directory, "*.png"))
                    _loadQueue.Enqueue(BuildingLoadData.FromFilePath(file));
            }
            );
        }
        private static void HandleRegisterStructurePath(string path)
        {
            var fullpath = Path.Combine(FileOperator.SavePath, path);
            QoLStructure structure = new(fullpath);
            _currentWaitingData = BuildingData.FromQotStructure(structure);
            var picturepath = Path.Combine(FileOperator.SavePath, "CreateWand", Path.GetFileNameWithoutExtension(path) + ".png");
            _currentWaitingData.SaveAsDatamap(picturepath);

            int width = _currentWaitingData.Width;
            int height = _currentWaitingData.Height;
            int length = width * height;
            Color[] colors = new Color[length];
            for (int n = 0; n < length; n++)
                colors[n] = _currentWaitingData.TileInfos[n].ToColor();
            _isWaitingPreview = true;
            var tag = CreateStructureTagFromColors(colors, width, height);
            var tagPath = Path.Combine(ModLoader.ModPath, nameof(ImproveGame), "tempStructure.qotstruct");
            TagIO.ToFile(tag, tagPath);
            WandSystem.ConstructFilePath = tagPath;
            _tempFilePath = tagPath;
            PreviewRenderer.ResetPreviewTarget = PreviewRenderer.ResetState.WaitReset;
            PreviewRenderer.PreviewTarget =
                new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    width * 16 + 4,
                    height * 16 + 4,
                    false,
                    default,
                    default,
                    default,
                    RenderTargetUsage.PreserveContents);
        }

        private static void HandleRegisterDatamapPath(string path)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open);
            using Texture2D texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, fileStream);
            HandleRegister(texture);
        }

        private static void HandleRegister(Texture2D texture)
        {
            var colors = GetColors(texture);

            _isWaitingPreview = true;

            var tag = CreateStructureTagFromColors(colors, texture.Width, texture.Height);
            var tagPath = Path.Combine(ModLoader.ModPath, nameof(ImproveGame), "tempStructure.qotstruct");
            TagIO.ToFile(tag, tagPath);
            WandSystem.ConstructFilePath = tagPath;
            _tempFilePath = tagPath;
            PreviewRenderer.ResetPreviewTarget = PreviewRenderer.ResetState.WaitReset;
            int width = texture.Width;
            int height = texture.Height;
            PreviewRenderer.PreviewTarget =
                new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    width * 16 + 4,
                    height * 16 + 4,
                    false,
                    default,
                    default,
                    default,
                    RenderTargetUsage.PreserveContents);

            _currentWaitingData = BuildingData.FromDataMap(texture);
        }

        private static void HandlePreviewRegister()
        {
            if (!_isWaitingPreview || PreviewRenderer.ResetPreviewTarget != PreviewRenderer.ResetState.Finished) return;

            _isWaitingPreview = false;
            if (!string.IsNullOrEmpty(_tempFilePath))
                File.Delete(_tempFilePath);
            FileOperator.CachedStructureDatas.Remove(_tempFilePath);
            var pvRender = PreviewRenderer.PreviewTarget;
            int width = pvRender.Width;
            int height = pvRender.Height;
            Texture2D previewTexture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            previewTexture.SetData(GetColors(pvRender));
            BuildingDataPreview_Internal.Add(_currentWaitingData, previewTexture);
            OnBuildingDataPreviewAdded?.Invoke(_currentWaitingData);
        }
        public override void PostUpdateEverything()
        {
            if (Main.dedServ) return;
            if (!_isWaitingPreview && _loadQueue.TryDequeue(out var data))
            {
                if (data.IsDataMap)
                    HandleRegister(data.DataMap);
                else if (data.IsStructureFile)
                    HandleRegisterStructurePath(data.FilePath);
                else
                    HandleRegisterDatamapPath(data.FilePath);
            }
            HandlePreviewRegister();
        }
    }
}
