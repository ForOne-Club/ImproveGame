namespace ImproveGame.Common.Utils;

// 如果有什么不足请为本仓库创建 Issue 或 Pr

public static class ProjectileHelper
{
    // NPC.immune 确定一个 npc 是否可以被特定玩家拥有的物品或弹丸击中（它是一个数组，每个插槽对应不同的玩家（whoAmI））
    // NPC.immune 每次更新都会递减到 0
    // 近战物品将 NPC.immune 设置为 player.itemAnimation，它从 item.useAnimation 开始并向 0 递减，然而，射弹提供了自定义免疫机制。
    // 1. penetrate == 1: 无论 npc 的免疫计数器如何，在 SetDefaults 中将穿透设置为 1 的射弹都会命中（在 maxPenetrate 中记住 SetDefaults 中的穿透）
    //	  例如：木箭。
    // 2. No code and penetrate > 1 or -1: npc.immune[owner] 将被设置为 10。
    // 	  如果 NPC 没有免疫则将被击中并且免疫所有伤害 10 ticks
    // 	  例如：邪恶之箭
    // 3. 覆盖 OnHitNPC：如果不免疫，当它击中它时手动设置一个非 10 的免疫
    // 	  例如：Arkhalis：将其设置为 5
    // 	  例如：Sharknado Minion：设置为 20
    // 4. Projectile.usesIDStaticNPCImmunity 和 Projectile.idStaticNPCHitCooldown：指定一种类型的弹丸对每个npc都有一个共享的免疫计时器。
    //    如果你希望其他射弹有机会造成伤害，但不希望相同的射弹类型快速击中 NPC，请使用此选项。
    //    例如：Ghastly Glaive 是唯一一个使用它的人。
    // 5. Projectile.usesLocalNPCImmunity 和 Projectile.localNPCHitCooldown：指定射弹为每个 npc 管理它自己的免疫计时器
    //    如果你想让多个相同类型的弹丸有机会快速攻击，但不希望单个弹丸快速命中，请使用此选项。-1 值可防止同一射弹再次击中 NPC。
    //    例如：Lightning Aura 哨兵使用这个。（localNPCHitCooldown = 3，但其他代码控制弹丸本身命中的速度）
    //    重叠的光环都有机会互相攻击，即使它们共享相同的 ID。

    /// <summary>
    /// 设置弹幕默认参数
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="timeLeft"></param>
    /// <param name="friendly"></param>
    /// <param name="extraUpdates"></param>
    public static void SetBaseValues(this Projectile projectile,
        int width, int height,
        int timeLeft, bool tileCollide = true,
        int alpha = 255, float light = 0f, bool ignoreWater = false, int extraUpdates = 0)
    {
        projectile.width = width;
        projectile.height = height;
        projectile.timeLeft = timeLeft;
        projectile.tileCollide = tileCollide;
        projectile.alpha = alpha;
        projectile.light = light;
        projectile.ignoreWater = ignoreWater;
        projectile.extraUpdates = extraUpdates;
    }

    /// <summary>
    /// 设置攻击类属性
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="penetrate"></param>
    /// <param name="friendly"></param>
    /// <param name="hostile"></param>
    /// <param name="usesIDStaticNPCImmunity"></param>
    /// <param name="idStaticNPCHitCooldown"></param>
    /// <param name="usesLocalNPCImmunity"></param>
    /// <param name="localNPCHitCooldown"></param>
    public static void SetAttackValues(this Projectile projectile,
        DamageClass damageType, int penetrate = 1, bool friendly = false, bool hostile = false,
        bool usesIDStaticNPCImmunity = false, int idStaticNPCHitCooldown = -1,
        bool usesLocalNPCImmunity = false, int localNPCHitCooldown = -1)
    {
        projectile.DamageType = damageType;
        projectile.penetrate = penetrate;
        projectile.hostile = hostile;
        projectile.friendly = friendly;
        projectile.usesIDStaticNPCImmunity = usesIDStaticNPCImmunity;
        projectile.idStaticNPCHitCooldown = idStaticNPCHitCooldown;
        projectile.usesLocalNPCImmunity = usesLocalNPCImmunity;
        projectile.localNPCHitCooldown = localNPCHitCooldown;
    }

    // 弹幕注意事项，必要的东西
    // 1. 弹幕命中任何单位或方块音效，消失音效看情况而定
    // 2. 弹幕命中任何单位或方块粒子效果，通常弹幕在撞击方块的时候消失，固将碰撞粒子写于 OnKill()
}
