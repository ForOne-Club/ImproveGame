using ImproveGame.Core;

namespace ImproveGame.Content.Functions.VeinMiner;

public class VeinMinerSystem : ModSystem
{
    private bool VeinMiningEnabled =>
        Config.SimpleVeinMining && !ModLoader.HasMod("BAM") && !ModLoader.HasMod("OreExcavator");

    // 连锁挖矿可以关闭的提示每3min弹出一次
    private static int _popupTipTimer = 99999;
    private static bool _usingMiningTools;
    public static int MinerIndex;
    public static bool VeinMining => MinerIndex is not -1;

    private static readonly List<Point> NearbyPoints =
    [
        new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0)
    ];

    private static void EnqueueForNearbyOres(Point pos, Queue<Point> queue)
    {
        foreach (var nearbyPoint in NearbyPoints)
        {
            queue.Enqueue(pos + nearbyPoint);
        }
    }

    // 递推搜索同类物块
    private static void SearchOres(Point center, int oreType, ref HashSet<Point> ores)
    {
        var posQueue = new Queue<Point>();
        EnqueueForNearbyOres(center, posQueue);

        while (posQueue.TryDequeue(out var pos))
        {
            // 达到指标，下班！
            if (ores.Count >= 599)
                return;

            // 不在世界内
            if (!WorldGen.InWorld(pos.X, pos.Y, 2))
                continue;

            var tile = Main.tile[pos];
            int type = tile.type;
            // 不是同类物块
            if (type != oreType)
                continue;

            // 已经搜索过的话，就不加
            if (!ores.Add(pos))
                continue;

            EnqueueForNearbyOres(pos, posQueue);
        }
    }

    private static void CombineItems()
    {
        var player = Main.player[MinerIndex];
        var allCenterItems = new List<Item>();
        foreach (var activeItem in Main.ActiveItems)
        {
            if (activeItem.Center.Distance(player.Center) < 16 && activeItem.stack > 0)
                allCenterItems.Add(activeItem);
        }

        foreach (var item in allCenterItems)
        {
            if (!item.CanCombineStackInWorld() || item.stack >= item.maxStack)
                continue;

            foreach (var item2 in allCenterItems)
            {
                if (item2.type != item.type || item.shimmered != item2.shimmered || item2.stack <= 0 ||
                    item2.playerIndexTheItemIsReservedFor != item.playerIndexTheItemIsReservedFor)
                    continue;

                if (!ItemLoader.CanStack(item, item2))
                    continue;

                if (!ItemLoader.CanStackInWorld(item, item2))
                    continue;

                ItemLoader.StackItems(item, item2, out _);

                if (item2.stack <= 0)
                {
                    item2.SetDefaults();
                    item2.active = false;
                }
            }
        }
    }

    private static void BreakAllOres(List<Point> positions)
    {
        int count = 0;
        foreach (var pos in positions)
        {
            WorldGen.KillTile(pos.X, pos.Y);

            // 平滑斜坡，美观至上！
            Tile.SmoothSlope(pos.X, pos.Y, applyToNeighbors: true, sync: true);

            // 每30个方块合并一次物品
            if (++count % 30 == 0)
                CombineItems();
        }
    }

    private static void DoPopupTip()
    {
        // 3min一次提示
        if (_popupTipTimer < 60 * 60 * 3)
            return;

        _popupTipTimer = 0;
        AddNotification(GetText("Configs.ImproveConfigs.SimpleVeinMining.PopupTip"), itemIconType: ItemID.IronPickaxe);
    }

    public override void Load()
    {
        // 此方法在双端运行
        On_Player.PickTile += (orig, self, x, y, power) =>
        {
            var tile = Main.tile[x, y];
            int type = tile.type;

            bool isNotOre = (TileID.Sets.IsAContainer[type] || !Main.tileShine2[type]) || (!TileID.Sets.Ore[type] &&
                Main.tileOreFinderPriority[type] <= 0 && !Main.tileSpelunker[type]) || Main.tileContainer[tile.type];
            if (!_usingMiningTools || isNotOre)
            {
                orig.Invoke(self, x, y , power);
                return;
            }

            // 设置状态
            MinerIndex = self.whoAmI;

            bool tileActiveOld = tile.HasTile;
            orig.Invoke(self, x, y , power);
            bool tileActiveNow = tile.HasTile;

            // 如果破坏了方块
            if (!tileActiveNow && tileActiveOld)
            {
                var ores = new HashSet<Point>(66);
                var pos = new Point(x, y);
                SearchOres(pos, type, ref ores);
                BreakAllOres(ores.ToList());
                DoPopupTip();
            }

            MinerIndex = -1;
        };

        On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += (
            On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player self, Item item, out bool walls,
            int i, int j) =>
        {
            if (!VeinMiningEnabled)
            {
                orig.Invoke(self, item, out walls, i, j);
                return;
            }

            _usingMiningTools = true;
            orig.Invoke(self, item, out walls, i, j);
            _usingMiningTools = false;
        };
    }

    public override void PreUpdateTime()
    {
        _popupTipTimer++;
    }
}