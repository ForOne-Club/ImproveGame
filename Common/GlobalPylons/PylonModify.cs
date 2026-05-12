using ImproveGame.Common.Configs;

namespace ImproveGame.Common.GlobalPylons;

public class PylonModify : GlobalPylon
{
    /// <summary>
    /// 标记是否在运行HandleTeleportRequest，不要影响别的地方的InInteractionRange判定
    /// </summary>
    private bool _runningHandleTeleportRequest;

    public override void Load()
    {
        On_TeleportPylonsSystem.IsPlayerNearAPylon += On_TeleportPylonsSystem_IsPlayerNearAPylon;

        On_TeleportPylonsSystem.HandleTeleportRequest += (orig, self, info, index) =>
        {
            _runningHandleTeleportRequest = true;
            orig.Invoke(self, info, index);
            _runningHandleTeleportRequest = false;
        };

        On_Player.InTileEntityInteractionRange += (orig, self, x, y, sizeX, sizeY, settings) =>
        {
            if (_runningHandleTeleportRequest && ImproveConfigs.Instance.PylonTeleNoNear)
                return true;
            return orig.Invoke(self, x, y, sizeX, sizeY, settings);
        };
    }

    private bool On_TeleportPylonsSystem_IsPlayerNearAPylon(On_TeleportPylonsSystem.orig_IsPlayerNearAPylon orig,
        Player player)
    {
        if (ImproveConfigs.Instance.PylonTeleNoNear)
        {
            return true;
        }

        return orig(player);
    }

    public override bool? ValidTeleportCheck_PreAnyDanger(TeleportPylonInfo pylonInfo)
    {
        if (ImproveConfigs.Instance.PylonTeleNoDanger)
        {
            return true;
        }

        return base.ValidTeleportCheck_PreAnyDanger(pylonInfo);
    }

    public override bool? ValidTeleportCheck_PreBiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
    {
        if (ImproveConfigs.Instance.PylonTeleNoBiome)
        {
            return true;
        }

        return base.ValidTeleportCheck_PreBiomeRequirements(pylonInfo, sceneData);
    }

    public override bool? ValidTeleportCheck_PreNPCCount(TeleportPylonInfo pylonInfo, ref int defaultNecessaryNPCCount)
    {
        if (ImproveConfigs.Instance.PylonTeleNoNPC)
        {
            return true;
        }

        return base.ValidTeleportCheck_PreNPCCount(pylonInfo, ref defaultNecessaryNPCCount);
    }

    public override bool? PreCanPlacePylon(int x, int y, int tileType, TeleportPylonType pylonType)
    {
        if (ImproveConfigs.Instance.PylonPlaceNoRestriction)
            return true;
        return null;
    }
}