using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using System.IO;
using BepInEx.Logging;

namespace BestFFAZones
{
    [BepInPlugin("com.tonpseudo.BestFFAZones", "BestFFAZones", "1.0.0")]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;
        public static Plugin Instance { get; private set; }
        public static ManualLogSource Logger;

        public override void Load()
        {
            Instance = this;
            Logger = Log;

            // 1. Gestion du dossier de configuration
            var configPath = Path.Combine(BepInEx.Paths.ConfigPath, "BestFFAZones");
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} est en cours de chargement...");

            // 4. Enregistrement des commandes (VCF)
            CommandRegistry.RegisterAll();

            // 5. Application des patches Harmony
            _harmony = new Harmony("com.tonpseudo.BestFFAZones");
            _harmony.PatchAll();

            Log.LogInfo("BestFFAZones est chargé et prêt !");
        }

        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            return base.Unload();
        }
    }
}