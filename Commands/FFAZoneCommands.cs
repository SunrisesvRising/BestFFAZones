using VampireCommandFramework;
using ProjectM;
using ProjectM.Network;
using Unity.Transforms;
using BestFFAZones.Services;

namespace BestFFAZones.Commands
{
    [CommandGroup("ffa", "Gestion des zones FFA")]
    public static class FFACommands
    {
        [Command("reload", description: "Recharge la config FFA", adminOnly: true)]
        public static void ReloadZones(ChatCommandContext ctx)
        {
            FfaConfigService.Load();
            ctx.Reply($"[BestFFAZones] Config rechargée.");
        }

        [Command("list", description: "Liste les zones FFA chargées")]
        public static void ListZones(ChatCommandContext ctx)
        {
            var zones = ZoneService.GetAllZones();
            if (zones.Count == 0) { ctx.Reply("Aucune zone chargée."); return; }
            ctx.Reply($"{zones.Count} zone(s) :");
            foreach (var z in zones)
                ctx.Reply($"  • {z.Name}");
        }

        [Command("zone", description: "Affiche le nom/ID de la zone où vous êtes")]
        public static void WhereAmI(ChatCommandContext ctx)
        {
            var em = Core.EntityManager;
            var entity = ctx.Event.SenderCharacterEntity;
            if (!em.Exists(entity)) { ctx.Reply("Personnage introuvable."); return; }

            var pos = em.GetComponentData<LocalToWorld>(entity).Position;
            var zone = ZoneService.GetZoneAtPosition(pos.x, pos.z);

            if (zone == null)
                ctx.Reply($"Position ({pos.x:F0}, {pos.z:F0}) — aucune zone détectée.");
            else
                ctx.Reply($"Zone : {zone.Name}");
        }
    }
}