using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.Notify_LostThing))]
public class Building_Storage_Notify_LostThing
{
    public static void Postfix(Building_Storage __instance)
    {
        var slotGroup = __instance.GetSlotGroup();
        StorageLimits.CheckNeedsFilled(slotGroup, __instance.MaxItemsInCell,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).NeedsFilled,
            ref StorageLimits.GetLimitSettings(slotGroup.Settings).CellsFilled);
    }
}