namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 无限 BUFF 帮助类
/// </summary>
internal static class InfiniteBuffHelper
{
    public static string LeftClickDisable => GetText("BuffTracker.LeftClickDisable");
    public static string LeftClickEnable => GetText("BuffTracker.LeftClickEnable");
    public static string RightClickDisable => GetText("BuffTracker.RightClickDisable");
    public static string RightClickEnable => GetText("BuffTracker.RightClickEnable");

    public static string GetLeftClickString(bool enable)
    {
        return enable ? LeftClickDisable : LeftClickEnable;
    }

    public static string GetRightClickString(bool enable)
    {
        return enable ? RightClickDisable : RightClickEnable;
    }
}

//public static class ExpressionHelper
//{
//    /// <summary>
//    /// 生成一个传入对象获取对象中 <see cref="PropertyInfo"/> 属性的方法
//    /// </summary>
//    /// <param name="prop">对应的属性信息</param>
//    /// <returns>获取属性的方法</returns>
//    public static Func<object, object> CreateGetter(PropertyInfo prop)
//    {
//        var instance = Expression.Parameter(typeof(object), "obj");

//        // ((T)obj).Property
//        var propertyAccess = Expression.Property(Expression.Convert(instance, prop.DeclaringType!), prop);

//        // 转为 object: (object)(((T)obj).Property)
//        var convertResult = Expression.Convert(propertyAccess, typeof(object));

//        // Lambda: (object obj) => (object)((T)obj).Property
//        var lambda = Expression.Lambda<Func<object, object>>(convertResult, instance);

//        return lambda.Compile();
//    }

//    /// <summary>
//    /// 为指定类型生成一个基于带有 <see cref="KeyPropertyAttribute"/> 属性的哈希函数。
//    /// 返回的委托接受一个 object 实例，计算其关键属性的 HashCode。
//    /// </summary>
//    /// <param name="type">要生成哈希函数的类型</param>
//    /// <returns>一个 Func<object, int>，用于计算实例的 HashCode</returns>
//    public static Func<object, int> CreateKeyPropertiesHashCodeFunc(Type type)
//    {
//        // 输入参数 obj
//        var objParam = Expression.Parameter(typeof(object), "obj");

//        // 类型转换为目标类型
//        var targetVar = Expression.Variable(type, "target");

//        // 用于累加哈希值
//        var hashCodeVar = Expression.Variable(typeof(HashCode), "hashCode");

//        // 初始化表达式块
//        var blockExpressions = new List<Expression>
//        {
//            // obj 转换为 target 类型
//            Expression.Assign(targetVar, Expression.Convert(objParam, type)),

//            // new HashCode()
//            Expression.Assign(hashCodeVar, Expression.New(typeof(HashCode)))
//        };

//        // 获取所有带 KeyPropertyAttribute 的公共实例属性
//        var keyProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
//            .Where(p => p.IsDefined(typeof(KeyPropertyAttribute), inherit: true)
//                        && p.CanRead && p.CanWrite)
//            .ToArray();

//        // HashCode.Add<T>(T value) 方法定义
//        var addMethodDef = typeof(HashCode)
//            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//            .First(m => m.Name == "Add" && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);

//        // 为每个关键属性调用 hashCode.Add(propValue)
//        foreach (var prop in keyProps)
//        {
//            var addMethod = addMethodDef.MakeGenericMethod(prop.PropertyType);
//            var getter = Expression.Property(targetVar, prop);
//            blockExpressions.Add(Expression.Call(hashCodeVar, addMethod, getter));
//        }

//        // 调用 hashCode.ToHashCode()
//        var toHashCodeMethod = typeof(HashCode)
//            .GetMethod(nameof(HashCode.ToHashCode), BindingFlags.Public | BindingFlags.Instance);
//        blockExpressions.Add(Expression.Call(hashCodeVar, toHashCodeMethod));

//        // 返回 Lambda 表达式并编译
//        return Expression
//            .Lambda<Func<object, int>>(
//                Expression.Block([targetVar, hashCodeVar], blockExpressions),
//                objParam)
//            .Compile();
//    }

//}

//[AttributeUsage(AttributeTargets.Property)]
//public class KeyPropertyAttribute : Attribute;

//public abstract class DirtyTrackable
//{
//    private static readonly Dictionary<Type, Func<object, int>> _hashFuncCache = [];

//    private int _snapshotHash;

//    private bool _isDirty;

//    protected void MarkDirty() { _isDirty = true; }
//    protected void ClearDirty() { _isDirty = false; }

//    /// <summary>
//    /// 检测对象关键数据是否发生变化
//    /// </summary>
//    protected void CheckDirty()
//    {
//        if (!_isDirty) return;
//        ClearDirty();

//        int hash = ComputeObjectHash();
//        if (hash != _snapshotHash)
//        {
//            _snapshotHash = hash;
//            OnChanged();
//        }
//    }

//    protected virtual void OnChanged() { }

//    private int ComputeObjectHash()
//    {
//        if (!_hashFuncCache.TryGetValue(GetType(), out var getHashCode))
//        {
//            _hashFuncCache[GetType()] = getHashCode = ExpressionHelper.CreateKeyPropertiesHashCodeFunc(GetType());
//        }

//        return getHashCode(this);
//    }
//}

//public sealed class BuffFilter : DirtyTrackable
//{
//    [KeyProperty]
//    public string Keywords
//    {
//        get; set
//        {
//            if (value == null) return;
//            if (field == value) return;
//            field = value;
//            MarkDirty();
//        }
//    } = string.Empty;

//    public void Update()
//    {
//        CheckDirty();
//    }

//    protected override void OnChanged()
//    {
//        var list = new List<int>();
//    }
//}
