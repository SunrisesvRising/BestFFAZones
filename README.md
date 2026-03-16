# BestFFAZones

A V Rising server-side mod that defines **Free-For-All PvP zones** using the game's native map regions. Players receive automatic notifications when entering or leaving FFA zones, with support for zone groups and Rift T2 conditional FFA zones.

---

## How It Works

On server startup, BestFFAZones reads all map zone polygons directly from the game world. Every second, it checks each connected player's position against those polygons using point-in-polygon detection. When a player crosses a zone boundary, they receive an in-game chat notification.

There are three types of zone behaviour:

- **FFA Zones** — permanently active PvP zones. Players are notified on entry and exit.
- **Rift T2 Zones** — zones that are only FFA during a Rift T2 event. Players are warned on entry that PvP will activate when a Rift T2 is running.
- **Zone Groups** — multiple adjacent zones treated as one continuous area. Moving between zones in the same group produces no notification, preventing spam at internal borders.

---

## Installation

1. Install **BepInEx** on your V Rising dedicated server.
2. Install **VampireCommandFramework**.
3. Drop `BestFFAZones.dll` into your `BepInEx/plugins/` folder.
4. Start the server — the config file is auto-generated on first launch at:
   ```
   BepInEx/config/BestFFAZones/FfaZoneConfig.json
   ```

---

## Configuration

Config file: `BepInEx/config/BestFFAZones/FfaZoneConfig.json`

The config is a JSON file with three sections:

```json
{
  "FfaZones": [
    "9f4bb828842244b9b17ce3d9fd39be6e",
    "a59283ab08114eb99d930f544fd72cb3"
  ],
  "RiftT2Zones": [
    "997217d606c04a3289cff476563b87f7",
    "70669632e11840f9b3e3c463137a6938"
  ],
  "ZoneGroups": [
    [
      "1c8627c360994c698672682d810ebc65",
      "1bacf251032c44e2bb5689122e1d6240",
      "7bc63b78e0934ee6aeb98df40a4279f4"
    ]
  ]
}
```

| Field | Description |
|-------|-------------|
| `FfaZones` | List of zone IDs that are permanently FFA. |
| `RiftT2Zones` | List of zone IDs that become FFA only during a Rift T2 event. |
| `ZoneGroups` | List of zone ID groups. Zones in the same group are treated as one continuous area — crossing between them produces no notification. |

Zone IDs are the internal names returned by the game's `MapZoneData`. Use `.ffa zone` in-game to find the ID of the zone you're currently standing in.

Changes to the config can be applied live with `.ffa reload` — no server restart needed.

---

## Commands

| Command | Description | Admin only |
|---------|-------------|:---:|
| `.ffa reload` | Reload `FfaZoneConfig.json` from disk. | ✅ |
| `.ffa list` | List all zones currently loaded from the game world. | ❌ |
| `.ffa zone` | Display the name/ID of the zone you are currently standing in. | ❌ |

> Use `.ffa zone` while standing in a location to find its zone ID, then add it to the config.

---

## Player Notifications

| Event | Message |
|-------|---------|
| Enter FFA zone | `⚔ You have entered a FFA zone. All PvP is enabled!` |
| Leave FFA zone | `✔ You have left the FFA zone.` |
| Enter Rift T2 zone | `⚠ This zone is FFA only during Rift T2 events. PvP will be enabled when a Rift T2 is active!` |
| Leave Rift T2 zone | `You have left the Rift T2 FFA zone.` |

Notifications are sent only on zone transitions. Players moving within the same zone group receive no message.

---

## Notes

- Zone detection runs every **1 second** server-side, keeping performance impact minimal.
- Zones are loaded from the game world on the **first player connection** after server start.
- The mod handles zone **boundary detection** and **notifications** only — actual PvP rule enforcement (damage, flagging) depends on your server's PvP settings.

---

## Dependencies

- [BepInEx](https://github.com/BepInEx/BepInEx) for V Rising
- [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework)
