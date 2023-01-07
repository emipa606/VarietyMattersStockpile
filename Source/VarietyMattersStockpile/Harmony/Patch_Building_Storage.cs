using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Building_Storage))]
public class Patch_Building_Storage
{
    [HarmonyPatch("Notify_ReceivedThing")]
    [HarmonyPostfix]
    public static void Postfix_ReceivedThing(Building_Storage __instance)
    {
        //Log.Message("Stockpile received something");
        StorageLimits.CheckIfFull(__instance.GetSlotGroup(), __instance.MaxItemsInCell);
    }

    [HarmonyPatch("Notify_LostThing")]
    [HarmonyPostfix]
    public static void Postfix_LostThing(Building_Storage __instance)
    {
        //Log.Message("Something removed from stockpile");
        var slotGroup = __instance.GetSlotGroup();
        StorageLimits.CheckNeedsFilled(slotGroup, __instance.MaxItemsInCell,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).needsFilled,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).cellsFilled);
    }
}