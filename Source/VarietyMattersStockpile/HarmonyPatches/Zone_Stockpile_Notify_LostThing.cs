using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Zone_Stockpile), nameof(Zone_Stockpile.Notify_LostThing))]
public class Zone_Stockpile_Notify_LostThing
{
    public static void Postfix(Zone_Stockpile __instance)
    {
        var slotGroup = __instance.GetSlotGroup();
        StorageLimits.CheckNeedsFilled(slotGroup, 1,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).NeedsFilled,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).CellsFilled);
    }
}