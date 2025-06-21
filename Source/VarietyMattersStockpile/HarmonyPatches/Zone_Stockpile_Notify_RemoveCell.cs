using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Zone_Stockpile), nameof(Zone_Stockpile.RemoveCell))]
public class Zone_Stockpile_Notify_RemoveCell
{
    public static void Postfix(Zone_Stockpile __instance)
    {
        StorageLimits.CheckIfFull(__instance.GetSlotGroup(), 1);
    }
}