using ImproveGame.Attributes;

namespace ImproveGame.Content.NPCs.Dummy;

public struct DummyConfig
{
    public enum AIType
    {
        Default = -1,
        Slime = 1,
        EvilEye = 2,
        Soilder = 3,
        GoldenFish = 16,
        JellyFish = 18,
        HugeMimic = 87,
        SelfDefine = 124
    }
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
    //[Annotate]
    //public AIType AIStyle = AIType.Default;
    public DummyConfig() { }
}
