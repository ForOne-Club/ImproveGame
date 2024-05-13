using ImproveGame.Common.Configs;
using ImproveGame.Common.ModHooks;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content;
using ImproveGame.Core;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameContent.UI.Chat;

namespace ImproveGame.Common.GlobalItems;

public class ImproveItem : GlobalItem, IItemOverrideHover, IItemMiddleClickable
{
    public override void SetDefaults(Item item)
    {
        // 最大堆叠
        if (item.maxStack > 1 && Config.ItemMaxStack > item.maxStack && item.DamageType != DamageClass.Melee &&
            !ItemID.Sets.CommonCoin[item.type])
        {
            item.maxStack = Config.ItemMaxStack;
        }

        // 使用速度 → 15
        // 生命水晶、魔力水晶、生命果
        if (item.type is ItemID.LifeCrystal or ItemID.ManaCrystal or ItemID.LifeFruit)
        {
            item.autoReuse = true;
            item.useTime = item.useAnimation = 15;
        }

        // 任务鱼可堆叠
        if (Config.QuestFishStack && Main.anglerQuestItemNetIDs.Contains(item.netID))
        {
            item.uniqueStack = false;
            item.maxStack = Config.ItemMaxStack;
        }
    }

    public override float UseTimeMultiplier(Item item, Player player)
    {
        if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || item.type is ItemID.WireCutter)
            return 1f - Config.ExtraToolSpeed;
        return 1f;
    }

    public override void Load()
    {
        // 锤子敲背景墙有特殊处理，要用On才能应用工具速度提升
        On_Player.ItemCheck_UseMiningTools_TryHittingWall += (orig, player, item, x, y) =>
        {
            orig.Invoke(player, item, x, y);

            // 检测那一堆if判断成没成
            if (player.itemTime == item.useTime / 2)
                player.itemTime = (int)Math.Max(1, (1f - Config.ExtraToolSpeed) * (item.useTime / 2f));
        };
    }

    public override void UseAnimation(Item item, Player player)
    {
        if (item.type is ItemID.MagicMirror or ItemID.CellPhone or ItemID.IceMirror or ItemID.Shellphone
                or ItemID.ShellphoneOcean or ItemID.ShellphoneHell or ItemID.ShellphoneSpawn or ItemID.MagicConch
                or ItemID.DemonConch && UIConfigs.Instance.MagicMirrorInstantTp)
        {
            player.SetItemTime(CombinedHooks.TotalUseTime(item.useTime, player, item));
            player.itemTime = player.itemTimeMax / 2 + 4;
        }
    }

    // Ju 2022.6.27: 去掉 ImprovePlayer 的额外速度，工具速度提升方式：减少工具使用间隔。（并非为提升工具速度）
    // Cyrilly 2023.7.17: 由于1.4.4这种写法会使再生之斧无法正常使用，删去了，换成UseTimeMultiplier
    /*
    private void ActuallyUseMiningTool(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After,
                i => i.Match(OpCodes.Ldarg_0),
                i => i.Match(OpCodes.Ldarg_1),
                i => i.Match(OpCodes.Ldloc_0)))
            return;
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<Player>>((player) =>
        {
            player.SetItemTime((int)MathHelper.Max(1, MathF.Round(player.itemTime * (1f - Config.ExtraToolSpeed))));
        });
    }

    private void TryHittingWall(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After,
                i => i.Match(OpCodes.Div),
                i => i.Match(OpCodes.Stfld)))
            return;
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<Player>>((player) =>
        {
            player.SetItemTime((int)MathHelper.Max(1, MathF.Round(player.itemTime * (1f - Config.ExtraToolSpeed))));
        });
    }
    */

    // 物品消耗
    public override bool ConsumeItem(Item item, Player player)
    {
        // 所有召唤物不会消耗
        if (Config.NoConsume_SummonItem &&
            (Lookups.BossSummonItems.Contains(item.type) || Lookups.EventSummonItems.Contains(item.type)))
            return false;

        // 魔杖 材料 999 不消耗
        if ((((item.createTile >= TileID.Dirt || item.createWall > WallID.None) && item.stack >= 999) ||
             ((item.createTile is TileID.WorkBenches or TileID.Chairs or TileID.Beds) && item.stack >= 99))
            && ModItemID.NoConsumptionItems.Contains(player.HeldItem.type))
            return !Config.WandMaterialNoConsume;

        // 抛射物不消耗
        if (Config.NoConsume_Projectile && item.stack >= 3996 && item.shoot > ProjectileID.None)
            return false;

        return base.ConsumeItem(item, player);
    }

    public override bool CanBeConsumedAsAmmo(Item ammo, Item weapon, Player player)
    {
        if (Config.NoConsume_Ammo)
        {
            // 坠落之星特判
            if (ammo.stack >= 999 && ammo.type == ItemID.FallenStar)
                return false;
            if (ammo.stack >= 3996 && ammo.ammo > 0)
                return false;
        }

        return base.CanBeConsumedAsAmmo(ammo, weapon, player);
    }

    public void OnMiddleClicked(Item item)
    {
        var player = Main.LocalPlayer;
        if (player.mount.Active && player.mount._type == item.mountType)
        {
            player.mount.Dismount(player);
            return;
        }

        player.mount.SetMount(item.mountType, player);

        ItemLoader.UseItem(item, player);

        if (item.UseSound != null)
            SoundEngine.PlaySound(item.UseSound, player.Center);
    }

    public bool MiddleClickable(Item item)
    {
        var player = Main.LocalPlayer;
        if (player.frozen || player.tongued || player.webbed || player.stoned || player.gravDir is -1f ||
            player.dead || player.noItems)
            return false;
        return item.mountType != -1 && player.mount.CanMount(item.mountType, player) &&
               player.ItemCheck_CheckCanUse(item) && !MountID.Sets.Cart[item.mountType];
    }

    public void ManageHoverTooltips(Item item, List<TooltipLine> tooltips)
    {
        string text = (GetTextWith("Tips.MouseMiddleUse", new { ItemName = item.Name }));
        tooltips.Add(new TooltipLine(Mod, "MountQuickUse", text) { OverrideColor = Color.LightGreen });
    }

    // 中键功能
    public bool OverrideHover(Item[] inventory, int context, int slot)
    {
        ((IItemMiddleClickable)this).HandleHover(inventory, context, slot);
        return false;
    }

    // 额外提示
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        ((IItemMiddleClickable)this).HandleTooltips(item, tooltips);

        TooltipShimmer(item, tooltips);
        TooltipMoreData(item, tooltips);
    }

    private void TooltipShimmer(Item item, List<TooltipLine> tooltips)
    {
        if (!UIConfigs.Instance.ShowShimmerInfo) return;

        if (ItemID.Sets.CoinLuckValue[item.type] > 0)
        {
            tooltips.Add(new TooltipLine(Mod, "ShimmerResult", GetText("Tips.ShimmerIntoWithCoinLuck", ItemID.Sets.CoinLuckValue[item.type])) { OverrideColor = new Color(241, 175, 233) });
            return;
        }

        var items = RecipeSystem.ShimmerInto[item.type];
        int stackRequired = RecipeSystem.ShimmerIntoWithStack[item.type];
        if (items is null) return;

        string text = stackRequired is not 1
            ? GetText("Tips.ShimmerIntoWithStack", stackRequired)
            : GetText("Tips.ShimmerInto");
        items.ForEach(i => text += ItemTagHandler.GenerateTag(i));
        tooltips.Add(new TooltipLine(Mod, "ShimmerResult", text) { OverrideColor = new Color(241, 175, 233) });
    }

    private void TooltipMoreData(Item item, List<TooltipLine> tooltips)
    {
        // 更多信息
        if (!UIConfigs.Instance.ShowMoreData)
            return;

        tooltips.Add(new(Mod, "Quantity", "Quantity: " +
            CurrentFrameProperties.ExistItems.GetTotalStack(item.type)));

        tooltips.Add(new(Mod, "Value", $"Value: {item.value}"));
        tooltips.Add(new(Mod, "Rare", $"Rare: {item.rare}"));
        tooltips.Add(new(Mod, "Type", "Type: " + item.type));
        tooltips.Add(new(Mod, "useTime", "UseTime: " + item.useTime));
        tooltips.Add(new(Mod, "UseAnimation", "UseAnimation: " + item.useAnimation));

        if (item.shoot > ProjectileID.None)
        {
            tooltips.Add(new(Mod, "Shoot", "Shoot: " + item.shoot)); 
            tooltips.Add(new(Mod, "ShootSpeed", "ShootSpeed: " + item.shootSpeed));
        }

        if (item.ammo > ItemID.None)
        {
            tooltips.Add(new(Mod, "Ammo", "Ammo: " + item.ammo));
        }

        if (item.buffType > 0)
        {
            tooltips.Add(new(Mod, "BuffType", "BuffType: " + item.buffType));
            tooltips.Add(new(Mod, "BuffTime", "BuffTime: " + item.buffTime));
        }

        if (item.tileWand > -1)
        {
            tooltips.Add(new(Mod, "TileWand", "TileWand: " + item.tileWand));
        }

        if (item.createTile > -1)
        {
            tooltips.Add(new(Mod, "CreateTile", "CreateTile: " + item.createTile));
            if (item.placeStyle > 0)
                tooltips.Add(new(Mod, "PlaceStyle", "PlaceStyle: " + item.placeStyle));
        }

        if (item.createWall > -1)
        {
            tooltips.Add(new(Mod, "CreateWall", "CreateWall: " + item.createWall));
        }
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame,
        Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (ApplyBuffItem.IsItemAvailable(item))
        {
            BigBagItemSlot.OpenItemGlow(sb);
        }

        return true;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame,
        Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (ApplyBuffItem.IsItemAvailable(item))
        {
            sb.ReBegin(null, Main.UIScaleMatrix);
        }
    }
}