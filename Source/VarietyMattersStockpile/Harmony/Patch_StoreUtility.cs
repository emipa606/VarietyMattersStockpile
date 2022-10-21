using HarmonyLib;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch]
public class Patch_StoreUtility
{
    [HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
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
        if (slotGroup.CellsList.Count != 1 && !limitSettings.needsFilled && limitSettings.cellFillPercentage < 1)
        {
            return false;
        }

        var sizeLimit = limitSettings.stackSizeLimit;
        if (sizeLimit <= 0 || sizeLimit >= thing.def.stackLimit)
        {
            return !(limitSettings.dupStackLimit > 0 && DupLimitReached(slotGroup, c, map, thing,
                limitSettings.dupStackLimit, thing.def.stackLimit));
        }

        var newStack = true;
        __result = StackSpaceAvailable(ref newStack, sizeLimit, c, map, thing);
        if (!newStack || limitSettings.dupStackLimit <= 0)
        {
            return __result;
        }

        return !DupLimitReached(slotGroup, c, map, thing, limitSettings.dupStackLimit, sizeLimit);
    }

    [HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
    public static void Prefix(Thing t, ref StoragePriority currentPriority)
    {
        if (t.stackCount > StorageLimits.CalculateSizeLimit(t))
        {
            currentPriority = StoragePriority.Low;
        }
    }

    public static bool StackSpaceAvailable(ref bool firstStack, int sizeLimit, IntVec3 c, Map map, Thing thing)
    {
        //Log.Message(thing.Label + "'s stack size limit is " + sizeLimit);
        var storedItems = map.thingGrid.ThingsListAt(c);
        var numStacks = 0;
        foreach (var thing2 in storedItems)
        {
            if (thing == thing2)
            {
                numStacks += 1;
            }
            else if (thing2.def.EverStorable(false) && thing.CanStackWith(thing2))
            {
                numStacks += 1;
                if (thing2.stackCount >= sizeLimit * numStacks)
                {
                    continue;
                }

                firstStack = false;
                return true;
            }
        }

        firstStack = numStacks == 0;
        return firstStack;
    }

    public static bool DupLimitReached(SlotGroup slotGroup, IntVec3 c, Map map, Thing t, int dupLimit,
        int sizeLimit)
    {
        // Log.Message("Checking for duplicates");
        var duplicates = 0;
        foreach (var storedItem in slotGroup.HeldThings)
        {
            if (!storedItem.def.EverStorable(false) || !t.CanStackWith(storedItem) &&
                (!ModSettings_VarietyStockpile.limitNonStackables ||
                 t.def.stackLimit != 1 || t.def != storedItem.def))
            {
                continue;
            }

            if (storedItem.Position == c && storedItem.stackCount < sizeLimit)
            {
                return false;
            }

            duplicates++;
            if (duplicates >= dupLimit)
            {
                return true;
            }
        }

        if (!ModSettings_VarietyStockpile.checkReservations || map.reservationManager == null)
        {
            return false;
        }

        var allReservations = map.reservationManager.ReservationsReadOnly;
        if (allReservations == null)
        {
            return false;
        }

        foreach (var reservation in allReservations)
        {
            var job = reservation.Job;
            if (job?.targetA.Thing == null)
            {
                continue;
            }

            SlotGroup slotGroup2 = null;
            var reservedItem = job.targetA.Thing;
            if (job.def == JobDefOf.HaulToCell)
            {
                var map2 = reservedItem.Map ?? reservedItem.MapHeld;
                if (map2 != null)
                {
                    slotGroup2 = job.targetB.Cell.GetSlotGroup(map2);
                }
            }
            else if (job.def == JobDefOf.HaulToContainer && job.targetB.Thing != null)
            {
                slotGroup2 = job.targetB.Thing.Position.GetSlotGroup(job.targetB.Thing.Map);
            }

            if (slotGroup2 == null || slotGroup != slotGroup2 || t == reservedItem ||
                !reservedItem.def.EverStorable(false) || !t.CanStackWith(reservedItem) &&
                (!ModSettings_VarietyStockpile.limitNonStackables ||
                 t.def.stackLimit != 1 || t.def != reservedItem.def))
            {
                continue;
            }

            duplicates++;
            if (duplicates >= dupLimit)
            {
                return true;
            }
        }

        return false;
    }
}