using HarmonyLib;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
public class StoreUtility_NoStorageBlockersIn
{
    public static bool Postfix(bool __result, IntVec3 c, Map map, Thing thing)
    {
        if (!__result)
        {
            return false;
        }

        var slotGroup = c.GetSlotGroup(map);
        if (slotGroup == null)
        {
            return true;
        }

        //Log.Message("Get Limit Settings");
        var limitSettings = StorageLimits.GetLimitSettings(slotGroup.Settings);
        if (!limitSettings.NeedsFilled && limitSettings.CellFillPercentage < 1)
        {
            return false;
        }

        var sizeLimit = limitSettings.StackSizeLimit;
        if (sizeLimit <= 0 || sizeLimit >= thing.def.stackLimit)
        {
            return !(limitSettings.DupStackLimit > 0 && Mod_VarietyStockpile.DupLimitReached(slotGroup, c, map, thing,
                limitSettings.DupStackLimit, thing.def.stackLimit));
        }

        var newStack = true;
        __result = Mod_VarietyStockpile.StackSpaceAvailable(ref newStack, sizeLimit, c, map, thing);
        if (!newStack || limitSettings.DupStackLimit <= 0)
        {
            return __result;
        }

        return !Mod_VarietyStockpile.DupLimitReached(slotGroup, c, map, thing, limitSettings.DupStackLimit, sizeLimit);
    }
}