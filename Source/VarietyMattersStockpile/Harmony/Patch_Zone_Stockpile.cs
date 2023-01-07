using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Zone_Stockpile))]
public class Patch_Zone_Stockpile
{
    [HarmonyPatch("Notify_ReceivedThing")]
    [HarmonyPostfix]
    public static void Postfix_ReceivedThing(Zone_Stockpile __instance)
    {
        //Log.Message("Stockpile received something");
        StorageLimits.CheckIfFull(__instance.GetSlotGroup(), 1);
    }

    [HarmonyPatch("RemoveCell")]
    [HarmonyPostfix]
    public static void Postfix_RemoveCell(Zone_Stockpile __instance)
    {
        //Log.Message("Stockpile is smaller, check if it still needs to be filled");
        StorageLimits.CheckIfFull(__instance.GetSlotGroup(), 1);
    }

    [HarmonyPatch("Notify_LostThing")]
    [HarmonyPostfix]
    public static void Postfix_LostThing(Zone_Stockpile __instance)
    {
        //Log.Message("Something removed from stockpile");
        var slotGroup = __instance.GetSlotGroup();
        StorageLimits.CheckNeedsFilled(slotGroup, 1,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).needsFilled,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).cellsFilled);
    }

    [HarmonyPatch("AddCell")]
    [HarmonyPostfix]
    public static void Postfix_AddCell(Zone_Stockpile __instance)
    {
        //Log.Message("Stockpile is larger, start filling");
        var slotGroup = __instance.GetSlotGroup();
        if (slotGroup != null)
        {
            StorageLimits.GetLimitSettings(slotGroup.Settings).needsFilled = true;
        }
    }
}