namespace ImproveGame.Content.Items.Globes;

public class AetherGlobe : GlobeBase
{
    public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Shimmer;

    public override bool NotFoundCheck() => StructureDatas.ShimmerPosition == default;
}

public class DungeonGlobe : GlobeBase
{
    public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Dungeon;
}

public class TempleGlobe : GlobeBase
{
    public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Temple;

    public override bool NotFoundCheck() => StructureDatas.TemplePosition == default;
}

public class PyramidGlobe : GlobeBase
{
    public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Pyramids;
}

public class FloatingIslandGlobe : GlobeBase
{
    public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.FloatingIslands;

    public override bool NotFoundCheck() =>
        StructureDatas.SkyHousePositions.Count is 0 && StructureDatas.SkyLakePositions.Count is 0;
}