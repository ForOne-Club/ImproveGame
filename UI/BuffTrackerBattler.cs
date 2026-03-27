//using ImproveGame.Common.ModPlayers;
//using ImproveGame.Packets;
//using ImproveGame.UIFramework.Common;
//using ImproveGame.UIFramework.SUIElements;
//using Terraria.GameContent.Creative;

//namespace ImproveGame.UI;

//public class BattlerPanel : SUIPanel
//{
//    public BattlerPanel() : base(Color.Black, UIStyle.TitleBg) { }

//    public string displayTextCache = string.Empty;

//    public override void Draw(SpriteBatch spriteBatch)
//    {
//        // [AI注释] 仅在玩家满足条件 Buff 时显示面板，避免无效 UI 干扰。
//        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
//        {
//            base.Draw(spriteBatch);

//            if (IsMouseHovering)
//                Main.instance.MouseTextNoOverride(displayTextCache, 0, 0);

//        }
//    }

//    public override void Update(GameTime gameTime)
//    {
//        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
//            base.Update(gameTime);
//    }
//}

//public class BuffTrackerBattler
//{
//    internal BattlerPanel MainPanel;
//    internal UIText maxRateText;
//    internal UIText minRateText;
//    internal UIText midRateText;
//    internal float _sliderCurrentValueCache;
//    private float _currentTargetValue;
//    private bool _needsToCommitChange;
//    private DateTime _nextTimeWeCanPush = DateTime.UtcNow;

//    public void Initialize()
//    {
//        MainPanel = new()
//        {
//            Width = new StyleDimension(77f, 0f),
//            Height = new StyleDimension(220f, 0f)
//        };
//        MainPanel.OnUpdate += UpdateUseMouseInterface;

//        UIVerticalSlider uIVerticalSlider =
//            CreativePowersHelper.CreateSlider(GetSliderValue, SetValueKeyboard, SetValueGamepad);
//        // [AI注释] 每帧更新悬停提示并尝试提交延迟的滑条变更。
//        uIVerticalSlider.OnUpdate += UpdateSliderAndShowMultiplierMouseOver;
//        uIVerticalSlider.EmptyColor = Color.Lime;
//        uIVerticalSlider.FilledColor = Color.CornflowerBlue;
//        MainPanel.Append(uIVerticalSlider);

//        maxRateText = new("x200")
//        {
//            HAlign = 1.6f,
//            VAlign = 0f
//        };
//        maxRateText.OnLeftClick += delegate
//        {
//            SoundEngine.PlaySound(SoundID.ResearchComplete);
//            PushChange(1.0f);
//        };
//        maxRateText.OnMouseOver += delegate
//        {
//            maxRateText.ShadowColor = Color.Yellow;
//            SoundEngine.PlaySound(SoundID.MenuTick);
//        };
//        maxRateText.OnMouseOut += delegate
//        {
//            maxRateText.ShadowColor = Color.Black;
//        };
//        MainPanel.Append(maxRateText);

//        UIText element2 = new("x1")
//        {
//            HAlign = 0.8f,
//            VAlign = 0.5f
//        };
//        MainPanel.Append(element2);
//        element2.OnLeftClick += delegate
//        {
//            SoundEngine.PlaySound(SoundID.ResearchComplete);
//            PushChange(0.5f);
//        };
//        element2.OnMouseOver += delegate
//        {
//            element2.ShadowColor = Color.Yellow;
//            SoundEngine.PlaySound(SoundID.MenuTick);
//        };
//        element2.OnMouseOut += delegate
//        {
//            element2.ShadowColor = Color.Black;
//        };
//        midRateText = element2;

//        UIText element3 = new("x0")
//        {
//            HAlign = 0.9f,
//            VAlign = 1f
//        };
//        element3.OnLeftClick += delegate
//        {
//            SoundEngine.PlaySound(SoundID.ResearchComplete);
//            PushChange(0f);
//        };
//        element3.OnMouseOver += delegate
//        {
//            element3.ShadowColor = Color.Yellow;
//            SoundEngine.PlaySound(SoundID.MenuTick);
//        };
//        element3.OnMouseOut += delegate
//        {
//            element3.ShadowColor = Color.Black;
//        };
//        MainPanel.Append(element3);
//        minRateText = element3;
//    }

