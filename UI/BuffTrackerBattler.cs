using ImproveGame.Common.ModPlayers;
using ImproveGame.Packets;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameContent.Creative;

namespace ImproveGame.UI
{
    public class BattlerPanel : SUIPanel
    {
        public BattlerPanel() : base(Color.Black, UIStyle.TitleBg) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
                base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
                base.Update(gameTime);
        }
    }

    public class BuffTrackerBattler
    {
        internal BattlerPanel MainPanel;
        internal UIText maxRateText;
        internal float _sliderCurrentValueCache;
        private float _currentTargetValue;
        private bool _needsToCommitChange;
        private DateTime _nextTimeWeCanPush = DateTime.UtcNow;

        public void Initialize()
        {
            MainPanel = new()
            {
                Width = new StyleDimension(77f, 0f),
                Height = new StyleDimension(220f, 0f)
            };
            MainPanel.OnUpdate += UpdateUseMouseInterface;

            UIVerticalSlider uIVerticalSlider =
                CreativePowersHelper.CreateSlider(GetSliderValue, SetValueKeyboard, SetValueGamepad);
            uIVerticalSlider.OnUpdate += UpdateSliderAndShowMultiplierMouseOver;
            uIVerticalSlider.EmptyColor = Color.Lime;
            uIVerticalSlider.FilledColor = Color.CornflowerBlue;
            MainPanel.Append(uIVerticalSlider);

            maxRateText = new("x200")
            {
                HAlign = 1.6f,
                VAlign = 0f
            };
            MainPanel.Append(maxRateText);

            UIText element2 = new("x1")
            {
                HAlign = 0.8f,
                VAlign = 0.5f
            };
            MainPanel.Append(element2);

            UIText element3 = new("x0")
            {
                HAlign = 0.9f,
                VAlign = 1f
            };
            MainPanel.Append(element3);
        }

        private void AttemptPushingChange()
        {
            if (_needsToCommitChange && DateTime.UtcNow.CompareTo(_nextTimeWeCanPush) != -1)
                PushChange(_currentTargetValue);
        }

        internal void PushChange(float newSliderValue)
        {
            _needsToCommitChange = false;
            _sliderCurrentValueCache = newSliderValue;
            _nextTimeWeCanPush = DateTime.UtcNow;
            SpawnRateSlider.Get(Main.myPlayer, _sliderCurrentValueCache).Send(runLocally: true);
        }

        internal void SetValueKeyboard(float value)
        {
            if (value != _currentTargetValue)
            {
                _currentTargetValue = value;
                _needsToCommitChange = true;
            }
        }

        internal void SetValueGamepad() { } // 我寻思着，就算你真的用手柄玩Mod，咱模组也不支持啊

        private void UpdateUseMouseInterface(UIElement affectedElement)
        {
            var dimensions = affectedElement.GetDimensions();
            if (dimensions.ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        private void UpdateSliderAndShowMultiplierMouseOver(UIElement affectedElement)
        {
            var dimensions = affectedElement.GetDimensions();
            if (dimensions.ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
            {
                string originalText = "x" + BattlerPlayer.RemapSliderValueToPowerValue(GetSliderValue()).ToString("F2");
                if (_sliderCurrentValueCache == 0f)
                    originalText = Language.GetTextValue("CreativePowers.NPCSpawnRateSliderEnemySpawnsDisabled");

                Main.instance.MouseTextNoOverride(originalText, 0, 0);
            }
            else if (MainPanel.GetDimensions().ToRectangle().Intersects(new(Main.mouseX, Main.mouseY, 1, 1)))
            {
                string text = GetText("BuffTracker.NPCSpawnRatePanel");
                Main.instance.MouseTextNoOverride(text, 0, 0);
            }

            AttemptPushingChange();
        }

        internal float GetSliderValue() => _sliderCurrentValueCache;

        public void ResetDataForNewPlayer(int playerIndex)
        {
            if (Main.player[playerIndex].TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
            {
                battlerPlayer.SpawnRateSliderValue = BattlerPlayer.SliderDefaultValue;
                if (playerIndex == Main.myPlayer)
                {
                    _currentTargetValue = BattlerPlayer.SliderDefaultValue;
                    _sliderCurrentValueCache = BattlerPlayer.SliderDefaultValue;
                    SpawnRateSlider.Get(Main.myPlayer, _sliderCurrentValueCache).Send(runLocally: false);
                }
            }
        }

        public void Update()
        {
            if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var modPlayer) && modPlayer.HasRequiredBuffs())
            {
                int rate = Config.SpawnRateMaxValue;
                maxRateText.SetText($"x{rate}");
                maxRateText.HAlign = rate < 100 ? rate < 10 ? 0.7f : 1f : 1.4f;
            }
            else
            {
                _currentTargetValue = BattlerPlayer.SliderDefaultValue;
                _sliderCurrentValueCache = BattlerPlayer.SliderDefaultValue;
            }
        }
    }
}