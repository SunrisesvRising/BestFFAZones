using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Entities;
using BestFFAZones.Services;

namespace BestFFAZones.Patches
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    public static class ServerBootstrapPatch
    {
        public static void Postfix(ServerBootstrapSystem __instance)
        {
            try
            {
                if (Core.hasInitialized) return;

                Core.hasInitialized = true;
                ZoneService.LoadZones();
                FfaConfigService.Load();
                Plugin.Logger.LogInfo("[BestFFAZones] Ready.");
            }
            catch (System.Exception e)
            {
                Core.LogException(e);
            }
        }
    }

    [HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.OnUpdate))]
    public static class GameFramePatch
    {
        private const float CHECK_INTERVAL = 1.0f;
        private static float _timer = 0f;

        public static void Postfix(TriggerPersistenceSaveSystem __instance)
        {
            try
            {
                if (!Core.hasInitialized) return;

                _timer += UnityEngine.Time.deltaTime;
                if (_timer < CHECK_INTERVAL) return;
                _timer = 0f;

                PlayerZoneTracker.Tick();
            }
            catch (System.Exception e)
            {
                Core.LogException(e);
            }
        }
    }
}