//    private void AttemptPushingChange()
//    {
//        // [AI注释] 只有存在待提交值且到达节流时间后才发送，防止频繁发包。
//        if (_needsToCommitChange && DateTime.UtcNow.CompareTo(_nextTimeWeCanPush) != -1)
//            PushChange(_currentTargetValue);
//    }

//    internal void PushChange(float newSliderValue)
//    {
//        _needsToCommitChange = false;
//        _sliderCurrentValueCache = newSliderValue;
//        _nextTimeWeCanPush = DateTime.UtcNow;
//        // [AI注释] runLocally=true：本地先即时生效，再由同步逻辑广播给其他端。
//        SpawnRateSlider.Get(Main.myPlayer, _sliderCurrentValueCache).Send(runLocally: true);
//    }

//    internal void SetValueKeyboard(float value)
//    {
//        // [AI注释] 键盘调整只更新目标值，真正发包由 OnUpdate 中的节流流程处理。
//        if (value != _currentTargetValue)
//        {
//            _currentTargetValue = value;
//            _needsToCommitChange = true;
//        }
//    }

//    internal void SetValueGamepad() { } // 我寻思着，就算你真的用手柄玩Mod，咱模组也不支持啊

//    private void UpdateUseMouseInterface(UIElement affectedElement)
//    {
//        var dimensions = affectedElement.GetDimensions();
//        if (dimensions.ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
//        {
//            Main.LocalPlayer.mouseInterface = true;
//        }
//    }

//    private void UpdateSliderAndShowMultiplierMouseOver(UIElement affectedElement)
//    {
//        var dimensions = affectedElement.GetDimensions();
//        if (dimensions.ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
//        {
//            string originalText = "x" + BattlerPlayer.RemapSliderValueToPowerValue(GetSliderValue()).ToString("F2");
//            if (_sliderCurrentValueCache == 0f && Config.SpawnRateMinValue == 0)
//                originalText = Language.GetTextValue("CreativePowers.NPCSpawnRateSliderEnemySpawnsDisabled");

//            MainPanel.displayTextCache = originalText;
//        }
//        else if (MainPanel.GetDimensions().ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
//        {
//            // [AI注释] 鼠标在面板但不在滑条上时，显示功能描述而不是倍率值。
//            string text = GetText("BuffTracker.NPCSpawnRatePanel");

//            MainPanel.displayTextCache = text;
//        }

//        AttemptPushingChange();
//    }

//    internal float GetSliderValue() => _sliderCurrentValueCache;

//    public void ResetDataForNewPlayer(int playerIndex)
//    {
//        if (Main.player[playerIndex].TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
//        {
//            battlerPlayer.SpawnRateSliderValue = BattlerPlayer.SliderDefaultValue;
//            if (playerIndex == Main.myPlayer)
//            {
//                // [AI注释] 本地玩家重置时主动同步默认值，避免 UI 缓存和实际值不一致。
//                _currentTargetValue = BattlerPlayer.SliderDefaultValue;
//                _sliderCurrentValueCache = BattlerPlayer.SliderDefaultValue;
//                SpawnRateSlider.Get(Main.myPlayer, _sliderCurrentValueCache).Send(runLocally: false);
//            }
//        }
//    }

//    public void Update()
//    {
//        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
//        {
//            int rateMax = Config.SpawnRateMaxValue;
//            maxRateText.SetText($"x{rateMax}");
//            maxRateText.HAlign = rateMax < 100 ? rateMax < 10 ? 0.7f : 1f : 1.4f;

//            float rateMin = Config.SpawnRateMinValue;
//            if (rateMin > rateMax) rateMin = rateMax;
//            minRateText.SetText($"x{rateMin:0.0}");

//            // [AI注释] 中间档位文案按区间自适配，尽量提供可读且有意义的刻度提示。
//            string text;
//            if (rateMin < 1)
//                text = "x1";
//            else if (rateMax < 10)
//                text = rateMin > 1 ? $"x{(rateMax + rateMin) * .5f:0.0}" : "x1";
//            else
//                text = "x10";
//            midRateText.SetText(text);
//        }
//        else
//        {
//            _currentTargetValue = BattlerPlayer.SliderDefaultValue;
//            _sliderCurrentValueCache = BattlerPlayer.SliderDefaultValue;
//        }
//    }
//}