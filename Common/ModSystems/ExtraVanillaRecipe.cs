using System.Text;
using Terraria.GameContent.ItemDropRules;

namespace ImproveGame.Common.ModSystems
{
    public static class RecipeExt
    {
        private static readonly string modName = nameof(ImproveGame);
        private const string Bar = "Bars";
        internal static Dictionary<int, RecipeGroup> bars;
        private static RecipeGroup RegisterPairsBars(int index, int bar1, int bar2)
        {
            string key = new StringBuilder(modName).Append(':').Append(Bar).Append(index).ToString();
            RecipeGroup rg = new(() => Language.GetTextValue(key), bar1, bar2);
            RecipeGroup.RegisterGroup(key, rg);
            return rg;
        }
        public static void AddRecipeGroups()
        {
            bars = [];
            bars[1] = RegisterPairsBars(1, ItemID.CopperBar, ItemID.TinBar);
            bars[2] = RegisterPairsBars(2, ItemID.IronBar, ItemID.LeadBar);
            bars[3] = RegisterPairsBars(3, ItemID.SilverBar, ItemID.TungstenBar);
            bars[4] = RegisterPairsBars(4, ItemID.GoldBar, ItemID.PlatinumBar);
            bars[5] = RegisterPairsBars(5, ItemID.DemoniteBar, ItemID.CrimtaneBar);
            bars[7] = RegisterPairsBars(7, ItemID.CobaltBar, ItemID.PalladiumBar);
            bars[8] = RegisterPairsBars(8, ItemID.MythrilBar, ItemID.OrichalcumBar);
            bars[9] = RegisterPairsBars(9, ItemID.AdamantiteBar, ItemID.TitaniumBar);
        }
        public static Recipe Anvils(this Recipe recipe) => recipe.AddTile(TileID.Anvils);
        /// <summary>
        /// 012熔炉 地狱熔炉 精金熔炉
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Recipe Forge(this Recipe recipe, int level = 0)
        {
            return recipe.AddTile(level switch
            {
                2 => TileID.AdamantiteForge,
                1 => TileID.Hellforge,
                _ => TileID.Furnaces
            });
        }

        public static Recipe WorkBenches(this Recipe recipe) => recipe.AddTile(TileID.WorkBenches);
        public static Recipe Tinker(this Recipe recipe) => recipe.AddTile(TileID.TinkerersWorkbench);
        public static Recipe Altar(this Recipe recipe) => recipe.AddTile(TileID.DemonAltar);
        /// <summary>
        /// 1铜 2铁 3银 4金 5陨石 6邪恶 7狱岩
        /// <br/> high  1钴蓝 2秘银 3精金 4神圣 5绿藻 6夜明
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="level"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static Recipe Bars(this Recipe recipe, int level, int stack = 1, bool high = false)
        {
            if (high)
                level += 4;
            if (bars.TryGetValue(level, out var rg))
            {
                return recipe.AddRecipeGroup(rg, stack);
            }
            int bar = level switch
            {
                7 => ItemID.HellstoneBar,
                11 => ItemID.HallowedBar,
                12 => ItemID.ChlorophyteBar,
                13 => ItemID.LunarBar,
                _ => 0
            };
            return recipe.AddIngredient(bar, stack);
        }
        public static Recipe Woods(this Recipe recipe, int stack) => recipe.AddRecipeGroup(RecipeGroupID.Wood, stack);
    }

