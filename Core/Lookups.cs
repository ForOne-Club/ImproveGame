namespace ImproveGame.Core;

/// <summary>
/// 所有KeyValue对应的静态数据放到这里，比如Buff站及其对应Buff之类的，好找一点
/// </summary>
public static class Lookups
{
    // 特殊药水
    public static readonly List<int> SpecialPotions = new()
    {
        ItemID.RecallPotion,
        ItemID.TeleportationPotion,
        ItemID.WormholePotion,
        ItemID.PotionOfReturn
    };

    public static readonly List<PortableBuffTile> BuffTiles = new()
    {
        new PortableBuffTile(TileID.CatBast, -1, BuffID.CatBast), // 巴斯特雕像
        new PortableBuffTile(TileID.Campfire, -1, BuffID.Campfire), // 篝火
        new PortableBuffTile(TileID.Fireplace, -1, BuffID.Campfire), // 壁炉
        new PortableBuffTile(TileID.HangingLanterns, 9, BuffID.HeartLamp), // 红心灯笼
        new PortableBuffTile(TileID.HangingLanterns, 7, BuffID.StarInBottle), // 星星瓶
        new PortableBuffTile(TileID.Sunflower, -1, BuffID.Sunflower), // 向日葵
        new PortableBuffTile(TileID.AmmoBox, -1, BuffID.AmmoBox), // 弹药箱
        new PortableBuffTile(TileID.BewitchingTable, -1, BuffID.Bewitched), // 施法桌
        new PortableBuffTile(TileID.WarTable, -1, BuffID.WarTable), // 战争桌
        new PortableBuffTile(TileID.CrystalBall, -1, BuffID.Clairvoyance), // 水晶球
        new PortableBuffTile(TileID.SliceOfCake, -1, BuffID.SugarRush), // 蛋糕块
        new PortableBuffTile(TileID.SharpeningStation, -1, BuffID.Sharpened), // 利器站
        new PortableBuffTile(TileID.WaterCandle, -1, BuffID.WaterCandle), // 水蜡烛
        new PortableBuffTile(TileID.PeaceCandle, -1, BuffID.PeaceCandle), // 和平蜡烛
        new PortableBuffTile(TileID.ShadowCandle, -1, BuffID.ShadowCandle) // 暗影蜡烛
    };

    /// <summary>
    /// BOSS 召唤物
    /// </summary>
    public static readonly HashSet<int> BossSummonItems = new(new int[]
    {
        ItemID.SlimeCrown, // 史莱姆皇冠
        ItemID.SuspiciousLookingEye, // 可疑眼球
        ItemID.WormFood, // 世吞
        ItemID.BloodySpine, // 克脑
        ItemID.Abeemination, // 蜂后
        ItemID.DeerThing, // 鹿玩意儿
        ItemID.QueenSlimeCrystal, // 史莱姆皇后
        ItemID.MechanicalEye, // 双子
        ItemID.MechanicalWorm, // 毁灭者
        ItemID.MechanicalSkull, // 机械骷髅王
        ItemID.LihzahrdPowerCell, // 石巨人
        ItemID.CelestialSigil, // 天界符
        ItemID.MechdusaSummon // 机械美杜莎 - 奥库瑞姆剃刀
    });

    /// <summary>
    /// 特殊事件 召唤物
    /// </summary>
    public static readonly HashSet<int> EventSummonItems = new(new int[]
    {
        ItemID.BloodMoonStarter, // 血泪
        ItemID.GoblinBattleStandard, // 哥布林战旗
        ItemID.DD2ElderCrystal, // 埃特尼亚水晶
        ItemID.SnowGlobe, // 雪人军团
        ItemID.SolarTablet, // 日食
        ItemID.PirateMap, // 海盗地图
        ItemID.PumpkinMoonMedallion, // 南瓜月
        ItemID.NaughtyPresent, // 霜月
    });
}