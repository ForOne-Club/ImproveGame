# 📘 代码规范文档

建议遵循以下代码规范，使项目内的代码风格更加统一。

*由Github Copilot生成，Cyrilly负责修改和后续维护。*

## 注释行规范

- 使用双斜杠 `//` 进行单行注释，注释应简洁明了，准确描述代码的功能或目的。
- 注释应与代码保持一致，避免过时或误导性的注释。
- 注释一般应放在被注释代码的上方，并与代码保持一个空行的间隔。
- 双斜杠 `//` 和描述文本之间应该**保留一个空格的间隙**以保证整体美观。
- VS可以使用 `Ctrl + K, Ctrl + C` 快捷键进行多行注释，使用 `Ctrl + K, Ctrl + U` 取消多行注释。

示例：

```csharp
// 主面板
public SUIPanel MainPanel;
```

## 换行规范

- 每个方法之间应保持一个**空行**，以提高代码的可读性。
- 在**逻辑上相关**的代码块之间应保持一个空行，以便于区分不同的逻辑部分。
- 方法内部的代码应**根据逻辑分段**，适当使用空行进行分隔。

示例：

```csharp
public override void OnInitialize()
{
    const int gapBetweenPanels = 20;
    const int sidePanelWidth = 220;
    const int tooltipPanelHeight = 146;
    Instance = this;

    // 主面板
    MainPanel = new SUIPanel(ConfigColors.MainPanelBorder, ConfigColors.MainPanelBg)
    {
        Shaded = true,
        HAlign = 0.5f,
        VAlign = 0.5f
    };
    MainPanel.SetPadding(gapBetweenPanels);
    MainPanel.SetPosPixels(0f, -20f);
    MainPanel.SetSizePercent(0.86f, 0.82f)
        .JoinParent(this);

    // 侧栏放类别
    CategoryPanel = new CategorySidePanel(ConfigColors.DarkBorderlessPanel)
    {
        RelativeMode = RelativeMode.Horizontal
    };
    CategoryPanel.SetSize(sidePanelWidth, 0f, 0f, 1f);
    CategoryPanel.JoinParent(MainPanel);
}
```

## 变量和方法的位置规范

- **无注释**的全局变量，根据**实现的功能上的重合**决定是否留空行，用于实现同一个功能的变量要放在一起。
- **有注释**的全局变量，一律在前后留空行
- 构造函数应放在类的最前面，紧随其后的是属性和字段的定义，最后是方法的定义。
- 一个类内如果还有类，应当把该内含类的全部代码放在最前面。

示例：

```csharp
public sealed class ModernConfigUI : UIState
{
    /// <summary>
    /// 如有必要添加注释
    /// </summary>
    public class SubUI : View
    {
        // code
    }

    public static ModernConfigUI Instance { get; set; }
    public bool Enabled { get; set; }

    public bool OpenFromMasterControl;

    public static bool DrawCalledForMakingGlass;
    public static RenderTarget2D Glass; // 云母效果用

    // 主面板
    public SUIPanel MainPanel;

    // 侧栏放类别
    public CategorySidePanel CategoryPanel;

    // 主栏上放选项
    public ConfigOptionsPanel OptionsPanel;

    // 主栏下放描述
    public TooltipPanel TooltipPanel;

    // 当前打开的mod
    public Mod CurrentMod;

    public override void OnInitialize()
    {
        // 方法实现
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // 方法实现
    }

    public void Open(Mod mod)
    {
        // 方法实现
    }

    public void Close()
    {
        // 方法实现
    }

    public override void Update(GameTime gameTime)
    {
        // 方法实现
    }
}
```

## 命名约定

变量名应具有描述性，能够准确反映其用途。**切忌a, b, c这种不明所以的OIer式命名。**

1. **类名**：使用 PascalCase，例如 `IndicatorMapLayer`。
2. **方法名**：使用 PascalCase，例如 `Draw`、`ProcessStructure`。
3. **局部变量名**：使用 camelCase，例如 `columns`、`rows`。
4. **全局变量名**：私有字段使用 _camelCase，例如 `_width`、`_height`。非私有字段使用 PascalCase，例如 `Draggable`、`Offset`。
5. **全局常量名**：使用 PascalCase，例如 `TimeLeftMax`。
6. **命名空间**：使用 PascalCase，并且一般应与项目结构一致，例如 `ImproveGame.Content`。

## 代码格式

1. **缩进**：使用 4 个空格进行缩进。
2. **行宽**：每行代码不超过 120 个字符。（不严格要求）
3. **大括号**：大括号 `{}` 应与控制语句（如 `if`、`for` 等）在新行开始，并在新行结束。

   ```csharp
   if (condition)
   {
       // code
   }
   else
   {
       // code
   }
   ```

4. **空行**：在方法之间、类之间以及逻辑上有明显分隔的代码块之间使用空行分隔。

## 最佳实践

1. **代码复用**：尽量避免代码重复，使用方法和类来封装重复的逻辑。
2. **异常处理**：使用 `try-catch` 块进行异常处理，并记录异常日志。

   ```csharp
   try
   {
       // code
   }
   catch (Exception ex)
   {
       // 记录异常日志
       Console.WriteLine(ex.Message);
   }
   ```

3. **代码优化**：定期进行代码审查和优化，确保代码的性能和可维护性。
4. **单一职责原则**：每个类和方法应只负责一项职责，避免过多的功能耦合。
5. **使用配置文件**：将可配置的参数放在配置文件中，避免硬编码。

通过整合上述代码规范，可以提高代码的可读性和可维护性，确保团队成员之间的代码风格一致。
