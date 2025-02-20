using ImproveGame.Content.NPCs.Dummy;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using System.Reflection;
using Terraria.GameContent.UI.Elements;

namespace ImproveGame.UI
{
    [AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Dummy Configuration GUI")]
    public class DummyConfigurationUI : BaseBody
    {
        public static DummyConfigurationUI Instance { get; private set; }

        public DummyConfigurationUI() => Instance = this;

        public override bool IsNotSelectable => StartTimer.AnyClose;

        public override bool Enabled
        {
            get => StartTimer.Closing || _enabled;
            set => _enabled = value;
        }

        private bool _enabled;

        public override bool CanSetFocusTarget(UIElement target)
            => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;

        /// <summary>
        /// 启动关闭动画计时器
        /// </summary>
        public AnimationTimer StartTimer = new(3);

        // 主面板
        public SUIPanel MainPanel;

        // 标题面板
        private View TitlePanel;

        public SUIScrollBar Scrollbar;
        public UIText TipText;

        public SUIDropdownListContainer DropdownList { get; set; }

        public SUINumericText customAIStyleBox;

        public SUIImageButton aiStyleWikiOpener;

        public override void OnInitialize()
        {
            DropdownList = new SUIDropdownListContainer();
            DropdownList.JoinParent(this);


            // 主面板
            MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
            {
                Shaded = true,
                Draggable = true,
                IsAdaptiveHeight = true
            };
            MainPanel.SetPadding(0f);
            MainPanel.SetPosPixels(410, 360)
                .SetSizePixels(404, 0)
                .JoinParent(this);

            TitlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
            TitlePanel.SetPadding(0f);
            TitlePanel.JoinParent(MainPanel);

            // 标题
            var title = new SUIText
            {
                IsLarge = true,
                UseKey = true,
                TextOrKey = "Mods.ImproveGame.UI.DummyConfiguration.Title",
                TextAlign = new Vector2(0f, 0.5f),
                TextScale = 0.5f,
                Height = StyleDimension.Fill,
                Width = StyleDimension.Fill,
                DragIgnore = true,
                Left = new StyleDimension(16f, 0f)
            };
            title.JoinParent(TitlePanel);

            var cross = new SUICross
            {
                HAlign = 1f,
                Rounded = new Vector4(0f, 10f, 0f, 0f),
                CrossSize = 20f,
                CrossRounded = 4.5f * 0.85f,
                Border = 0f,
                BorderColor = Color.Transparent,
                BgColor = Color.Transparent,
            };
            cross.CrossOffset.X = 1f;
            cross.Width.Pixels = 46f;
            cross.Height.Set(0f, 1f);
            cross.OnUpdate += _ =>
            {
                cross.BgColor = cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
            };
            cross.OnLeftMouseDown += (_, _) => Close();
            cross.JoinParent(TitlePanel);

            var bagPanel = new View
            {
                DragIgnore = true,
                RelativeMode = RelativeMode.Vertical,
                IsAdaptiveHeight = true,
            };
            bagPanel.SetPadding(6, 6, 6, 6);
            bagPanel.SetSize(0f, 640, 1f, 0f);
            bagPanel.JoinParent(MainPanel);

            customAIStyleBox = new SUINumericText
            {
                HAlign = 1f,
                Left = new(-180,0),
                Width = new(60f, 0),
                Height = new(0, 1f),
                BgColor = Color.Black * 0.4f,
                Rounded = new Vector4(14f),
                MinValue = 1,
                MaxValue = 125,
                InnerText =
                {
                    TextAlign = new Vector2(0.5f, 0.5f),
                    TextOffset = new Vector2(0f, -2f),
                    MaxCharacterCount = 10,
                    MaxLines = 1,
                    IsWrapped = false
                },
                MaxLength = 3,
                DefaultValue = 0,
                Format = "0",
                VAlign = 0.5f
            };
            customAIStyleBox.ContentsChanged += (ref string content) =>
            {

                if (int.TryParse(content, out int value))
                    DummyNPC.LocalConfig.customAIStyle = value;
            };
            customAIStyleBox.EndTakingInput += () =>
            {
                if (int.TryParse(customAIStyleBox.Text, out int value))
                {
                    DummyNPC.LocalConfig.customAIStyle = value;

                    SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                }

            };
            customAIStyleBox.OnUpdate += (elem) =>
            {

                var value = DummyNPC.LocalConfig.customAIStyle;
                if (!customAIStyleBox.IsWritingText)
                    customAIStyleBox.Value = value;
            };
            aiStyleWikiOpener = new SUIImageButton(TextureAssets.Item[ItemID.Book].Value, GetText("UI.DummyConfiguration.OpenWikiTip"),false) 
            {
                HAlign = 1f,
                Left = new(-140, 0),
                Width = new(40f, 0),
                Height = new(0, 1f),
                VAlign = 0.5f
            };
            aiStyleWikiOpener.OnLeftClick += (evt, elem) =>
            {
                Utils.OpenToURL(GetText("UI.DummyConfiguration.WikiURL"));
            };
            //void MakeSeparator()
            //{
            //    View searchArea = new()
            //    {
            //        Height = new StyleDimension(10f, 0f),
            //        Width = new StyleDimension(-16f, 1f),
            //        HAlign = 0.5f,
            //        DragIgnore = true,
            //        RelativeMode = RelativeMode.Vertical,
            //        Spacing = new Vector2(0, 6)
            //    };
            //    searchArea.JoinParent(bagPanel);
            //    searchArea.Append(new UIHorizontalSeparator
            //    {
            //        Width = StyleDimension.FromPercent(1f),
            //        Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            //    });
            //}
            var fieldInfos = typeof(DummyConfig).GetFields();
            foreach (var fInfo in fieldInfos)
            {
                if (fInfo.Name == nameof(DummyConfig.customAIStyle)) continue;
                var fPanel = new View
                {
                    DragIgnore = true,
                    RelativeMode = RelativeMode.Vertical,
                    Rounded = new Vector4(6),
                    Spacing = new Vector2(0, 8)
                };
                fPanel.SetPadding(6, 6, 6, 6);
                fPanel.SetSize(0f, 40, 1f, 0f);
                fPanel.JoinParent(bagPanel);
                fPanel.BgColor = Color.Black * .125f;
                fPanel.OnUpdate += delegate
                {
                    Color targetColor = fPanel.IsMouseHovering ? Color.White * .0625f : Color.Black * .25f;
                    fPanel.BgColor = Color.Lerp(fPanel.BgColor, targetColor, 0.1f);
                    fPanel.Border = 1;
                    fPanel.BorderColor = UIStyle.PanelBorder;
                };
                var fieldName = new SUIText
                {
                    UseKey = true,
                    TextOrKey = $"Mods.ImproveGame.UI.DummyConfiguration.fieldName.{fInfo.Name}",
                    TextAlign = new Vector2(0f, 0f),
                    TextScale = 1f,
                    Height = new StyleDimension(40, 0),
                    Width = StyleDimension.Fill,
                    DragIgnore = true,
                    Left = new StyleDimension(10f, 0f)

                };
                fieldName.JoinParent(fPanel);
                var fType = fInfo.FieldType;
                if (fType == typeof(bool))
                {
                    SUISwitch uiSwitch = new SUISwitch(
                        () => (bool)fInfo.GetValue(DummyNPC.LocalConfig),
                        flag =>
                        {
                            fInfo.SetValueDirect(__makeref(DummyNPC.LocalConfig), flag);
                            SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                        }, ""
                        )
                    {
                        HAlign = 1f,
                        Width = new StyleDimension(60, 0),
                        Height = StyleDimension.Fill
                    };
                    uiSwitch.JoinParent(fPanel);
                }
                else if (fType == typeof(int))
                {
                    if (fInfo.Name == "LifeMax")
                    {
                        SUINumericText numberBox =
                        new SUINumericText
                        {
                            HAlign = 1f,
                            Width = new(120f, 0),
                            Height = new(0, 1f),
                            BgColor = Color.Black * 0.4f,
                            Rounded = new Vector4(14f),
                            MinValue = 1,
                            MaxValue = 2147483647,
                            InnerText =
                            {
                                TextAlign = new Vector2(0.5f, 0.5f),
                                TextOffset = new Vector2(0f, -2f),
                                MaxCharacterCount = 10,
                                MaxLines = 1,
                                IsWrapped = false
                            },
                            MaxLength = 10,
                            DefaultValue = 2000000000,
                            Format = "0",
                            VAlign = 0.5f
                        };
                        numberBox.ContentsChanged += (ref string content) =>
                        {
                            if (int.TryParse(content, out int value))
                                DummyNPC.LocalConfig.LifeMax = value;
                        };
                        numberBox.EndTakingInput += () =>
                        {
                            if (int.TryParse(numberBox.Text, out int value))
                            {
                                DummyNPC.LocalConfig.LifeMax = value;
                                SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                            }

                        };
                        numberBox.OnUpdate += (elem) =>
                        {

                            var value = DummyNPC.LocalConfig.LifeMax;
                            if (!numberBox.IsWritingText)
                                numberBox.Value = value;
                        };
                        numberBox.JoinParent(fPanel);
                    }
                    else
                    {
                        SUISlider<int> sUISlider = new SUISlider<int>(
                        () => (int)fInfo.GetValue(DummyNPC.LocalConfig),
                        obj =>
                        {
                            fInfo.SetValueDirect(__makeref(DummyNPC.LocalConfig), obj);
                            SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                        }, "",
                        0, 1000, 0
                        )
                        {
                            HAlign = 1f,
                            Width = new StyleDimension(180, 0),
                            Height = StyleDimension.Fill
                        };
                        sUISlider.JoinParent(fPanel);
                    }
                }
                else if (fType == typeof(DummyConfig.AIType))
                {
                    SUIDropdownList<DummyConfig.AIType> list = new(() => (DummyConfig.AIType)fInfo.GetValue(DummyNPC.LocalConfig), obj =>
                    {
                        fInfo.SetValueDirect(__makeref(DummyNPC.LocalConfig), obj);
                        var aistyle = (DummyConfig.AIType)obj;
                        if (aistyle != DummyConfig.AIType.Default && aistyle != DummyConfig.AIType.SelfDefine)
                            DummyNPC.LocalConfig = aistyle switch
                            {
                                DummyConfig.AIType.Slime or DummyConfig.AIType.Soilder or DummyConfig.AIType.JellyFish or DummyConfig.AIType.GoldenFish or DummyConfig.AIType.HugeMimic => DummyNPC.LocalConfig with { NoTileCollide = false, NoGravity = false },
                                DummyConfig.AIType.EvilEye => DummyNPC.LocalConfig with { NoGravity = true },
                                _ => DummyNPC.LocalConfig
                            };
                        SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                        if (aistyle == DummyConfig.AIType.SelfDefine && customAIStyleBox.Parent == null)
                        {
                            customAIStyleBox.JoinParent(fPanel);
                            aiStyleWikiOpener.JoinParent(fPanel);
                        }
                        else if (aistyle != DummyConfig.AIType.SelfDefine && customAIStyleBox.Parent != null) 
                        {
                            customAIStyleBox.Remove();
                            aiStyleWikiOpener.Remove();
                        }
                    }, DropdownList, "")
                    {
                        HAlign = 1f,
                        Width = new StyleDimension(180, 0),
                        Height = StyleDimension.Fill
                    };
                    list.JoinParent(fPanel);
                    if (DummyNPC.LocalConfig.AIStyle == DummyConfig.AIType.SelfDefine && customAIStyleBox.Parent == null)
                    {
                        customAIStyleBox.JoinParent(fPanel);
                        aiStyleWikiOpener.JoinParent(fPanel);
                    }
                    else if (DummyNPC.LocalConfig.AIStyle != DummyConfig.AIType.SelfDefine && customAIStyleBox.Parent != null)
                    {
                        customAIStyleBox.Remove();
                        aiStyleWikiOpener.Remove();
                    }

                }
                else
                {
                    SUISlider<float> sUISlider = new SUISlider<float>(
                    () => (float)fInfo.GetValue(DummyNPC.LocalConfig),
                    obj =>
                    {
                        fInfo.SetValueDirect(__makeref(DummyNPC.LocalConfig), obj);
                        SyncDummyModule.Get(null, Main.myPlayer, DummyNPC.LocalConfig).Send(runLocally: true);
                    }, "",
                    0, 2, 0
                    )
                    {
                        HAlign = 1f,
                        Width = new StyleDimension(180, 0),
                        Height = StyleDimension.Fill
                    };
                    sUISlider.JoinParent(fPanel);
                }
                //MakeSeparator();

            }
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            StartTimer.Update();
            if (!MainPanel.IsMouseHovering)
                return;

            PlayerInput.LockVanillaMouseScroll("ImproveGame: Dummy Configuration GUI");
            Main.LocalPlayer.mouseInterface = true;
        }

        public void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Enabled = true;
            StartTimer.Open();

            var vec = Main.MouseScreen;
            vec /= Main.UIScale;
            float zoom = Main.GameZoomTarget * Main.ForcedMinimumZoom;
            vec = (vec - Main.ScreenSize.ToVector2() * .5f) * zoom + Main.ScreenSize.ToVector2() * .5f;

            MainPanel.SetPosPixels(vec.X, vec.Y);
            MainPanel.Recalculate();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Enabled = false;
            StartTimer.Close();
        }

        public override bool RenderTarget2DDraw => !StartTimer.Opened;
        public override float RenderTarget2DOpacity => StartTimer.Schedule;
        public override Vector2 RenderTarget2DOrigin => MainPanel.GetDimensionsCenter();
        public override Vector2 RenderTarget2DPosition => MainPanel.GetDimensionsCenter();
        public override Vector2 RenderTarget2DScale => new Vector2(0.95f + StartTimer.Lerp(0, 0.05f));
    }
}
