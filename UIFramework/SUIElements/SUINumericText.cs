using System.Globalization;

namespace ImproveGame.UIFramework.SUIElements;

// 只能输入数字的文本框
public class SUINumericText : SUIEditableText
{
    public string Format = "0.00";
    public double DefaultValue = 0;
    public double MinValue = 0;
    public double MaxValue = 1;

    public bool IsValueSafe => double.TryParse(Text, out _);

    public double Value
    {
        get => double.Parse(Text);
        set => Text = value.ToString(Format);
    }

    public SUINumericText()
    {
        // 删除所有非数字字符，保留数字和小数点（俄语小数点是逗号，也是挺神奇的）
        ContentsChanged += (ref string text) =>
        {
            text = new string(text.Where(c => char.IsDigit(c) || c is '.' or '-' or ',').ToArray());
        };

        // 结束输入时检查是否在范围内
        EndTakingInput += () =>
        {
            Value = !double.TryParse(Text, out double digit)
                ? DefaultValue
                : Math.Clamp(digit, MinValue, MaxValue);
        };
    }
}