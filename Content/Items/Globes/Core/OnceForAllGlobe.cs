namespace ImproveGame.Content.Items.Globes.Core;

/// <summary>
/// 世界生成时就保存了数据，而且不会变的球，使用一次可以解锁全部对应数据
/// </summary>
public abstract class OnceForAllGlobe () : Globe(ItemRarityID.Quest, Item.sellPrice(silver: 10))
{
    public abstract StructureDatas.UnlockID StructureType { get; }

    public virtual bool NotFoundCheck() => false;

    public override bool RevealOperation(Projectile projectile, bool onlyJudging)
    {
        if (StructureType is StructureDatas.UnlockID.Pyramids && StructureDatas.PyramidPositions.Count is 0)
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(this.GetLocalizedValue("NotFound"), Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (NotFoundCheck())
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(GetLocalizedText("NotFound").Value, Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (StructureDatas.StructuresUnlocked[(byte)StructureType])
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(GetLocalizedText("AlreadyRevealed").Value, Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (onlyJudging)
            return true;

        StructureDatas.StructuresUnlocked[(byte)StructureType] = true;
        var text = Language.GetText("Mods.ImproveGame.Items.GlobeBase.Reveal")
            .WithFormatArgs(this.GetLocalizedValue("BiomeName"), Main.player[projectile.owner].name);
        AddNotification(text.Value, Color.Pink);

        return true;
    }
}