using ImproveGame.Common.ConstructCore;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.UIElements_Shader;
using System.Diagnostics;
using System.Reflection;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class StructureGUI : UIState
    {
        public static bool Visible { get; private set; }
        public bool CacheSetupStructures; // 缓存，在下一帧Setup
        public bool CacheSetupStructureInfos; // 缓存，在下一帧Setup
        public string CacheStructureInfoPath;

        private int oldScreenWidth;
        private int oldScreenHeight;

        public Asset<Texture2D> SaveTexture;
        public Asset<Texture2D> LoadTexture;
        public Asset<Texture2D> RefreshTexture;
        public Asset<Texture2D> BackTexture;
        public Asset<Texture2D> ButtonPanel;
        public Asset<Texture2D> ButtonPanel_Highlight;

        public UserInterface UserInterface;

        private SUIPanel BasePanel; // 背景板
        public ModScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表
        public ModImageButton RefreshButton; // 刷新/回退按钮

        public override void OnInitialize()
        {
            SaveTexture = GetTexture("UI/Construct/Save");
            LoadTexture = GetTexture("UI/Construct/Load");
            RefreshTexture = GetTexture("UI/Construct/Refresh");
            BackTexture = GetTexture("UI/Construct/Back");
            ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
            ButtonPanel_Highlight = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

            BasePanel = new(new(29, 34, 70), new(44, 57, 105, 160))
            {
                Top = StyleDimension.FromPixels(150f),
                HAlign = 0.5f
            };
            BasePanel.SetSize(600f, 0f, precentHeight: 0.6f).SetPadding(12f);
            Append(BasePanel);

            UIList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
            };
            UIList.SetPadding(2f);
            UIList.ManualSortMethod = (list) => { }; // 阻止他自动排序
            BasePanel.Append(UIList);

            Scrollbar = new(UserInterface);
            Scrollbar.Left.Set(310f, 0.5f);
            Scrollbar.Top.Set(154f, 0f);
            Scrollbar.Height.Set(-8f, 0.6f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            Append(Scrollbar);

            RefreshButton = QuickButton(RefreshTexture);
            RefreshButton.SetPos(new(-296f, 100f), 0.5f, 0f);
            RefreshButton.OnMouseDown += (_, _) =>
            {
                FileOperator.CachedStructureDatas.Clear();
                SetupStructuresList();
            };
            Append(RefreshButton);

            var folderButton = QuickButton(GetTexture("UI/Construct/Folder"));
            folderButton.SetPos(new(-246f, 100f), 0.5f, 0f);
            folderButton.OnMouseDown += (_, _) => TrUtils.OpenFolder(FileOperator.SavePath);
            Append(folderButton);

            var modeButton = QuickButton(SaveTexture);
            modeButton.SetPos(new(-196f, 100f), 0.5f, 0f);
            modeButton.OnMouseDown += (_, _) =>
            {
                if (WandSystem.ConstructMode == WandSystem.Construct.Place)
                    WandSystem.ConstructMode = WandSystem.Construct.Save;
                else
                    WandSystem.ConstructMode = WandSystem.Construct.Place;
            };
            modeButton.OnUpdate += (_) =>
            {
                if (WandSystem.ConstructMode == WandSystem.Construct.Place)
                    modeButton.SetImage(LoadTexture);
                else
                    modeButton.SetImage(SaveTexture);
            };
            Append(modeButton);

            var closeButton = QuickButton(GetTexture("UI/Construct/Close"));
            closeButton.SetPos(new(246f, 100f), 0.5f, 0f);
            closeButton.OnMouseDown += (_, _) => Close();
            Append(closeButton);
        }

        private ModImageButton QuickButton(Asset<Texture2D> texture)
        {
            var button = new ModImageButton(texture, Color.White, Color.White);
            button.SetBackgroundImage(ButtonPanel);
            button.SetHoverImage(ButtonPanel_Highlight);
            button.SetSize(44, 44);
            button.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
            return button;
        }

        private void SetupScrollBar(bool resetViewPosition = true)
        {
            float height = UIList.GetInnerDimensions().Height;
            float totalHeight = UIList.GetTotalHeight();
            Scrollbar.SetView(height, totalHeight);
            if (resetViewPosition)
                Scrollbar.ViewPosition = 0f;

            Scrollbar.Visible = true;
            if (height >= totalHeight)
            {
                Scrollbar.Visible = false;
            }
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (BasePanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
                Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var innerList = UIList.GetType().GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(UIList) as UIElement;
            if (Scrollbar is not null && innerList is not null)
            {
                innerList.Top.Set(-Scrollbar.GetValue(), 0);
            }
            UIList.Recalculate();

            base.DrawSelf(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (CacheSetupStructures)
            {
                SetupStructuresList();
                CacheSetupStructures = false;
            }
            if (CacheSetupStructureInfos)
            {
                SetupCurrentStructureList();
                CacheSetupStructureInfos = false;
            }

            Recalculate();

            base.Update(gameTime);

            if (BasePanel.IsMouseHovering)
            {
                if (Scrollbar.Visible)
                {
                    PlayerInput.LockVanillaMouseScroll("ImproveGame: Structure GUI");
                }
                Main.LocalPlayer.mouseInterface = true;
            }

            if (oldScreenHeight != Main.screenHeight)
            {
                SetupScrollBar(false);
            }

            oldScreenHeight = Main.screenHeight;
        }

        // 当前结构的信息
        public void SetupCurrentStructureList()
        {
            if (string.IsNullOrEmpty(CacheStructureInfoPath) || !File.Exists(CacheStructureInfoPath))
            {
                SetupStructuresList();
                return;
            }

            UIList.Clear();

            var structure = new QoLStructure(CacheStructureInfoPath);

            if (structure.Tag is null)
            {
                SetupStructuresList();
                return;
            }

            static UIText QuickUIText(string text, float originY) => new(text, 0.6f, true)
            {
                Height = StyleDimension.FromPixels(50f),
                Width = StyleDimension.FromPercent(1f),
                TextOriginX = 0.5f,
                TextOriginY = originY
            };
            static UIText QuickSmallUIText(string text) => new(text)
            {
                Height = StyleDimension.FromPixels(24f),
                Width = StyleDimension.FromPercent(1f),
                TextOriginX = 0.5f,
                TextOriginY = 0f
            };

            UIList.Add(QuickUIText("文件信息", 0.5f));
            string name = CacheStructureInfoPath.Split('\\').Last();
            name = name[..^FileOperator.Extension.Length];
            UIList.Add(QuickSmallUIText($"文件名: {name}"));
            UIList.Add(QuickSmallUIText($"保存时间: {DateTime.Parse(structure.BuildTime)}"));
            UIList.Add(QuickSmallUIText($"版本号: v{structure.ModVersion}"));
            UIList.Add(QuickSmallUIText($"结构尺寸: {structure.Width + 1}x{structure.Height + 1}"));

            var materialsAndStacks = MaterialCore.CountMaterials(structure);
            if (materialsAndStacks.Count > 0)
            {
                UIList.Add(QuickUIText("材料明细", 0.8f));

                var sortedResult = from pair in materialsAndStacks orderby pair.Key ascending select pair; // 排序
                foreach ((int itemType, int stack) in sortedResult)
                {
                    UIList.Add(new MaterialInfoElement(itemType, stack));
                }
            }

            UIList.Add(QuickUIText("结构预览", 0.85f));

            var viewPanel = new StructurePreviewPanel(CacheStructureInfoPath);
            viewPanel.OnResetHeight += (_) => SetupScrollBar(false);
            UIList.Add(viewPanel);

            RefreshButton.SetImage(BackTexture);

            Recalculate();
            SetupScrollBar();
        }

        public void SetupStructuresList()
        {
            UIList.Clear();

            var filePaths = Directory.GetFiles(FileOperator.SavePath);
            foreach (string path in filePaths)
            {
                if (Path.GetExtension(path) == FileOperator.Extension)
                {
                    UIList.Add(new StructurePanel(path));
                }
            }

            RefreshButton.SetImage(RefreshTexture);

            Recalculate();
            SetupScrollBar();
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open()
        {
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SetupStructuresList();
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close()
        {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
