using ImproveGame.Common.Configs;
using ImproveGame.Core;
using System.Collections;
using Terraria.DataStructures;

namespace ImproveGame.Content.Patches.Summon;

[Autoload(Side = ModSide.Client)] // 原版召唤都是在客户端运行的
public class AutoSummonLogic : ModPlayer
{
    private static bool Enabled => !ModLoader.HasMod("SummonersAssociation") && UIConfigs.Instance.AutoSummon;

    private static readonly CoroutineRunner SummonRunner = new();
    private int _lastUsedStaffType;
    private int _lastUsedStaffMinion;

    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        // 至少我们能适配灾厄
        if (Player.altFunctionUse != 2 && ProjectileID.Sets.MinionTargettingFeature[type] &&
            ContentSamples.ProjectilesByType[type].minion &&
            (_lastUsedStaffType != item.type || _lastUsedStaffMinion != type))
        {
            _lastUsedStaffType = item.type;
            _lastUsedStaffMinion = type;
            AddNotification($"复活后自动使用的召唤杖已设置为{item.Name}", default, item.type);
        }

        return base.Shoot(item, source, position, velocity, type, damage, knockback);
    }

    public override void PreUpdate()
    {
        if (Main.myPlayer != Player.whoAmI)
            return;
        SummonRunner.Update(1);
    }

    public override void OnRespawn()
    {
        if (!Enabled || Main.myPlayer != Player.whoAmI || !LocalPlayerHasItemFast(_lastUsedStaffType))
            return;

        SummonRunner.StopAll();

        // 还是要遍历，因为要正确处理词缀，找到正确的物品
        var allItems = GetAllInventoryItemsList(Player);
        Player.slotsMinions = 0f;
        foreach (var item in allItems.Where(item => item.type == _lastUsedStaffType))
        {
            float slotsPerSpawn = ItemID.Sets.StaffMinionSlotsRequired[item.type];
            int iterationTimes = (int)(Player.maxMinions / slotsPerSpawn);

            SummonRunner.Run(SummonMinions(item, iterationTimes));
            break;
        }
    }

    IEnumerator SummonMinions(Item item, int iterationTimes)
    {
        for (int i = 0; i < iterationTimes; i++)
        {
            var mouseX = Main.mouseX;
            var mouseY = Main.mouseY;
            Main.mouseX = (int)(Player.Center.X - Main.screenPosition.X);
            Main.mouseY = (int)(Player.Top.Y - Main.screenPosition.Y) - 32;

            Player.FreeUpPetsAndMinions(item);
            Player.ItemCheck_Shoot(Player.whoAmI, item, Player.GetWeaponDamage(item));
            Player.ItemCheck_ApplyPetBuffs(item);

            Main.mouseX = mouseX;
            Main.mouseY = mouseY;
            yield return 1;
        }

        AddNotification($"召唤了{iterationTimes}次{Lang.GetProjectileName(_lastUsedStaffMinion).Value}", Color.Pink, item.type);
    }
}