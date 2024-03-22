using HarmonyLib;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Thing), nameof(Thing.TryAbsorbStack))]
internal class Patch_Thing
{
    public static bool Prefix(Thing __instance, Thing other, bool respectStackLimit, ref bool __result)
    {
        //Log.Message("Thing.TryAbsorbStack begin");
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
/*
    [HarmonyPatch(typeof(Thing), nameof(Thing.TryAbsorbStack))]
    class Patch_Thing
    {
        public static bool Prefix(Thing __instance, Thing other, bool respectStackLimit, ref bool __result)
        {
            //Log.Message("Thing.TryAbsorbStack begin");
            if (!respectStackLimit) //We only care if the stack size limit matters
            {
                return true;
            }
            int sizeLimit = StorageLimits.CalculateSizeLimit(__instance);
            if (sizeLimit != __instance.def.stackLimit) //We only care if stack size limit was modified
            {
                if (__instance.CanStackWith(other))
                {
                    if (ThingUtility.TryAbsorbStackNumToTake(__instance, other, respectStackLimit) <= 0)
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
*/