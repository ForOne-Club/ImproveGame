namespace ImproveGame.Common.GlobalPylons
{
    public class PylonModify : GlobalPylon
    {
        public override void Load()
        {
            On_TeleportPylonsSystem.IsPlayerNearAPylon += On_TeleportPylonsSystem_IsPlayerNearAPylon;
        }

        private bool On_TeleportPylonsSystem_IsPlayerNearAPylon(On_TeleportPylonsSystem.orig_IsPlayerNearAPylon orig, Player player)
        {
            if (Config.PylonTeleNoNear)
            {
                return true;
            }
            return orig(player);
        }
        public override bool? ValidTeleportCheck_PreAnyDanger(TeleportPylonInfo pylonInfo)
        {
            if (Config.PylonTeleNoDanger)
            {
                return true;
            }
            return base.ValidTeleportCheck_PreAnyDanger(pylonInfo);
        }
        public override bool? ValidTeleportCheck_PreBiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            if (Config.PylonTeleNoBiome)
            {
                return true;
            }
            return base.ValidTeleportCheck_PreBiomeRequirements(pylonInfo, sceneData);
        }
        public override bool? ValidTeleportCheck_PreNPCCount(TeleportPylonInfo pylonInfo, ref int defaultNecessaryNPCCount)
        {
            if (Config.PylonTeleNoNPC)
            {
                return true;
            }
            return base.ValidTeleportCheck_PreNPCCount(pylonInfo, ref defaultNecessaryNPCCount);
        }
        public override bool? PreCanPlacePylon(int x, int y, int tileType, TeleportPylonType pylonType)
        {
            bool amount = Config.PylonPlaceNoAmount, biome = Config.PylonPlaceNoBiome;
            if (amount && biome)
            {
                return true;
            }
            if (biome)
            {
                if(Main.PylonSystem.HasPylonOfType(pylonType))
                {
                    return null;
                }
            }
            if (amount)
            {
                TeleportPylonInfo info = new()
                {
                    PositionInTiles = new(x, y),
                    TypeOfPylon = pylonType,
                };
                var scene = Main.PylonSystem._sceneMetrics;
                if (TileLoader.GetTile(tileType) is ModPylon pylon)
                {
                    if (pylon.ValidTeleportCheck_BiomeRequirements(info, scene))
                    {
                        return true;
                    }
                }
                switch (pylonType)
                {
                    case TeleportPylonType.SurfacePurity:
                        {
                            bool flag = info.PositionInTiles.Y <= Main.worldSurface;
                            if (Main.remixWorld)
                                flag = info.PositionInTiles.Y > Main.rockLayer && info.PositionInTiles.Y < Main.maxTilesY - 350;

                            bool flag2 = info.PositionInTiles.X >= Main.maxTilesX - 380 || info.PositionInTiles.X <= 380;
                            if (!flag || flag2)
                                return false;

                            if (scene.EnoughTilesForJungle || scene.EnoughTilesForSnow || scene.EnoughTilesForDesert || scene.EnoughTilesForGlowingMushroom || scene.EnoughTilesForHallow || scene.EnoughTilesForCrimson || scene.EnoughTilesForCorruption)
                                return false;

                            return true;
                        }
                    case TeleportPylonType.Jungle:
                        return scene.EnoughTilesForJungle;
                    case TeleportPylonType.Snow:
                        return scene.EnoughTilesForSnow;
                    case TeleportPylonType.Desert:
                        return scene.EnoughTilesForDesert;
                    case TeleportPylonType.Beach:
                        {
                            bool flag3 = info.PositionInTiles.Y <= Main.worldSurface && info.PositionInTiles.Y > Main.worldSurface * 0.3499999940395355;
                            bool flag4 = info.PositionInTiles.X >= Main.maxTilesX - 380 || info.PositionInTiles.X <= 380;
                            if (Main.remixWorld)
                            {
                                flag3 |= info.PositionInTiles.Y > Main.rockLayer && info.PositionInTiles.Y < Main.maxTilesY - 350;
                                flag4 |= info.PositionInTiles.X < Main.maxTilesX * 0.43 || info.PositionInTiles.X > Main.maxTilesX * 0.57;
                            }

                            return flag4 && flag3;
                        }
                    case TeleportPylonType.GlowingMushroom:
                        if (Main.remixWorld && info.PositionInTiles.Y >= Main.maxTilesY - 200)
                            return false;
                        return scene.EnoughTilesForGlowingMushroom;
                    case TeleportPylonType.Hallow:
                        return scene.EnoughTilesForHallow;
                    case TeleportPylonType.Underground:
                        return info.PositionInTiles.Y >= Main.worldSurface;
                    case TeleportPylonType.Victory:
                        return true;
                }
            }
            return null;
        }
    }
}
