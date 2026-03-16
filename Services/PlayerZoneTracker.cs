using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using System.Collections.Generic;

namespace BestFFAZones.Services
{
    public static class PlayerZoneTracker
    {
        private static readonly Dictionary<int, string> _playerCurrentZone = new();

        public static void Tick()
        {
            if (!ZoneService.IsLoaded) return;

            var em = Core.EntityManager;

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCharacter>(),
                ComponentType.ReadOnly<Unity.Transforms.LocalTransform>(),
                ComponentType.ReadOnly<IsConnected>()
            );

            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var playerEntity in entities)
            {
                try
                {
                    var transform = em.GetComponentData<Unity.Transforms.LocalTransform>(playerEntity);
                    var playerChar = em.GetComponentData<PlayerCharacter>(playerEntity);
                    var networkId = em.GetComponentData<NetworkId>(playerEntity);
                    var userEntity = playerChar.UserEntity;

                    if (!em.Exists(userEntity)) continue;

                    int playerId = networkId.Normal_Index;
                    float px = transform.Position.x;
                    float pz = transform.Position.z;

                    var currentZone = ZoneService.GetZoneAtPosition(px, pz);
                    string currentZoneName = currentZone?.Name;

                    _playerCurrentZone.TryGetValue(playerId, out string previousZoneName);

                    if (currentZoneName == previousZoneName) continue;

                    // Transition dans le même groupe → on met à jour silencieusement
                    if (FfaConfigService.AreSameGroup(previousZoneName, currentZoneName))
                    {
                        _playerCurrentZone[playerId] = currentZoneName;
                        continue;
                    }

                    _playerCurrentZone[playerId] = currentZoneName;

                    // ── Entrée dans une zone ──────────────────────────────────────────
                    if (currentZoneName != null)
                    {
                        if (FfaConfigService.IsFfaZone(currentZoneName))
                        {
                            SendMessage(userEntity,
                                "<color=#ff4444>⚔ You have entered a FFA zone. All PvP is enabled!</color>");
                        }
                        else if (FfaConfigService.IsRiftT2Zone(currentZoneName))
                        {
                            SendMessage(userEntity,
                                "<color=#ff9900>⚠ This zone is FFA only during Rift T2 events. " +
                                "PvP will be enabled when a Rift T2 is active!</color>");
                        }
                    }

                    // ── Sortie d'une zone ─────────────────────────────────────────────
                    if (previousZoneName != null)
                    {
                        if (FfaConfigService.IsFfaZone(previousZoneName))
                        {
                            SendMessage(userEntity,
                                "<color=#00ff88>✔ You have left the FFA zone.</color>");
                        }
                        else if (FfaConfigService.IsRiftT2Zone(previousZoneName))
                        {
                            SendMessage(userEntity,
                                "<color=#aaaaaa>You have left the Rift T2 FFA zone.</color>");
                        }
                    }
                }
                catch (System.Exception e) { Core.LogException(e); }
            }

            entities.Dispose();
            query.Dispose();
        }

        private static void SendMessage(Entity userEntity, string message)
        {
            try
            {
                var user = Core.EntityManager.GetComponentData<User>(userEntity);
                var msg = new FixedString512Bytes(message);
                ServerChatUtils.SendSystemMessageToClient(Core.EntityManager, user, ref msg);
            }
            catch (System.Exception e) { Core.LogException(e); }
        }

        public static void RemovePlayer(int playerId) => _playerCurrentZone.Remove(playerId);
        public static string GetPlayerZone(int playerId) =>
            _playerCurrentZone.TryGetValue(playerId, out var z) ? z : null;
    }
}