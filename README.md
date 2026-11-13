# Build Anywhere (ZED ZONE)

Removes the game's building placement restrictions.

- **Objects anywhere.** Furniture, crafting stations, cooking hobs, planters and every other buildable object can be placed against walls and trees and inside the protected story areas.
- **Stacking (off by default).** Turn on `Objects.AllowStacking` to also place objects on top of / overlapping other built objects.
- **Ignore floor level.** Objects can be dropped even when the game says the floor under them is at a different level.
- **Walls anywhere.** Voxel walls, floors and stairs can be built through objects, trees and characters, and in protected story areas.
- **Horde mode.** Objects horde mode blocks from building can be built.

Materials are still used as normal. The wall tool's 32-block drag length and the world's height limit are built into the game code and are not changed.

## Requirements

- ZED ZONE (Windows, Steam)
- [ZED ZONE Code Mod Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=3834614258) (BepInEx 6 IL2CPP + ZedModsLoader). Run its `install.cmd` once.

## Install

- **Steam Workshop:** subscribe to Build Anywhere.
- **Manually:** put `BuildAnywhere.dll` and `mod.json` in `%USERPROFILE%\AppData\LocalLow\LevenLiu\ZEDZONE\Mods\BuildAnywhere\`.

`BepInEx\BuildAnywhere.log` should show `Build Anywhere 1.1.0 loaded.` and six `hook ... attached` lines.

## Co-op

**Everyone should install it, the host most of all.**

- Objects: each player's own game checks where they drop an object, and the host doesn't check it again. So a player with the mod can place objects anywhere even if others don't have it.
- Walls: the host checks every wall a guest builds. If the host doesn't have the mod (or has `Walls.IgnoreObstacles` off), a guest's blocked walls are refused.

## Config

`BepInEx\config\girthy_mods.zedzone.buildanywhere.cfg` (created after the first launch):

| Setting | Default | What it does |
|---|---|---|
| `General.Enabled` | `true` | Turn the mod on or off. |
| `General.HordeModeUnlockAll` | `true` | Allow objects that horde mode blocks. |
| `Objects.IgnoreObstacles` | `true` | Place objects against walls, trees and other obstacles and in protected areas. |
| `Objects.AllowStacking` | `false` | Also allow objects on top of / overlapping other built objects. |
| `Objects.IgnoreFloorLevel` | `true` | Place objects even when the floor under them is at a different level. |
| `Walls.IgnoreObstacles` | `true` | Build walls through objects, trees, characters and in protected areas. |
| `Debug.VerboseLog` | `false` | Extra detail in the log. |

Careful with walls through characters and zombies: they can end up stuck inside them.

## License

MIT
