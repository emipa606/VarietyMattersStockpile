using HarmonyLib;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(ThingUtility), "TryAbsorbStackNumToTake")]
public static class AbsorbNumberPatch
{
    [HarmonyPriority(Priority.Low)]
    public static void Postfix(ref int __result, Thing thing, bool respectStackLimit)
    {
        if (!respectStackLimit)
        {
            return;
        }

        var sizeLimit = StorageLimits.CalculateSizeLimit(thing);
        if (__result > sizeLimit - thing.stackCount) //We only care if too much will be taken
        {
            __result = sizeLimit - thing.stackCount;
        }
    }
}