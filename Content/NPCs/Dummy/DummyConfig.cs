using ImproveGame.Attributes;
using Terraria.ModLoader.Config;

namespace ImproveGame.Content.NPCs.Dummy;

public struct DummyConfig
{
    const string key = "$Mods.ImproveGame.UI.DummyConfiguration.AIType.";

    public enum AIType
    {
        [LabelKey($"{key}Default")]
        Default = -1,
        [LabelKey($"{key}Slime")]
        Slime = 1,
        [LabelKey($"{key}EvilEye")]
        EvilEye = 2,
        [LabelKey($"{key}Soilder")]
        Soilder = 3,
        [LabelKey($"{key}GoldenFish")]
        GoldenFish = 16,
        [LabelKey($"{key}JellyFish")]
        JellyFish = 18,
        [LabelKey($"{key}HugeMimic")]
        HugeMimic = 87,
        //SelfDefine = 124
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
    [Annotate]
    public AIType AIStyle = AIType.Default;
    public DummyConfig() { }
}
