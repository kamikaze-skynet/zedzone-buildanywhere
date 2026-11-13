using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BuildAnywhere
{
    [BepInPlugin(Guid, "Build Anywhere", Version)]
    public class Plugin : BasePlugin
    {
        public const string Guid = "girthy_mods.zedzone.buildanywhere";
        public const string Version = "1.1.0";

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<bool> ObjectsIgnoreObstacles;
        internal static ConfigEntry<bool> ObjectsIgnoreFloorLevel;
        internal static ConfigEntry<bool> ObjectsAllowStacking;
        internal static ConfigEntry<bool> WallsIgnoreObstacles;
        internal static ConfigEntry<bool> HordeModeUnlockAll;
        internal static ConfigEntry<bool> VerboseLog;

        public override void Load()
        {
            Enabled = Config.Bind("General", "Enabled", true,
                "Turn the whole mod on or off.");
            ObjectsIgnoreObstacles = Config.Bind("Objects", "IgnoreObstacles", true,
                "Furniture, crafting stations, planters and other built objects can be placed against walls, trees, other obstacles and in protected story areas (overlapping other objects is controlled by AllowStacking). The preview always shows as placeable.");
            ObjectsAllowStacking = Config.Bind("Objects", "AllowStacking", false,
                "Allow placing objects on top of / overlapping other built objects (furniture, stations, crates...). Off: objects still ignore walls, trees and protected areas, but can't overlap each other.");
            ObjectsIgnoreFloorLevel = Config.Bind("Objects", "IgnoreFloorLevel", true,
                "Objects can be placed even when the game says the floor under them is at a different level (e.g. half on a built floor, half off it).");
            WallsIgnoreObstacles = Config.Bind("Walls", "IgnoreObstacles", true,
                "Voxel walls, floors and stairs can be built through objects, trees, characters and in protected story areas. In co-op the HOST needs this on too, because the host checks every wall a guest builds.");
            HordeModeUnlockAll = Config.Bind("General", "HordeModeUnlockAll", true,
                "Horde mode blocks a few objects from being built. This lifts that block.");
            VerboseLog = Config.Bind("Debug", "VerboseLog", false, "Extra detail in BepInEx\\BuildAnywhere.log.");

            BLog.Verbose = VerboseLog.Value;
            BLog.Open(Log, Version);
            BLog.Info($"Config: Enabled={Enabled.Value} Objects.IgnoreObstacles={ObjectsIgnoreObstacles.Value} Objects.AllowStacking={ObjectsAllowStacking.Value} Objects.IgnoreFloorLevel={ObjectsIgnoreFloorLevel.Value} Walls.IgnoreObstacles={WallsIgnoreObstacles.Value} HordeModeUnlockAll={HordeModeUnlockAll.Value}");

            Patches.Apply(new Harmony(Guid));
            BLog.Info($"Build Anywhere {Version} loaded.");
        }
    }
}
