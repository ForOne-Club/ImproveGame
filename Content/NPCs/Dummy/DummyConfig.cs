using ImproveGame.Attributes;

namespace ImproveGame.Content.NPCs.Dummy;

public struct DummyConfig
{
    [Annotate]
    public bool LockHP = true;
    [Annotate]
    public int LifeMax = 200000;
    [Annotate]
    public int Defense = 0;
    [Annotate]
    public int Damage = 0;
    [Annotate]
    public bool ShowBox = true;
    [Annotate]
    public bool ShowDamageData = true;
    [Annotate]
    public bool ShowNameOnHover = false;
    [Annotate]
    public bool Immortal = false;
    [Annotate]
    public bool NoGravity = true;
    [Annotate]
    public bool NoTileCollide = true;
    [Annotate]
    public float KnockBackResist = 0f;

    public DummyConfig() { }
}
