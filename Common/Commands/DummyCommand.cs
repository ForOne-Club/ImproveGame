using ImproveGame.Attributes;
using ImproveGame.Content.NPCs.Dummy;
using System.Reflection;

namespace ImproveGame.Common.Commands;

public class DummyCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "dummy";
    public override string Usage => "用法: /dummy <attribute> <value> (不区分大小写)";
    public override string Description =>
        $"attribute: 属性名, value: 值. 可以输入指令 /dummy info 查看可用属性与对应类型以及当前值.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Type type = typeof(DummyConfig);

        if (args.Length == 1)
        {
            if (args[0].Equals("info", StringComparison.OrdinalIgnoreCase))
            {
                FieldInfo[] fields = type.GetFields();

                caller.Reply("假人属性信息：", MyColor.Normal);

                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<AnnotateAttribute>() is AnnotateAttribute annotateAttribute)
                    {
                        ref string annotate = ref annotateAttribute.Annotate;

                        if (annotate.Equals(string.Empty))
                        {
                            annotate = GetText($"NPC.{field.Name}");
                        }
                        else if (annotate.Length > 1 && annotate.StartsWith('$'))
                        {
                            annotate = GetText($"NPC.{annotate.TrimStart('$')}");
                        }

                        caller.Reply($"[{field.FieldType.Name}] {field.Name}: {field.GetValue(DummyNPC.Config)} ({annotate})", MyColor.Normal);
                    }
                }

                return;
            }
        }

        if (args.Length == 2)
        {
            string name = args[0];
            FieldInfo[] fields = type.GetFields();

            foreach (var field in fields)
            {
                if (field.FieldType.IsPrimitive && field.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        field.SetValueDirect(__makeref(DummyNPC.Config), Convert.ChangeType(args[1], field.FieldType));
                        caller.Reply($"修改成功: {name} {args[1]} !!!", MyColor.Success);
                        return;
                    }
                    catch
                    {
                        caller.Reply($"修改失败 ( {input} ) !!!", MyColor.Fail);
                        return;
                    }
                }
            }
        }

        caller.Reply($"无效指令 ( {input} ) !!!", MyColor.Fail);
    }

    public record CommandColor(Color Normal, Color Success, Color Fail);
    public CommandColor MyColor = new(new(0, 255, 127), new(0, 200, 0), new(200, 0, 0));
}
