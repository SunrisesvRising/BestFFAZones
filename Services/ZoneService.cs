using ProjectM.Terrain;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

namespace BestFFAZones.Services
{
    public class GameZone
    {
        public string Name { get; set; }
        public Entity Entity { get; set; }
        public List<float2> Vertices { get; set; } = new();

        public bool ContainsPoint(float px, float pz)
        {
            int n = Vertices.Count;
            if (n < 3) return false;

            bool inside = false;
            int j = n - 1;

            for (int i = 0; i < n; i++)
            {
                float xi = Vertices[i].x, zi = Vertices[i].y;
                float xj = Vertices[j].x, zj = Vertices[j].y;

                if (((zi > pz) != (zj > pz)) && (px < (xj - xi) * (pz - zi) / (zj - zi) + xi))
                    inside = !inside;

                j = i;
            }

            return inside;
        }
    }

    public static class ZoneService
    {
        private static List<GameZone> _zones = null;

        public static void LoadZones()
        {
            _zones = new List<GameZone>();

            var em = Core.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<MapZoneData>(),
                ComponentType.ReadOnly<MapZonePolygonVertexElement>()
            );

            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                try
                {
                    var zoneData = em.GetComponentData<MapZoneData>(entity);
                    var vertexBuffer = em.GetBuffer<MapZonePolygonVertexElement>(entity);

                    var zone = new GameZone { Name = zoneData.Name.ToString(), Entity = entity };

                    for (int i = 0; i < vertexBuffer.Length; i++)
                        zone.Vertices.Add(vertexBuffer[i].VertexPos);

                    _zones.Add(zone);
                }
                catch (System.Exception e)
                {
                    Core.LogException(e);
                }
            }

            entities.Dispose();
            query.Dispose();

            Plugin.Logger.LogInfo($"[BestFFAZones] {_zones.Count} zone(s) loaded.");
        }

        public static IReadOnlyList<GameZone> GetAllZones() => _zones ?? new List<GameZone>();
        public static bool IsLoaded => _zones != null;

        public static GameZone GetZoneAtPosition(float px, float pz)
        {
            if (_zones == null) return null;
            foreach (var zone in _zones)
                if (zone.ContainsPoint(px, pz)) return zone;
            return null;
        }
    }
}