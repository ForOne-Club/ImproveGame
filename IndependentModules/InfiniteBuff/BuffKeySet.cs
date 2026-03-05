using Terraria.ModLoader.IO;

namespace ImproveGame.IndependentModules.InfiniteBuff;

/// <summary>
/// Buff 键集合（兼容原版 ID 与 Mod 全名两种键）。
/// </summary>
public class BuffKeySet
{
    /// <summary>
    /// 原版 Buff 键：直接使用 BuffType（int）。
    /// </summary>
    public HashSet<int> Ids { get; } = [];

    /// <summary>
    /// Mod Buff 键：使用“模组名/Buff名”。
    /// </summary>
    public HashSet<string> FullNames { get; } = [];

    /// <summary>
    /// 从存档读取键集合。
    /// </summary>
    public void LoadData(TagCompound tag)
    {
        if (tag.TryGet<int[]>(nameof(Ids), out var ids))
        {
            Ids.Clear();

            foreach (var id in ids)
            {
                Ids.Add(id);
            }
        }

        if (tag.TryGet<string[]>(nameof(FullNames), out var names))
        {
            FullNames.Clear();

            foreach (var name in names)
            {
                FullNames.Add(name);
            }
        }
    }

    /// <summary>
    /// 导出当前键集合到可存档结构。
    /// </summary>
    public TagCompound GetData()
    {
        return new TagCompound
        {
            [nameof(Ids)] = Ids.ToArray(),
            [nameof(FullNames)] = FullNames.ToArray()
        };
    }

    /// <summary>
    /// 切换某个 Buff 对应键的存在状态（存在则移除，不存在则添加）。
    /// </summary>
    /// <param name="buffType">目标 BuffType。</param>
    public void Toggle(int buffType)
    {
        if (BuffLoader.GetBuff(buffType) is { } modBuff)
        {
            // Mod Buff 使用“模组名/Buff名”作为键，避免与原版或其他模组的数值 ID 冲突。
            var fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";

            if (!FullNames.Add(fullName))
            {
                FullNames.Remove(fullName);
            }
        }
        else
        {
            // 原版 Buff 直接以数值 ID 作为键进行开关切换。
            if (!Ids.Add(buffType))
            {
                Ids.Remove(buffType);
            }
        }
    }

    /// <summary>
    /// 按 BuffType 查询键集合是否“包含该 Buff”。
    /// </summary>
    /// <remarks>
    public bool ContainsByType(int buffType)
    {
        if (BuffLoader.GetBuff(buffType) is { } modBuff)
        {
            var fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
            return FullNames.Contains(fullName);
        }

        return Ids.Contains(buffType);
    }

    /// <summary>
    /// 将当前键集合解析为 BuffType 集合。
    /// </summary>
    /// <remarks>
    /// Mod 键通过 <see cref="BuffID.Search"/> 反查数值 ID。
    /// </remarks>
    public HashSet<int> GetBuffTypes()
    {
        var types = new HashSet<int>();

        foreach (var id in Ids)
        {
            types.Add(id);
        }

        foreach (var fullName in FullNames)
        {
            types.Add(BuffID.Search.GetId(fullName));
        }

        return types;
    }

}
