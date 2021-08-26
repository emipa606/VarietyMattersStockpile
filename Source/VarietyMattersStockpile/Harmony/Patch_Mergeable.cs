using HarmonyLib;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile
{
    [HarmonyPatch(typeof(ListerMergeables), "ShouldBeMergeable")]
    public class Patch_Mergeable
    {
        private static bool Prefix(ref bool __result, Thing t) //Should be faster performance than postfix
        {
            var stackLimit = StorageLimits.CalculateSizeLimit(t);
            if (stackLimit == t.def.stackLimit)
            {
                return true;
            }

            __result = !t.IsForbidden(Faction.OfPlayer) && t.GetSlotGroup() != null &&
                       t.stackCount !=
                       stackLimit; //Replace t.def.stackLimit in original method with StorageLimits.CalculateSizeLimit(t)
            return false;
        }

        /*
        static bool Postfix(bool __result, Thing t)
        {
            if (__result)
                return t.stackCount != StorageLimits.CalculateSizeLimit(t);
            else
                return !t.IsForbidden(Faction.OfPlayer) && t.GetSlotGroup() != null && t.stackCount != StorageLimits.CalculateSizeLimit(t); //Replace t.def.stackLimit in original method with StorageLimits.CalculateSizeLimit(t)
        }
        */
    }
}