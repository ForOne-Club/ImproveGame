using ImproveGame.Content.Projectiles;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes.Core;

/// <summary>
/// 世界生成时就保存了数据，而且不会变的球，使用一次可以解锁全部对应数据
/// </summary>
public abstract class OnceForAllGlobe() : Globe(ItemRarityID.Quest, Item.sellPrice(silver: 10));
public interface IOnceForAllGlobeProj
{
    StructureDatas.UnlockID StructureType { get; }
    bool NotFoundCheck();
    void ExtraCheckWhenNotRecorded();
    Point16[] Positions { get; }
    Point16[] PositionsAnother { get; }
}
public abstract class OnceForAllGlobeProj<T>(Color mainColor) : GlobeProjBase<T>(mainColor), IOnceForAllGlobeProj where T : OnceForAllGlobe
{
    public abstract StructureDatas.UnlockID StructureType { get; }
    public abstract bool NotFoundCheck();
    public override bool RevealOperation(bool onlyJudging) => GlobeRevealer.RevealOnceForAll(Projectile, GetModItemDummy(), onlyJudging);
    /// <summary>
    /// 用于在世界生成时没记录数据的额外检测，因为是可选就virtual了
    /// </summary>
    public virtual void ExtraCheckWhenNotRecorded() { }
    public abstract Point16[] Positions { get; }
    public virtual Point16[] PositionsAnother => null;
}