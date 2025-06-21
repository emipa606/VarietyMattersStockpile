using HarmonyLib;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToCellStorageJob))]
public static class HaulAIUtility_HaulToCellStorageJob
{
    // Pawns will never overfill size limited stacks but can't haul multiple stacks.
    public static void Postfix(ref Job __result, Pawn p, Thing t, IntVec3 storeCell)
    {
        var curLimit = StorageLimits.CalculateSizeLimit(t);
        var newLimit = StorageLimits.CalculateSizeLimit(p.Map.haulDestinationManager.SlotGroupAt(storeCell));
        if (t.stackCount > curLimit)
        {
            __result.count = Mathf.Min(t.stackCount - curLimit, newLimit);
            return;
        }

        if (newLimit >= t.def.stackLimit)
        {
            return;
        }

        __result.count = newLimit;
        var thing = p.Map.thingGrid.ThingAt(storeCell, t.def);
        if (thing != null)
        {
            __result.count -= thing.stackCount;
        }
    }
}