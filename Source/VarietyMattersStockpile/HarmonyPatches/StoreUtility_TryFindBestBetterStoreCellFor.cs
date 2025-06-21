using HarmonyLib;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(StoreUtility), nameof(StoreUtility.TryFindBestBetterStoreCellFor))]
public class StoreUtility_TryFindBestBetterStoreCellFor
{
    public static void Prefix(Thing t, ref StoragePriority currentPriority)
    {
        if (t.stackCount > StorageLimits.CalculateSizeLimit(t))
        {
            currentPriority = StoragePriority.Low;
        }
    }
}