using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VarietyMattersStockpile;

public class StorageLimits : IExposable
{
    private static readonly Dictionary<StorageSettings, StorageLimits> limitSettings = new();

    public float CellFillPercentage = 1f;
    public float CellsFilled = -1f;
    public int DupStackLimit = -1;
    public bool NeedsFilled = true;
    public int StackSizeLimit = -1;

    public void ExposeData()
    {
        Scribe_Values.Look(ref DupStackLimit, "duplicatesLimit", -1);
        Scribe_Values.Look(ref StackSizeLimit, "stackSizeLimit", -1);
        Scribe_Values.Look(ref CellFillPercentage, "cellfillPercentage", 1f);
        Scribe_Values.Look(ref NeedsFilled, "needsFilled", true);
        Scribe_Values.Look(ref CellsFilled, "cellsFilled", -1f);
    }

    public static StorageLimits GetLimitSettings(StorageSettings settings)
    {
        return limitSettings.TryGetValue(settings, out var setting) ? setting : new StorageLimits();
    }

    public static void SetLimitSettings(StorageSettings settings, StorageLimits newSettings)
    {
        limitSettings[settings] = newSettings;
    }

    public static int CalculateSizeLimit(Thing t)
    {
        if (!t.Spawned)
        {
            return t.def.stackLimit;
        }

        var slotGroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position);
        if (slotGroup == null)
        {
            return t.def.stackLimit;
        }

        var limit = GetLimitSettings(slotGroup.Settings).StackSizeLimit;
        return limit > 0 && limit < t.def.stackLimit ? limit : t.def.stackLimit;
    }

    public static int CalculateSizeLimit(SlotGroup slotGroup)
    {
        if (slotGroup == null)
        {
            return 99999;
        }

        var limit = GetLimitSettings(slotGroup.Settings).StackSizeLimit;
        return limit > 0 ? limit : 99999;
    }

    public static void CheckIfFull(SlotGroup slotGroup, int maxItemsInCell)
    {
        var needsFilled = GetLimitSettings(slotGroup.Settings).NeedsFilled;
        if (!needsFilled)
        {
            return;
        }

        needsFilled = false;
        var cellsList = slotGroup.CellsList;
        float totalCells = cellsList.Count * maxItemsInCell;

        foreach (var intVec3 in cellsList)
        {
            // Locate the storable items in this cell excluding the storage itself
            var thingList = intVec3.GetThingList(slotGroup.parent.Map).FindAll(t => t.def.EverStorable(false));

            // Looks at available slots only, not partial stacks.  Partial stacks has some complexity
            // as it may never set needsFilled=true if e.g. a meal stack can't be merged as it's a
            // different ingredient type
            if (thingList.Count >= maxItemsInCell)
            {
                continue;
            }

            needsFilled = true;
            break;
        }

        if (needsFilled)
        {
            return;
        }

        GetLimitSettings(slotGroup.Settings).NeedsFilled = false;
        GetLimitSettings(slotGroup.Settings).CellsFilled = totalCells;
    }

    public static void CheckNeedsFilled(SlotGroup slotGroup, int maxItemsInCell, ref bool needsFilled,
        ref float cellsFilled)
    {
        if (needsFilled)
        {
            return;
        }

        var cellsList = slotGroup.CellsList;
        float totalCells = cellsList.Count * maxItemsInCell;
        float tmpCellsFilled = 0;

        foreach (var intVec3 in cellsList)
        {
            var thingList = intVec3.GetThingList(slotGroup.parent.Map).FindAll(t => t.def.EverStorable(false));
            tmpCellsFilled += thingList.Count;
        }

        cellsFilled = tmpCellsFilled;

        if (cellsFilled <= 0 ||
            GetLimitSettings(slotGroup.Settings).CellFillPercentage >= cellsFilled / totalCells)
        {
            needsFilled = true;
        }
    }
}