using HarmonyLib;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile
{
    [HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData))]
    public class StorageSettings_ExposeData
    {
        [HarmonyPostfix]
        public static void ExposeData(StorageSettings __instance)
        {
            var storageLimits = StorageLimits.GetLimitSettings(__instance);
            Scribe_Deep.Look(ref storageLimits, "limitSettings");
            if (storageLimits != null)
            {
                StorageLimits.SetLimitSettings(__instance, storageLimits);
            }
        }
    }
}