using HarmonyLib;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Thing), nameof(Thing.TryAbsorbStack))]
internal class Thing_TryAbsorbStack
{
    public static bool Prefix(Thing __instance, Thing other, bool respectStackLimit, ref bool __result)
    {
        var sizeLimit = StorageLimits.CalculateSizeLimit(__instance);
        if (sizeLimit >= __instance.def.stackLimit || !__instance.CanStackWith(other))
        {
            return true;
        }

        if (ThingUtility.TryAbsorbStackNumToTake(__instance, other, respectStackLimit) > 0)
        {
            return true;
        }

        __result = false;
        return false;
    }
}