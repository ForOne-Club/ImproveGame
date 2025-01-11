using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements;

/// <summary>
/// 用于输入文本
/// </summary>
public sealed class OptionEditableText : ModernConfigOption
{
    private SUIEditableText _TextBox;
    public OptionEditableText(ModConfig config, string optionName) : base(config, optionName, 70)
    {
    }
    public OptionEditableText(ModConfig config, PropertyFieldWrapper variableInfo) : base(config, variableInfo, 70)
    {
    }
    protected override void OnBind(ModConfig config, string optionName, int reservedWidth)
    {
        base.OnBind(config, optionName, reservedWidth);
        CheckValid();

        var box = new View
        {
            IsAdaptiveWidth = true,
            HAlign = 1f,
            VAlign = 0.5f,
            Height = StyleDimension.Fill
        };
        box.JoinParent(this);

        bool isInt = VariableInfo.Type == typeof(int);
        _TextBox = new SUIEditableText
        {
            RelativeMode = RelativeMode.Horizontal,
            BgColor = Color.Black * 0.4f,
            Rounded = new Vector4(12f),
            InnerText =
            {
                TextAlign = new Vector2(0.5f, 0.5f),
                TextOffset = new Vector2(0f, -2f),
                MaxCharacterCount = isInt ? 12 : 4,
                MaxLines = 1,
                IsWrapped = false
            },
            MaxLength = isInt ? 12 : 4,
            VAlign = 0.5f
        };
        _TextBox.ContentsChanged += (ref string text) =>
        {
            SetConfigValue(text, broadcast: false);
        };
        _TextBox.EndTakingInput += () =>
        {
            SetConfigValue(_TextBox.Text, broadcast: true);
        };
        _TextBox.SetPadding(2, 2, 2, 2); // Padding影响里面的文字绘制
        _TextBox.SetSizePixels(50, 28);
        _TextBox.JoinParent(box);
    }
    private void CheckValid()
    {
        if (VariableInfo.Type != typeof(string))
            throw new Exception($"Field \"{OptionName}\" is not a string");
    }

    private void SetConfigValue(string value, bool broadcast)
    {
        //if (!Interactable) return;
        SetValueDirect(value);
        //ConfigHelper.SetConfigValue(Config, VariableInfo, value, Item, broadcast, path: path);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        _TextBox.IgnoresMouseInteraction = !Interactable;
        var value = VariableInfo.GetValue(Item)?.ToString();
        if (!_TextBox.IsWritingText)
            _TextBox.Text = value;
    }
}