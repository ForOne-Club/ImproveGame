using Terraria.UI.Chat;

namespace ImproveGame.Content.NPCs.Dummy;

public class DummyDPS
{
    public static readonly int ResetInterval = 120;

    public DummyNPC Parent;
    public bool IsStarted;
    public int SurvivalTimer;
    public int FirstHitTime;
    public int LastHitTime;
    public int FinalBlow;
    public int TotalDamage;
    public int HitDuration => LastHitTime - FirstHitTime;

    public void Update()
    {
        if (IsStarted)
        {
            SurvivalTimer++;
        }

        if (IsStarted && SurvivalTimer - LastHitTime > ResetInterval)
        {
            IsStarted = false;
            Reset();
        }
    }

    public void Hurt(int damage)
    {
        IsStarted = true;

        TotalDamage += damage;
        FinalBlow = damage;
        LastHitTime = SurvivalTimer;
    }

    public void Reset()
    {
        IsStarted = false;
        FirstHitTime = SurvivalTimer;
        LastHitTime = SurvivalTimer;
        TotalDamage = 0;
        FinalBlow = 0;
        Parent?.ClearBuffs();
    }

    public override string ToString()
    {
        float hitFrame = IsStarted ? Math.Max(1f, HitDuration) : 0f;
        float hitTick = MathF.Max(1f / 60f, hitFrame / 60f);

        return $"{GetTextWith("NPC.Dummy_Damage",
            new
            {
                S = IsStarted ? MathF.Round(TotalDamage / hitTick) : 0,
                F = IsStarted ? MathF.Round(TotalDamage / hitFrame, 1) : 0
            })} ({TotalDamage})\n" +
            $"{GetTextWith("NPC.Dummy_Time", new { S = Math.Round(hitTick, 1), F = hitFrame })}\n" +
            $"{GetTextWith("NPC.Dummy_FinalBlow", new { FinalBlow })}";
    }

    public void DrawString(Vector2 position, Vector2 percentOrign)
    {
        string @string = ToString();
        Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, @string, new(1f));

        MyUtils.DrawString(position, @string, Color.White, Color.Black, size * percentOrign, 1f, false);
    }
}
