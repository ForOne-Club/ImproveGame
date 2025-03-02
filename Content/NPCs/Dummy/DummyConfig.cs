using ImproveGame.Attributes;
using Terraria.ModLoader.Config;

namespace ImproveGame.Content.NPCs.Dummy;

public struct DummyConfig
{
    const string key = "$Mods.ImproveGame.UI.DummyConfiguration.AIType.";

    public enum AIType
    {
        [LabelKey($"{key}Default")]
        Default = 0,
        [LabelKey($"{key}Slime")]
        Slime = NPCID.BlueSlime,
        [LabelKey($"{key}EvilEye")]
        EvilEye = NPCID.DemonEye,
        [LabelKey($"{key}Soilder")]
        Soilder = NPCID.PossessedArmor,
        [LabelKey($"{key}GoldenFish")]
        GoldenFish = NPCID.Goldfish,
        [LabelKey($"{key}JellyFish")]
        JellyFish = NPCID.BlueJellyfish,
        [LabelKey($"{key}HugeMimic")]
        HugeMimic = NPCID.BigMimicCorruption,
        [LabelKey($"{key}SelfDefine")]
        SelfDefine = 688
    }
    [Annotate]
    public bool LockHP = true;
    [Annotate]
    public int LifeMax = 2000000000;
    [Annotate]
    public int Defense = 0;
    [Annotate]
    public int Damage = 0;
    [Annotate]
    public float Scale = 1f;
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

    public int customAIType = 0;
    public DummyConfig() { }
}
