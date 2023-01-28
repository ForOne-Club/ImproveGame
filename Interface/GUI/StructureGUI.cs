using ImproveGame.Common.ConstructCore;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.SUIElements;
using ImproveGame.Interface.UIElements;
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

        private int oldScreenHeight;

        public Asset<Texture2D> RefreshTexture;
        public Asset<Texture2D> BackTexture;
        public Asset<Texture2D> ButtonBackgroundTexture;

        private SUIPanel BasePanel; // 背景板
        public SUIScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表
        public ModImageButton RefreshButton; // 刷新/回退按钮

        public override void OnInitialize()
        {
            var saveTexture = GetTexture("UI/Construct/Save");
            var loadTexture = GetTexture("UI/Construct/Load");
            var explodeAndPlaceTexture = GetTexture("UI/Construct/ExplodeAndPlace");
            var placeOnlyTexture = GetTexture("UI/Construct/PlaceOnly");
            RefreshTexture = GetTexture("UI/Construct/Refresh");
            BackTexture = GetTexture("UI/Construct/Back");
            ButtonBackgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");

            BasePanel = new SUIPanel(new Color(29, 34, 70), new Color(44, 57, 105, 160))
            {
                Top = StyleDimension.FromPixels(150f),
                HAlign = 0.5f
            };
            BasePanel.SetSize(600f, 0f, precentHeight: 0.6f).SetPadding(12f);
            Append(BasePanel);

            UIList = new UIList
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
            };
            UIList.SetPadding(2f);
            UIList.ManualSortMethod = _ => { }; // 阻止他自动排序
            BasePanel.Append(UIList);

            Scrollbar = new();
            Scrollbar.Left.Set(310f, 0.5f);
            Scrollbar.Top.Set(154f, 0f);
            Scrollbar.Height.Set(-8f, 0.6f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            Append(Scrollbar);

            RefreshButton = QuickButton(RefreshTexture, "{$Mods.ImproveGame.Common.Refresh}");
            RefreshButton.SetPos(new(-296f, 100f), 0.5f, 0f);
            RefreshButton.OnMouseDown += (_, _) =>
            {
                FileOperator.CachedStructureDatas.Clear();
                SetupStructuresList();
            };
            Append(RefreshButton);

            var folderButton = QuickButton(GetTexture("UI/Construct/Folder"), "{$LegacyInterface.110}");
            folderButton.SetPos(new(-246f, 100f), 0.5f, 0f);
            folderButton.OnMouseDown += (_, _) => TrUtils.OpenFolder(FileOperator.SavePath);
            Append(folderButton);

            var modeButton = QuickButton(saveTexture, "");
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
                {
                    modeButton.SetImage(loadTexture);
                    modeButton.HoverText = "{$Mods.ImproveGame.ConstructGUI.LoadMode}";
                }
                else
                {
                    modeButton.SetImage(saveTexture);
                    modeButton.HoverText = "{$Mods.ImproveGame.ConstructGUI.SaveMode}";
                }
            };
            Append(modeButton);

            var explodeButton = QuickButton(saveTexture, "");
            explodeButton.SetPos(new(-146f, 100f), 0.5f, 0f);
            explodeButton.OnMouseDown += (_, _) =>
            {
                if (WandSystem.ExplodeMode == WandSystem.Construct.Place)
                    WandSystem.ExplodeMode = WandSystem.Construct.ExplodeAndPlace;
                else
                    WandSystem.ExplodeMode = WandSystem.Construct.Place;
            };
            explodeButton.OnUpdate += (_) =>
            {
                if (WandSystem.ExplodeMode == WandSystem.Construct.Place)
                {
                    explodeButton.SetImage(placeOnlyTexture);
                    explodeButton.HoverText = "{$Mods.ImproveGame.ConstructGUI.PlaceOnly}";
                }
                else
                {
                    explodeButton.SetImage(explodeAndPlaceTexture);
                    explodeButton.HoverText = "{$Mods.ImproveGame.ConstructGUI.ExplodeAndPlace}";
                }
            };
            Append(explodeButton);

            var closeButton = QuickButton(GetTexture("UI/Construct/Close"), "{$LegacyInterface.71}");
            closeButton.SetPos(new(246f, 100f), 0.5f, 0f);
            closeButton.OnMouseDown += (_, _) => Close();
            Append(closeButton);

            var tutorialButton = QuickButton(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Locked"), "{$Mods.ImproveGame.ConstructGUI.Tutorial.Button}");
            tutorialButton.SetPos(new(196f, 100f), 0.5f, 0f);
            tutorialButton.OnMouseDown += (_, _) => SetupTutorialPage();
            Append(tutorialButton);
        }

        private ModImageButton QuickButton(Asset<Texture2D> texture, string hoverText)
        {
            var button = new ModImageButton(texture, Color.White, Color.White);
            button.SetBackgroundImage(ButtonBackgroundTexture);
            button.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
            button.SetSize(44, 44);
            button.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
            button.HoverText = hoverText;
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
            if (BasePanel.IsMouseHovering || Scrollbar.IsMouseHovering)
                Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Scrollbar is not null)
            {
                UIList._innerList.Top.Set(-Scrollbar.ViewPosition, 0);
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

            if (BasePanel.IsMouseHovering || (Scrollbar.IsMouseHovering && Scrollbar.Visible))
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


            static UIText QuickSmallUIText(string text) => new(text)
            {
                Height = StyleDimension.FromPixels(24f),
                Width = StyleDimension.FromPercent(1f),
                TextOriginX = 0.5f,
                TextOriginY = 0f
            };

            UIList.Add(QuickTitleText(GetText("ConstructGUI.FileInfo.Title"), 0.5f));
            // 检查模组是否被加载了
            foreach ((string blockName, _) in structure.entries)
            {
                if (ModLoader.HasMod(blockName.Split('/')[0]))
                {
                    continue;
                }

                char countChar = GetText("ConstructGUI.FileInfo.ModMissing.Count")[0];
                int count = char.IsNumber(countChar) ? countChar - '0' : 2;
                for (int i = 1; i <= count; i++)
                {
                    var text = QuickSmallUIText(GetText($"ConstructGUI.FileInfo.ModMissing.{i}"));
                    text.TextColor = new(244, 208, 68);
                    UIList.Add(text);
                }
                break;
            }
            string name = CacheStructureInfoPath.Split('\\').Last();
            name = name[..^FileOperator.Extension.Length];
            UIList.Add(QuickSmallUIText(GetTextWith("ConstructGUI.FileInfo.Name", new { Name = name }))); // 文件名
            UIList.Add(QuickSmallUIText(GetTextWith("ConstructGUI.FileInfo.Time", new { Time = DateTime.Parse(structure.BuildTime) }))); // 保存时间
            UIList.Add(QuickSmallUIText(GetTextWith("ConstructGUI.FileInfo.Version", new { Version = $"v{structure.ModVersion}" }))); // 模组版本
            UIList.Add(QuickSmallUIText(GetTextWith("ConstructGUI.FileInfo.Size", new { Size = $"{structure.Width + 1}x{structure.Height + 1}" }))); // 结构尺寸

            var materialsAndStacks = MaterialCore.CountMaterials(structure);
            if (materialsAndStacks.Count > 0)
            {
                UIList.Add(QuickTitleText(GetText("ConstructGUI.MaterialInfo.Title"), 0.8f));

                var sortedResult = from pair in materialsAndStacks orderby pair.Key select pair; // 排序
                foreach ((int itemType, int stack) in from mat in sortedResult where mat.Value > 0 select mat)
                {
                    UIList.Add(new MaterialInfoElement(itemType, stack));
                }
            }

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Preview.Title"), 0.85f));

            var viewPanel = new StructurePreviewPanel(CacheStructureInfoPath);
            viewPanel.OnResetHeight += (_) => SetupScrollBar(false);
            UIList.Add(viewPanel);

            RefreshButton.SetImage(BackTexture);
            RefreshButton.HoverText = "{$UI.Back}";

            Recalculate();
            SetupScrollBar();
        }

        // 结构列表
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

            BasePanel.backgroundColor = new(44, 57, 105, 160);
            RefreshButton.SetImage(RefreshTexture);
            RefreshButton.HoverText = "{$Mods.ImproveGame.Common.Refresh}";

            Recalculate();
            SetupScrollBar();
        }

        // 显示“教程”
        public void SetupTutorialPage()
        {
            UIList.Clear();

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Tutorial.Button"), 0.5f));
            char countChar = GetText("ConstructGUI.Tutorial.AlphaTest.Count")[0];
            int count = char.IsNumber(countChar) ? countChar - '0' : 2;
            for (int i = 1; i <= count; i++)
            {
                UIList.Add(new UIText(GetText($"ConstructGUI.Tutorial.AlphaTest.{i}"))
                {
                    TextOriginX = 0.5f,
                    Width = StyleDimension.FromPercent(1f),
                    Height = StyleDimension.FromPixels(24f),
                    TextColor = new(244, 208, 68)
                });
            }
            UIList.Add(QuickSeparator());

            static UIPanel QuickTransparentPanel()
            {
                var panel = (UIPanel)new UIPanel()
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Transparent
                }.SetSize(new Vector2(600f, 6f));
                panel.SetPadding(6f);
                return panel;
            };

            static void Seperate(UIList list)
            {
                list.Add(QuickTransparentPanel()); // 创建大的空域
                list.Add(QuickSeparator());
                list.Add(QuickTransparentPanel()); // 创建大的空域
            };

            #region 保存
            var panel = QuickTransparentPanel();

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Tutorial.Save.Title"), 0.5f, 0f).SetPos(6f, 0f));
            var uiText = new UIText(GetText("ConstructGUI.Tutorial.Save.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixels(460f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            var buttonExample = new UIImage(ButtonBackgroundTexture).SetPos(490f, -10f).SetAlign(verticalAlign: 0.5f);
            buttonExample.Append(new UIImage(GetTexture("UI/Construct/Save")).SetAlign(0.5f, 0.5f));
            buttonExample.OnMouseDown += (_, _) => WandSystem.ConstructMode = WandSystem.Construct.Save;
            panel.Append(buttonExample);

            panel.Height = StyleDimension.FromPixels(uiText.MinHeight.Pixels - 4f);
            panel.Recalculate();

            UIList.Add(panel);
            int horizontalFrames = 13;
            int verticalFrames = 8;
            int totalFrames = 93;
            string gifName = "SaveStructureEn";
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                gifName = "SaveStructureZh";
            }
            UIList.Add(new GIFImage(gifName, horizontalFrames, verticalFrames, totalFrames, 2).SetAlign(0.5f));

            Seperate(UIList);
            #endregion

            #region 放置
            panel = QuickTransparentPanel();

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Tutorial.Place.Title"), 0.5f, 0f).SetPos(6f, 0f));
            uiText = new UIText(GetText("ConstructGUI.Tutorial.Place.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixels(470f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            buttonExample = new UIImage(ButtonBackgroundTexture).SetPos(490f, -10f).SetAlign(verticalAlign: 0.5f);
            buttonExample.Append(new UIImage(GetTexture("UI/Construct/Load")).SetAlign(0.5f, 0.5f));
            buttonExample.OnMouseDown += (_, _) => WandSystem.ConstructMode = WandSystem.Construct.Place;
            panel.Append(buttonExample);

            panel.Height = StyleDimension.FromPixels(uiText.MinHeight.Pixels - 4f);
            panel.Recalculate();

            UIList.Add(panel);
            UIList.Add(new GIFImage("PlaceStructure", 16, 5, 77, 2).SetAlign(0.5f));

            Seperate(UIList);
            #endregion

            #region 爆破
            panel = QuickTransparentPanel();

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Tutorial.Explode.Title"), 0.5f, 0f).SetPos(6f, 0f));
            uiText = new UIText(GetText("ConstructGUI.Tutorial.Explode.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixels(430f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            buttonExample = new UIImage(ButtonBackgroundTexture).SetPos(490f, -20f).SetAlign(verticalAlign: 0.5f);
            buttonExample.Append(new UIImage(GetTexture("UI/Construct/ExplodeAndPlace")).SetAlign(0.5f, 0.5f));
            buttonExample.OnMouseDown += (_, _) => WandSystem.ExplodeMode = WandSystem.Construct.ExplodeAndPlace;
            panel.Append(buttonExample);

            buttonExample = new UIImage(ButtonBackgroundTexture).SetPos(440f, -20f).SetAlign(verticalAlign: 0.5f);
            buttonExample.Append(new UIImage(GetTexture("UI/Construct/PlaceOnly")).SetAlign(0.5f, 0.5f));
            buttonExample.OnMouseDown += (_, _) => WandSystem.ExplodeMode = WandSystem.Construct.Place;
            panel.Append(buttonExample);

            panel.Height = StyleDimension.FromPixels(uiText.MinHeight.Pixels - 4f);
            panel.Recalculate();

            UIList.Add(panel);
            UIList.Add(new GIFImage("ExplodePlace", 15, 5, 75, 2).SetAlign(0.5f));

            Seperate(UIList);
            #endregion

            #region 列表单元
            panel = QuickTransparentPanel();

            UIList.Add(QuickTitleText(GetText("ConstructGUI.Tutorial.Panel.Title"), 0.5f, 0f).SetPos(6f, 0f));
            uiText = new UIText(GetText("ConstructGUI.Tutorial.Panel.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixels(562f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            panel.Height = StyleDimension.FromPixels(uiText.MinHeight.Pixels - 4f);
            panel.Recalculate();

            UIList.Add(panel);

            string imageName = "Tutorial_StructList_En";
            if (Language.ActiveCulture.Name == "zh-Hans")
                imageName = "Tutorial_StructList_Zh";
            var uiImage = new UIImage(GetTexture($"UI/Construct/{imageName}"));
            UIList.Add(uiImage);

            Seperate(UIList);
            #endregion

            #region 材料明细
            panel = QuickTransparentPanel();

            panel.Append(QuickTitleText(GetText("ConstructGUI.Tutorial.Materials.Title"), 0.5f, 0f).SetPos(0f, -10f));
            uiText = new UIText(GetText("ConstructGUI.Tutorial.Materials.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Top = StyleDimension.FromPixels(50f),
                Width = StyleDimension.FromPixels(260f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            var dollList = new UIList
            {
                Left = StyleDimension.FromPixels(260f),
                Width = StyleDimension.FromPixels(325f),
                Height = StyleDimension.FromPercent(1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
            };
            dollList.SetPadding(2f);
            dollList.ManualSortMethod = (list) => { }; // 阻止他自动排序
            dollList.Add(new MaterialInfoDoll(ItemID.Torch, 3));
            dollList.Add(new MaterialInfoDoll(ItemID.Wood, 45));
            dollList.Add(new MaterialInfoDoll(ItemID.WoodenChair, 3));
            dollList.Add(new MaterialInfoDoll(ItemID.WorkBench, 3));
            dollList.Add(new MaterialInfoDoll(ItemID.WoodWall, 87));
            dollList.Add(new MaterialInfoDoll(ItemID.WoodPlatform, 28));
            panel.Append(dollList);

            panel.Height = StyleDimension.FromPixels(Math.Max(uiText.MinHeight.Pixels + 50f, dollList.GetTotalHeight()) + 10f);
            panel.Recalculate();

            UIList.Add(panel);

            Seperate(UIList);
            #endregion

            #region 结构预览
            panel = QuickTransparentPanel();

            panel.Append(QuickTitleText(GetText("ConstructGUI.Tutorial.Preview.Title"), 0.5f, 0f).SetPos(0f, -10f));
            uiText = new UIText(GetText("ConstructGUI.Tutorial.Preview.Text"))
            {
                IsWrapped = true,
                TextOriginX = 0f,
                Top = StyleDimension.FromPixels(50f),
                Width = StyleDimension.FromPixels(280f)
            };
            uiText.Recalculate();
            panel.Append(uiText);

            uiImage = new UIImage(GetTexture("UI/Construct/Tutorial_Preview"));
            uiImage.SetPos(290f, 8f).SetAlign(verticalAlign: 0.5f);
            panel.Append(uiImage);

            panel.Height = StyleDimension.FromPixels(Math.Max(uiText.MinHeight.Pixels, uiImage.Height.Pixels) + 26f);
            panel.Recalculate();

            UIList.Add(panel);
            #endregion

            BasePanel.backgroundColor = new(49, 67, 125, 190);
            RefreshButton.SetImage(BackTexture);
            RefreshButton.HoverText = "{$UI.Back}";

            Recalculate();
            SetupScrollBar();
        }

        private static UIHorizontalSeparator QuickSeparator() => new()
        {
            Width = StyleDimension.FromPercent(1f),
            Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
        };

        private static UIText QuickTitleText(string text, float originY, float originX = 0.5f) => new(text, 0.6f, true)
        {
            Height = StyleDimension.FromPixels(50f),
            Width = StyleDimension.FromPercent(1f),
            TextOriginX = originX,
            TextOriginY = originY
        };

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
