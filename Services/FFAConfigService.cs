using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BepInEx;

namespace BestFFAZones.Services
{
    public class FfaConfig
    {
        public List<string> FfaZones { get; set; } = new();
        /// <summary>
        /// Zones FFA uniquement actives pendant un Rift T2.
        /// Message d'entrée différent : avertit que le FFA est lié au Rift.
        /// </summary>
        public List<string> RiftT2Zones { get; set; } = new();
        public List<List<string>> ZoneGroups { get; set; } = new();
    }

    public static class FfaConfigService
    {
        private static readonly string ConfigFolder = Path.Combine(Paths.ConfigPath, "BestFFAZones");
        private static readonly string ConfigFile = Path.Combine(ConfigFolder, "FfaZoneConfig.json");

        private static HashSet<string> _ffaZoneNames = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> _riftT2ZoneNames = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, int> _zoneGroupMap = new(StringComparer.OrdinalIgnoreCase);

        public static void Load()
        {
            try
            {
                if (!Directory.Exists(ConfigFolder))
                    Directory.CreateDirectory(ConfigFolder);

                if (!File.Exists(ConfigFile))
                    WriteDefaultConfig();

                var content = File.ReadAllText(ConfigFile);
                var config = JsonSerializer.Deserialize<FfaConfig>(content);

                _ffaZoneNames = new HashSet<string>(
                    config?.FfaZones ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase);

                _riftT2ZoneNames = new HashSet<string>(
                    config?.RiftT2Zones ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase);

                _zoneGroupMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                var groups = config?.ZoneGroups ?? new List<List<string>>();
                for (int i = 0; i < groups.Count; i++)
                    foreach (var z in groups[i])
                        _zoneGroupMap[z] = i;

                Plugin.Logger.LogInfo($"[BestFFAZones] {_ffaZoneNames.Count} FFA zone(s), {_riftT2ZoneNames.Count} Rift T2 zone(s), {groups.Count} group(s).");
            }
            catch (Exception e) { Core.LogException(e); }
        }

        public static bool IsFfaZone(string zoneName) => _ffaZoneNames.Contains(zoneName);
        public static bool IsRiftT2Zone(string zoneName) => _riftT2ZoneNames.Contains(zoneName);

        public static bool AreSameGroup(string zoneA, string zoneB)
        {
            if (zoneA == null || zoneB == null) return false;
            if (!_zoneGroupMap.TryGetValue(zoneA, out int a)) return false;
            if (!_zoneGroupMap.TryGetValue(zoneB, out int b)) return false;
            return a == b;
        }

        private static void WriteDefaultConfig()
        {
            var config = new FfaConfig
            {
                FfaZones = new List<string>
                {
                    // --- zones FFA permanentes (exemples existants) ---
                    "9f4bb828842244b9b17ce3d9fd39be6e",
                    "a59283ab08114eb99d930f544fd72cb3",
                    "5a9cec9e790347cfa3c8a567afe1f30b",
                    "737efd4800f347fb9f9727a151d1d038",
                    "f20338c8e2f24165865bcd25e528b084",
                    "4f1ebbbc58454419b2e476d65da49498",
                    "1e6477ec05cb414eb4b39dff87bc293d",
                    "45a43450e1334e6eae86c74f2706ac4c",
                    "de43f46cd3344d7a968d0932e8a689da",
                    "1c8627c360994c698672682d810ebc65",
                    "1bacf251032c44e2bb5689122e1d6240",
                    "7bc63b78e0934ee6aeb98df40a4279f4",
                    "00bfdf78f096415ab6ccd7c81d970f79"
                },
                // --- zones FFA uniquement pendant le Rift T2 ---
                RiftT2Zones = new List<string>
                {
                    "997217d606c04a3289cff476563b87f7", // Vampire Village Ruins
                    "70669632e11840f9b3e3c463137a6938", // Dracula's Castle Courtyard
                    "9d0dd663fdbb4498b6ba2ab501475c6e", // Frozen Lake Ruins
                    "e98a6696ebfe49f1b623ba32bd98f58a", // North Fortress Ruins
                    "287a1c1922934560a580d432e224c0b9", // Ancient Sacrificial Site
                    "68af6cff2b854e82ac208ca9f59a801f", // Vampire Cemetery
                },
                ZoneGroups = new List<List<string>>
                {
                    new List<string>
                    {
                        "1c8627c360994c698672682d810ebc65",
                        "1bacf251032c44e2bb5689122e1d6240",
                        "7bc63b78e0934ee6aeb98df40a4279f4",
                        "00bfdf78f096415ab6ccd7c81d970f79"
                    }
                }
            };

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }
    }
}