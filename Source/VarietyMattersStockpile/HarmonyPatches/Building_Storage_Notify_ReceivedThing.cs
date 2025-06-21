using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.Notify_ReceivedThing))]
public class Building_Storage_Notify_ReceivedThing
{
    public static void Postfix(Building_Storage __instance)
    {
        StorageLimits.CheckIfFull(__instance.GetSlotGroup(), __instance.MaxItemsInCell);
    }
}