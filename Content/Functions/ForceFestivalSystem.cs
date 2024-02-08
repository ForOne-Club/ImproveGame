using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions;

public class ForceFestivalSystem : ModSystem
{
    internal static bool ForceHalloween;
    internal static bool ForceXMas;

    public override void ClearWorld()
    {
        ForceHalloween = false;
        ForceXMas = false;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        BitsByte festivalBytes = tag.GetByte("Festival");
        ForceHalloween = festivalBytes[0];
        ForceXMas = festivalBytes[1];
    }

    public override void SaveWorldData(TagCompound tag)
    {
        var festivalBytes = new BitsByte(ForceHalloween, ForceXMas);
        tag["Festival"] = (byte)festivalBytes;
    }

    public override void PostUpdateTime()
    {
        Main.halloween |= ForceHalloween;
        Main.xMas |= ForceXMas;
    }
}