    public class ExtraVanillaRecipe : ModSystem
    {
        public override void AddRecipeGroups() => RecipeExt.AddRecipeGroups();
        public override void AddRecipes()
        {
            foreach (Recipe recipe in Exchange().Concat(ChestLoot()).Concat(NPCDrop()))
            {
                recipe.DisableDecraft().Register();
            }
        }
        public override void PostSetupContent()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.RottenChunk] = ItemID.Vertebrae;
            ItemID.Sets.ShimmerTransformToItem[ItemID.Vertebrae] = ItemID.RottenChunk;
        }
        private static Recipe Create(int itemid, int stack = 1) => Recipe.Create(itemid, stack);
        private static IEnumerable<Recipe> Exchange()
        {
            Dictionary<int, int> exchange = [];
            void Add(int start)
            {
                int ore = start;
                for (int i = 0; i < 5; i++)
                {
                    int shimmer = ItemID.Sets.ShimmerTransformToItem[ore];
                    exchange[ore] = 0;
                    exchange[shimmer] = 0;
                    ore = shimmer;
                }
            }
            Add(1106);// 钛金矿
            Add(702);// 铂金矿
            foreach (Recipe recipe in Main.recipe)
            {
                if (!recipe.Disabled && recipe.requiredItem.Count == 1)
                {
                    int ore = recipe.requiredItem[0].type;
                    if (exchange.ContainsKey(ore))
                    {
                        exchange[ore] = recipe.createItem.type;
                    }
                }
            }
            exchange.Add(ItemID.DemoniteOre, ItemID.DemoniteBar);
            exchange.Add(ItemID.CrimtaneOre, ItemID.CrimtaneBar);
            int count = exchange.Count;
            int[] ores = [.. exchange.Keys];
            int[] bars = [.. exchange.Values];
            for (int i = 0; i < count; i += 2)
            {
                int ore1 = ores[i], ore2 = ores[i + 1], bar1 = bars[i], bar2 = bars[i + 1];
                int forge = i <= 6 ? 2 : 0;
                yield return Create(ore1).AddIngredient(ore2).Forge(forge);
                yield return Create(ore2).AddIngredient(ore1).Forge(forge);
                yield return Create(bar1).AddIngredient(bar2).Forge(forge);
                yield return Create(bar2).AddIngredient(bar1).Forge(forge);
            }
        }
        private static IEnumerable<Recipe> ChestLoot()
        {
            #region 地表
            yield return Create(ItemID.Spear)
                .Bars(1, 8)
                .Anvils();// 长矛
            yield return Create(ItemID.Blowpipe)
                .AddIngredient(ItemID.BambooBlock, 8)
                .WorkBenches();// 吹管
            yield return Create(ItemID.WoodenBoomerang)
                .AddIngredient(ItemID.Wood, 8)
                .Bars(1)
                .WorkBenches();// 木回旋镖
            yield return Create(ItemID.Aglet)
                .Bars(2, 5)
                .Anvils();// 金属扣链
            yield return Create(ItemID.ClimbingClaws)
                .Bars(2, 5);// 攀爬爪
            yield return Create(ItemID.Umbrella)
                .Bars(2, 2)
                .AddIngredient(ItemID.Silk, 5)
                .WorkBenches();// 伞
            yield return Create(ItemID.CordageGuide)
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.BambooBlock, 8)
                .WorkBenches();// 纤维宝典
            yield return Create(ItemID.WandofSparking)
                .Woods(3)
                .AddIngredient(ItemID.Fireblossom)
                .WorkBenches();// 火花魔棒
            yield return Create(ItemID.Radar)
                .Bars(2, 5)
                .Bars(4)
                .AddIngredient(ItemID.Glass, 5)
                .Anvils();// 雷达
            yield return Create(ItemID.PortableStool)
                .AddIngredient(ItemID.Wood, 6)
                .WorkBenches();// 梯凳
            yield return Create(ItemID.BabyBirdStaff)
                .Woods(3)
                .AddRecipeGroup(RecipeGroupID.Birds)
                .WorkBenches();// 雀杖
            yield return Create(ItemID.LivingWoodWand)
                .AddIngredient(ItemID.Wood, 10)
                .WorkBenches();//生命木魔棒，
            yield return Create(ItemID.LeafWand)
                .AddIngredient(ItemID.Wood, 10)
                .WorkBenches();//树叶魔棒
            yield return Create(ItemID.SunflowerMinecart)
                .Bars(2, 10)
                .AddIngredient(ItemID.Sunflower);//向日葵矿车，
            yield return Create(ItemID.LadybugMinecart)
                .Bars(2, 10)
                .AddIngredient(ItemID.LadyBug);//瓢虫矿车
            #endregion

            #region 洞穴
            yield return Create(ItemID.BandofRegeneration)
                .Bars(3, 5)
                .AddIngredient(ItemID.LifeCrystal)
                .Anvils();// 再生手环
            yield return Create(ItemID.MagicMirror)
                .Bars(3, 5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient(ItemID.FallenStar)
                .Anvils();// 魔镜
            yield return Create(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.Cloud, 15)
                .WorkBenches();// 云朵瓶
            yield return Create(ItemID.HermesBoots)
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.Leather, 3)
                .WorkBenches();// 赫尔墨斯靴
            yield return Create(ItemID.Mace)
                .Bars(2, 8)
                .Anvils();//链锤5011	
            yield return Create(ItemID.ShoeSpikes)
                .Bars(2, 5)
                .Anvils();// 鞋钉
            yield return Create(ItemID.FlareGun)
                .Bars(2, 3)
                .Bars(4, 5)
                .Anvils(); //信号枪
            yield return Create(ItemID.Extractinator)
                .Bars(1, 3)
                .Bars(2, 3)
                .Bars(3, 3)
                .Bars(4, 3)
                .Anvils(); //提炼机
            yield return Create(ItemID.LavaCharm)
                .Bars(7, 15)
                .AddIngredient(ItemID.Fireblossom, 3)
                .AddCondition(Condition.NearLava);// 熔岩护身符
            #endregion

            #region 地牢
            yield return Create(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 5)
                .Anvils();// 金钥匙
            yield return Create(ItemID.Muramasa)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 8)
                .AddIngredient(ItemID.ManaCrystal)
                .Anvils();// 村正
            yield return Create(ItemID.CobaltShield)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 8)
                .AddIngredient(ItemID.ManaCrystal)
                .Anvils();// 钴蓝盾
            yield return Create(ItemID.MagicMissile)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(3, 8)
                .AddIngredient(ItemID.ManaCrystal)
                .Anvils(); //魔法导弹
            yield return Create(ItemID.AquaScepter)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(3, 8)
                .AddIngredient(ItemID.Sapphire)
                .Anvils(); //海蓝权杖
            yield return Create(ItemID.BlueMoon)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 5)
                .Bars(2, 5)
                .AddIngredient(ItemID.Sapphire)
                .Anvils(); //蓝月
            yield return Create(ItemID.Handgun)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 8)
                .Bars(2, 8).Anvils(); //手枪
            yield return Create(ItemID.Valor)
                .AddCondition(Condition.DownedSkeletron)
                .AddIngredient(ItemID.SilkRope)
                .AddIngredient(ItemID.Bone, 10)
                .Anvils(); //英勇球
            yield return Create(ItemID.ShadowKey)
                .AddCondition(Condition.DownedSkeletron)
                .Bars(4, 10)
                .Bars(6, 10)
                .Anvils();// 暗影钥匙
            yield return Create(ItemID.BoneWelder)
                .Bars(6, 5)
                .AddIngredient(ItemID.Bone, 15)
                .Anvils();//骨头焊机
            #endregion

            #region 沙漠
            yield return Create(ItemID.SandstorminaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.SandBlock, 15);// 沙暴瓶
            yield return Create(ItemID.MagicConch)
                .AddIngredient(ItemID.Seashell)
                .AddIngredient(ItemID.Starfish)
                .AddIngredient(ItemID.Coral)
                .AddIngredient(ItemID.ManaCrystal)
                .WorkBenches();// 魔法海螺
            yield return Create(ItemID.AncientChisel)
                .AddIngredient(ItemID.FossilOre, 5)
                .Anvils();// 远古凿子
            yield return Create(ItemID.FlyingCarpet)
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Leather, 5)
                .WorkBenches(); //飞毯
            yield return Create(ItemID.ThunderSpear)
                .Woods(5)
                .AddIngredient(ItemID.FossilOre, 5)
                .Bars(3, 5)
                .Anvils(); //风暴长矛
            yield return Create(ItemID.ThunderStaff)
                .Woods(5)
                .AddIngredient(ItemID.FossilOre, 5)
                .AddIngredient(ItemID.Amber)
                .WorkBenches(); //霹雳法杖
            yield return Create(ItemID.MysticCoilSnake)
                .AddIngredient(ItemID.BambooBlock, 8)
                .WorkBenches(); //耍蛇者长笛
            yield return Create(ItemID.SandBoots)
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.SandBlock, 15)
                .WorkBenches();// 沙丘行者
            yield return Create(ItemID.CatBast)
                .Bars(1, 10)
                .AddIngredient(ItemID.FossilOre, 10)
                .Anvils(); //韧皮雕像
            yield return Create(ItemID.DesertMinecart)
                .Bars(2, 10)
                .AddIngredient(ItemID.FossilOre, 3)
                .Anvils();//沙漠矿车
            yield return Create(ItemID.EncumberingStone)
                .AddIngredient(ItemID.SandBlock, 15)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddIngredient(ItemID.FossilOre, 3)
                .Anvils(); //负重石
            #endregion

            #region 雪地
            yield return Create(ItemID.BlizzardinaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.SnowBlock, 15)
                .WorkBenches(); //暴雪瓶
            yield return Create(ItemID.FlurryBoots)
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.SnowBlock, 15)
                .WorkBenches(); //疾风雪靴
            yield return Create(ItemID.IceSkates)
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.IceBlock, 15)
                .WorkBenches(); ; //溜冰鞋
            yield return Create(ItemID.IceBoomerang)
                .Bars(4, 3)
                .AddIngredient(ItemID.IceBlock, 15)
                .Anvils(); //冰雪回旋镖
            yield return Create(ItemID.IceBlade)
                .Bars(4, 8)
                .AddIngredient(ItemID.IceBlock, 15)
                .Anvils(); //冰雪刃
            yield return Create(ItemID.SnowballCannon)
                .Bars(4, 8)
                .AddIngredient(ItemID.SnowBlock, 15)
                .Anvils();//雪球炮S，
            yield return Create(ItemID.Fish)
                .AddRecipeGroup(RecipeGroupID.FishForDinner, 5)
                .WorkBenches(); //鱼669
            #endregion

            #region 丛林
            yield return Create(ItemID.FeralClaws)
                .AddIngredient(ItemID.Leather, 5)
                .WorkBenches(); //猛爪手套
            yield return Create(ItemID.AnkletoftheWind)
                .Bars(3, 5)
                .AddIngredient(ItemID.Feather, 5)
                .Anvils(); //疾风脚镯
            yield return Create(ItemID.Boomstick)
                .Bars(2, 8)
                .Bars(4, 8)
                .Anvils(); //三发猎枪
            yield return Create(ItemID.StaffofRegrowth)
                .AddIngredient(ItemID.LifeCrystal)
                .AddIngredient(ItemID.RichMahogany, 15)
                .WorkBenches();//再生法杖，
            yield return Create(ItemID.FlowerBoots)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.ManaFlower)
                .WorkBenches();//花靴，
            yield return Create(ItemID.FiberglassFishingPole)
                .Bars(2, 5)
                .Bars(4, 3)
                .AddIngredient(ItemID.Glass, 5)
                .Anvils();//玻璃钢钓竿，
            yield return Create(ItemID.Seaweed)
                .AddIngredient(ItemID.JungleSpores, 5);//海草	，
            yield return Create(ItemID.LivingMahoganyWand)
                .AddIngredient(ItemID.RichMahogany, 10)
                .WorkBenches();//生命红木魔棒
            yield return Create(ItemID.LivingMahoganyLeafWand)
                .AddIngredient(ItemID.RichMahogany, 10)
                .WorkBenches();// 红木树叶魔棒，
            yield return Create(ItemID.BeeMinecart)
                .Bars(2, 10)
                .AddIngredient(ItemID.BottledHoney)
                .Anvils(); // 蜜蜂矿车，
            yield return Create(ItemID.HoneyDispenser)
                .AddIngredient(ItemID.BottledHoney, 5)
                .AddIngredient(ItemID.MudBlock, 15)
                .AddIngredient(ItemID.RichMahogany, 15)
                .AddIngredient(ItemID.JungleSpores, 5)
                .WorkBenches();// 蜂蜜分配机	
            #endregion

            #region 空岛
            yield return Create(ItemID.ShinyRedBalloon)
                .AddIngredient(ItemID.Leather, 3)
                .AddIngredient(ItemID.SilkRope)
                .WorkBenches(); //气球
            yield return Create(ItemID.CreativeWings)
                .AddIngredient(ItemID.Feather, 10)
                .WorkBenches(); //雏翼
            yield return Create(ItemID.Starfury)
                .Bars(4, 8)
                .AddIngredient(ItemID.FallenStar, 10)
                .Anvils();// 星怒
            yield return Create(ItemID.LuckyHorseshoe)
                .Bars(4, 15)
                .AddIngredient(ItemID.FallenStar)
                .Anvils();// 幸运马掌
            yield return Create(ItemID.CelestialMagnet)
                .Bars(2, 3)
                .Bars(3, 5)
                .AddIngredient(ItemID.ManaCrystal)
                .Anvils();// 天界磁石，
            yield return Create(ItemID.SkyMill)
                .AddIngredient(ItemID.SunplateBlock, 15)
                .WorkBenches(); // 天磨
            #endregion

            #region 海洋
            yield return Create(ItemID.Flipper)
                .Bars(2)
                .AddIngredient(ItemID.Leather, 5)
                .WorkBenches(); //脚蹼
            yield return Create(ItemID.FloatingTube)
                .AddIngredient(ItemID.Leather, 5)
                .WorkBenches(); //浮游圈
            yield return Create(ItemID.WaterWalkingBoots)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.WaterWalkingPotion)
                .WorkBenches(); //水上漂靴
            yield return Create(ItemID.BreathingReed)
                .AddIngredient(ItemID.PalmWood, 3)
                .WorkBenches();//芦苇呼吸管，
            yield return Create(ItemID.Trident)
                .Bars(1, 8)
                .Anvils();//三叉戟t，
            yield return Create(ItemID.SandcastleBucket)
                .AddIngredient(ItemID.SandBlock, 15)
                .AddIngredient(ItemID.EmptyBucket)
                .WorkBenches();//沙堡桶，
            yield return Create(ItemID.SharkBait)
                .AddIngredient(ItemID.ApprenticeBait, 5)
                .WorkBenches();//鲨鱼鱼饵	
            #endregion

            #region 地狱
            yield return Create(ItemID.ObsidianRose)
                .Bars(7, 5)
                .AddIngredient(ItemID.Obsidian, 5)
                .AddIngredient(ItemID.Fireblossom)
                .AddCondition(Condition.NearLava);// 黑曜石玫瑰
            yield return Create(ItemID.HellwingBow)
                .Bars(7, 8)
                .AddIngredient(ItemID.HellButterfly)
                .Anvils();// 蝶狱弓
            yield return Create(ItemID.TreasureMagnet)
                .Bars(5, 5)
                .Bars(7, 5)
                .Anvils(); //宝藏磁铁
            yield return Create(ItemID.Sunfury)
                .Bars(7, 8)
                .Anvils();//阳炎之怒，
            yield return Create(ItemID.UnholyTrident)
                .Bars(6, 8)
                .AddCondition(Condition.RemixWorld)
                .Anvils();//邪恶三叉戟/
            yield return Create(ItemID.FlowerofFire)
                .Bars(7, 8)
                .AddCondition(Condition.NotRemixWorld)
                .Anvils();//火之花	（颠倒），
            yield return Create(ItemID.Flamelash)
                .Bars(7, 8)
                .Anvils();//烈焰火鞭，
            yield return Create(ItemID.DarkLance)
                .Bars(7, 8)
                .AddIngredient(ItemID.Obsidian, 5)
                .Anvils();//暗黑长枪
            yield return Create(ItemID.HellMinecart)
                .AddIngredient(ItemID.Obsidian, 15)
                .Anvils();//恶魔地狱矿车，
            yield return Create(ItemID.OrnateShadowKey)
                .Bars(6, 5)
                .AddIngredient(ItemID.Obsidian, 5)
                .Anvils();//华丽暗影钥匙	，
            yield return Create(ItemID.HellCake)
                .AddIngredient(ItemID.SliceOfCake)
                .AddIngredient(ItemID.Obsidian, 5)
                .WorkBenches();//地狱蛋糕块
            #endregion

            #region 其他
            yield return Create(ItemID.WebSlinger)
                .AddIngredient(ItemID.WebRope, 8)
                .WorkBenches();//蛛丝吊索
            #endregion
        }
        private static IEnumerable<Recipe> NPCDrop()
        {
            /*#region 海盗
            yield return Create(ItemID.);//钱币枪
            yield return Create(ItemID.);//幸运币
            yield return Create(ItemID.);//优惠卡
            yield return Create(ItemID.);//海盗法杖
            yield return Create(ItemID.);//幸运币
            #endregion

            #region 万圣节
            yield return Create(ItemID.);//利刃手套
            yield return Create(ItemID.);//血腥砍刀
            #endregion

            #region 血月
            yield return Create(ItemID.);//致胜炮hardmode
            #endregion

            #region 神圣
            yield return Create(ItemID.);//混沌传送杖
            #endregion

            #region 洞穴
            yield return Create(ItemID.);//骨之羽
            #endregion

            #region 地狱
            yield return Create(ItemID.);//喷流球
            #endregion

            #region 地牢
            yield return Create(ItemID.);//磁球，妖灵瓶，钥匙剑h,白骨魔棒
            #endregion

            #region 雪地
            yield return Create(ItemID.);//冰雪悠悠球
            #endregion

            #region 日蚀
            yield return Create(ItemID.);//断裂英雄剑
            #endregion*/
            for (int i = 1; i < NPCID.Count; i++)
            {
                int banner = Item.BannerToItem(Item.NPCtoBanner(i));
                if (banner > 0)
                {
                    foreach (var drs in Main.ItemDropsDB.GetRulesForNPCID(i, false))
                    {
                        List<DropRateInfo> drInfo = [];
                        drs.ReportDroprates(drInfo, new(1f));
                        foreach (var info in drInfo)
                        {
                            if (info.itemId > ItemID.Count) continue;
                            int count = Math.Clamp((int)(1f / info.dropRate / 50), 1, 10);
                            yield return Create(info.itemId, (info.stackMax + info.stackMin) / 2)
                                .AddIngredient(banner, count);
                        }
                    }
                }
            }
        }
        private static IEnumerable<Recipe> Another()
        {
            yield return Create(ItemID.GoldenBugNet); //金网兜
            yield return Create(ItemID.GoldenFishingRod); //金钓竿
            yield return Create(ItemID.HotlineFishingHook); //防熔岩钓竿
            yield return Create(ItemID.AnkhCharm); //十字护身符
            yield return Create(ItemID.VampireKnives); //吸血飞刀
            yield return Create(ItemID.ScourgeoftheCorruptor); //腐化灾兵
            yield return Create(ItemID.RainbowGun); //彩虹枪
            yield return Create(ItemID.RainbowGun); //食人鱼枪
            yield return Create(ItemID.StaffoftheFrostHydra); //寒霜九头蛇
            yield return Create(ItemID.StormTigerStaff); //沙漠虎杖
            yield return Create(ItemID.MagmaStone); //岩浆石
            yield return Create(ItemID.FireFeather); //火羽
            yield return Create(ItemID.GiantHarpyFeather); //巨型羽毛
            yield return Create(ItemID.IceFeather); //冰雪羽
            yield return Create(ItemID.DepthMeter); //深度计
            yield return Create(ItemID.Compass); //罗盘
            yield return Create(ItemID.LifeformAnalyzer); //生命体分析机
            yield return Create(ItemID.TallyCounter); //杀怪计数器
            yield return Create(ItemID.MetalDetector); //金属探测器
            yield return Create(ItemID.Stopwatch); //秒表
            yield return Create(ItemID.DPSMeter); //dps
            yield return Create(ItemID.FishermansGuide); //渔民宝典
            yield return Create(ItemID.WeatherRadio); //天气收音机
            yield return Create(ItemID.Sextant); //六分仪
        }
    }
}
