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
    protected override void OnBind()
    {
        CheckValid();

        var box = new View
        {
            IsAdaptiveWidth = true,
            HAlign = 1f,
            VAlign = 0.5f,
            Height = StyleDimension.Fill
        };
        box.JoinParent(this);

        _TextBox = new SUIEditableText
        {
            RelativeMode = RelativeMode.Horizontal,
            BgColor = Color.Black * 0.4f,
            Rounded = new Vector4(12f),
            InnerText =
            {
                TextAlign = new Vector2(0.5f, 0.5f),
                TextOffset = new Vector2(0f, -2f),
                MaxCharacterCount = 200,
                MaxLines = 1,
                IsWrapped = false
            },
            MaxLength = 200,
            VAlign = 0.5f,
            MaxWidth = new(0, 0.75f),
            MinWidth = new(50,0)
        };
        _TextBox.ContentsChanged += (ref string text) =>
        {
            SetConfigValue(text, broadcast: false);
            var size = FontAssets.MouseText.Value.MeasureString(text);
            var dimension = _TextBox.GetDimensions();
            _TextBox.Width.Set(size.X + 8, 0);
            this.Recalculate();
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
        if (VarType != typeof(string))
            throw new Exception($"Field \"{OptionName}\" is not a string");
    }

    private void SetConfigValue(string value, bool broadcast)
    {
        //if (!Interactable) return;
        SetValueDirect(value,broadcast);
        //ConfigHelper.SetConfigValue(Config, VariableInfo, value, Item, broadcast, path: path);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        _TextBox.IgnoresMouseInteraction = !Interactable;
        var value = GetValue()?.ToString();
        if (!_TextBox.IsWritingText)
            _TextBox.Text = value;
    }
}