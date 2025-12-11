using ImproveGame.Helpers.Extensions;
using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UIFramework.BaseViews;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.SUIElements
{
    /// <summary>
    /// 下拉框，用于Enum类型的字段
    /// </summary>
    public class SUIDropdownList<T> : TimerView where T : Enum
    {
        private bool DropdownListPersists => container.DropdownCaller == this;
        private readonly TimerView _textBox;
        private readonly SlideText _textElement;
        private string[] _valueStrings;
        private string[] _valueTooltips;
        private float _maxTextWidth;
        private Func<T> getState;
        private Action<object> setState;

        public SUIDropdownListContainer container { get; private set; }

        public SUIDropdownList(Func<T> getState, Action<object> setState, SUIDropdownListContainer container, string optionName)
        {
            //CheckValid();
            this.container = container;
            this.getState = getState;
            this.setState = setState;
            var box = new View
            {
                IsAdaptiveWidth = true,
                HAlign = 1f,
                VAlign = 0.5f,
                Height = StyleDimension.Fill
            };
            box.JoinParent(this);

            GetOptions();

            _textBox = new TimerView
            {
                RelativeMode = RelativeMode.Horizontal,
                BgColor = Color.Black * 0.3f,
                Rounded = new Vector4(6f),
                VAlign = 0.5f
            };
            _textBox.OnLeftMouseDown += (_, _) =>
            {
                //if (!Interactable) return;
                if (this.container.Enabled)
                {
                    this.container.Enabled = false;
                    return;
                }
                SoundEngine.PlaySound(SoundID.MenuTick);
                var dimensions = _textBox.GetDimensions();
                float x = dimensions.X;
                float y = dimensions.Y + 29;
                float width = _textBox.Width();
                var dropdownList = this.container;
                dropdownList.BuildDropdownList(x, y, width, _valueStrings, GetString(), this);
                dropdownList.OptionSelectedCallback = s =>
                {
                    int index = Array.IndexOf(_valueStrings, s);
                    SetConfigValue(index);
                };
                //dropdownList.DrawCallback = () =>
                //{
                //    TooltipPanel.SetText(Tooltip);
                //};
                //dropdownList.HoverOnOptionCallback = s =>
                //{
                //    int index = Array.IndexOf(_valueStrings, s);
                //    string tooltip = _valueTooltips[index];
                //    if (!string.IsNullOrWhiteSpace(tooltip))
                //        TooltipPanel.SetText($"{Tooltip}\n[c/0099ff:{s}]: {tooltip}");
                //    // 这里不需要else了，因为DrawCallback会先执行
                //};
            };
            float width = Math.Min(320, _maxTextWidth + 48);
            _textBox.SetPadding(0); // Padding影响里面的文字绘制
            _textBox.SetSizePixels(width, 28);
            _textBox.JoinParent(box);

            _textElement = new SlideText(GetString(), 30)
            {
                VAlign = 0.5f,
                Left = { Pixels = 8 },
                RelativeMode = RelativeMode.None
            };
            _textElement.JoinParent(_textBox);
        }

        private void GetOptions()
        {
            _valueStrings = Enum.GetNames(typeof(T));
            _valueTooltips = new string[_valueStrings.Length];

            for (int i = 0; i < _valueStrings.Length; i++)
            {
                var enumFieldFieldInfo = typeof(T).GetField(_valueStrings[i]);
                if (enumFieldFieldInfo is null)
                    continue;

                string name = ConfigManager.GetLocalizedLabel(new PropertyFieldWrapper(enumFieldFieldInfo));
                _valueStrings[i] = name;
                string tooltip = ConfigManager.GetLocalizedTooltip(new PropertyFieldWrapper(enumFieldFieldInfo));
                _valueTooltips[i] = tooltip;
            }

            _maxTextWidth = _valueStrings.Max(i => ChatManager.GetStringSize(FontAssets.MouseText.Value, i, Vector2.One).X);
        }

        //private void CheckValid()
        //{
        //    if (!typeof(T).IsEnum)
        //        throw new Exception($"Field \"{OptionName}\" is not a enum type");
        //}

        private void SetConfigValue(int index)
        {
            //if (!Interactable) return;

            var value = Enum.GetValues(typeof(T)).GetValue(index);
            setState?.Invoke(value);
            //ConfigHelper.SetConfigValue(Config, FieldInfo, value, broadcast);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 就算没有Hover，要是下拉框打开了也要高光（调整AnimationTimer）
            if (!IsMouseHovering)
            {
                if (DropdownListPersists)
                {
                    HoverTimer.ImmediateOpen();
                }
            }

            //_textBox.IgnoresMouseInteraction = !Interactable;
            _textBox.BgColor = _textBox.HoverTimer.Lerp(Color.Black * 0.4f, Color.Black * 0.2f);
            _textElement.DisplayText = GetString();




        }
        public override void Recalculate()
        {
            base.Recalculate();
            var dimensions = _textBox.GetDimensions();
            float x = dimensions.X;
            float y = dimensions.Y + 29;
            float width = _textBox.Width();
            container.SetCurrentPosition(x, y, width, _valueStrings.Length);
            container.Recalculate();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            /*
            var dimensions = _textBox.GetDimensions();
            float x = dimensions.X;
            float y = dimensions.Y + 29;
            float width = _textBox.Width();
            container.SetCurrentPosition(x, y, width, _valueStrings.Length);
            container.Recalculate();
            */

            var textBoxRect = _textBox.GetDimensions().ToRectangle();
            var tex = ModAsset.DropdownListMark.Value;
            //var effects = DropdownListPersists
            //    ? SpriteEffects.FlipVertically
            //    : SpriteEffects.None;
            var effects = SpriteEffects.None;
            spriteBatch.Draw(tex, new Vector2(textBoxRect.Right - 16, textBoxRect.Center.Y + 1),
                null, Color.White, 0f, tex.Size() / 2f, Vector2.One, effects, 0f);
        }

        private int GetIndex() => Array.IndexOf(Enum.GetValues(typeof(T)), getState());
        private string GetString() => _valueStrings[GetIndex()];
    }
}
