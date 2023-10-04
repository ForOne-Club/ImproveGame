using Terraria.UI.Chat;

namespace ImproveGame.Content.NPCs.Dummy;

public class DummyDPS
{
    public static readonly int ResetInterval = 120;

    public bool IsStarted;
    public int SurvivalTimer;
    public int FirstHitTime;
    public int LastHitTime;
    public int LastDamage;
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
        LastDamage = damage;
        LastHitTime = SurvivalTimer;
    }

    public void Reset()
    {
        IsStarted = false;
        FirstHitTime = SurvivalTimer;
        LastHitTime = SurvivalTimer;
        TotalDamage = 0;
        LastDamage = 0;
    }

    public override string ToString()
    {
        float hitFrame = IsStarted ? Math.Max(1f, HitDuration) : 0f;
        float hitTick = MathF.Max(1f / 60f, hitFrame / 60f);
        string damagePerTick = IsStarted ? $"{MathF.Round(TotalDamage / hitTick)}/秒" : "已重置";
        string damagePerFrame = IsStarted ? $"{MathF.Round(TotalDamage / hitFrame, 1)}/帧" : "已重置";

        return $"伤害: {damagePerTick} {damagePerFrame} ({TotalDamage})\n" +
            $"时长: {Math.Round(hitTick, 1)}秒 {hitFrame}帧\n" +
            $"最后一击: {LastDamage}";
    }

    public void DrawString(Vector2 position, Vector2 percentOrign)
    {
        string @string = ToString();
        Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, @string, new(1f));

        MyUtils.DrawString(position, @string, Color.White, Color.Black, size * percentOrign, 1f, false);
    }
}
