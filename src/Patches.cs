using System;
using System.Reflection;
using HarmonyLib;

namespace BuildAnywhere
{
    // What the game checks when you build (read from the game code):
    //
    // Objects (furniture, stations, planters...): GameController.BuildTerrainObject
    // spawns the object "lifted" in your hands. Every frame TerrainObject.UpdateLifted
    // runs ResolveLiftPlacementLevel (sets liftPlacementLevelOk) and then the virtual
    // CanPlace() (protected story areas + a Physics2D box overlap against other
    // colliders) to tint the preview. TerrainObject.PlaceTerrainObject refuses the
    // drop unless liftPlacementLevelOk && CanPlace(), and also refuses objects that
    // GameConfigs.IsTerrainObjectBuildBlocked marks (horde mode). The host does NOT
    // re-check overlap for a guest's object, so each player only needs the mod for
    // their own placements.
    //
    // Walls (voxel build tool): VoxelBuildTool.TryWriteColumn calls
    // VoxelPlacementCheck.CanPlaceColumn for every column (protected areas, raised
    // surface objects, and a collider overlap for objects/characters). In co-op the
    // host runs the same check again in NetVoxelSync.HostApplyPlace for every wall a
    // guest builds, so the host needs the mod for guests' walls to go through.
    //
    // All hooks are postfixes that only overwrite the answer; the game's own code
    // still runs, so nothing else it does on the way is skipped.
    internal static class Patches
    {
        internal static void Apply(Harmony h)
        {
            Hook(h, typeof(TerrainObject), "CanPlace", Type.EmptyTypes, nameof(CanPlacePostfix));
            Hook(h, typeof(TerrainObject_CookingHob), "CanPlace", Type.EmptyTypes, nameof(CanPlacePostfix));
            Hook(h, typeof(TerrainObject_PlantingPot), "CanPlace", Type.EmptyTypes, nameof(CanPlacePostfix));
            Hook(h, typeof(TerrainObject), "ResolveLiftPlacementLevel", Type.EmptyTypes, nameof(LevelPostfix));
            Hook(h, typeof(GameConfigs), "IsTerrainObjectBuildBlocked", new[] { typeof(int) }, nameof(BuildBlockedPostfix));
            Hook(h, typeof(VoxelPlacementCheck), "CanPlaceColumn", new[] { typeof(int), typeof(int), typeof(int), typeof(int) }, nameof(ColumnPostfix));
        }

        static void Hook(Harmony h, Type t, string name, Type[] args, string postfix)
        {
            try
            {
                MethodInfo m = AccessTools.Method(t, name, args);
                if (m == null || m.DeclaringType != t) { BLog.Warn($"hook {t.Name}.{name} not found - skipped"); return; }
                h.Patch(m, postfix: new HarmonyMethod(typeof(Patches), postfix));
                BLog.Info($"hook {t.Name}.{name} attached");
            }
            catch (Exception e) { BLog.Error($"hook {t.Name}.{name} failed", e); }
        }

        // Logged once per kind so the log shows the mod actually stepped in.
        static bool _saidObject, _saidLevel, _saidWall, _saidHorde;

        static void CanPlacePostfix(ref bool __result)
        {
            if (__result || !Plugin.Enabled.Value || !Plugin.ObjectsIgnoreObstacles.Value) return;
            __result = true;
            if (!_saidObject) { _saidObject = true; BLog.Info("object placement check overridden (first time this session)"); }
        }

        static void LevelPostfix(TerrainObject __instance)
        {
            try
            {
                if (!Plugin.Enabled.Value || !Plugin.ObjectsIgnoreFloorLevel.Value || __instance == null) return;
                if (__instance.liftPlacementLevelOk) return;
                __instance.liftPlacementLevelOk = true;
                if (!_saidLevel) { _saidLevel = true; BLog.Info("object floor-level check overridden (first time this session)"); }
            }
            catch (Exception e) { BLog.Error("floor-level override failed", e); }
        }

        static void BuildBlockedPostfix(int terrainObjectId, ref bool __result)
        {
            if (!__result || !Plugin.Enabled.Value || !Plugin.HordeModeUnlockAll.Value) return;
            __result = false;
            if (!_saidHorde) { _saidHorde = true; BLog.Info($"horde-mode build block lifted for object {terrainObjectId}"); }
        }

        static void ColumnPostfix(int globalVx, int globalVy, ref bool __result)
        {
            if (__result || !Plugin.Enabled.Value || !Plugin.WallsIgnoreObstacles.Value) return;
            __result = true;
            if (!_saidWall) { _saidWall = true; BLog.Info($"wall placement check overridden at {globalVx},{globalVy} (first time this session)"); }
        }
    }
}
