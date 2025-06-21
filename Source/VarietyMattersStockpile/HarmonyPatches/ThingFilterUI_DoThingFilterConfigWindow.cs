using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(ThingFilterUI), nameof(ThingFilterUI.DoThingFilterConfigWindow))]
public class ThingFilterUI_DoThingFilterConfigWindow
{
    public static void Prefix(ref Rect rect)
    {
        var tab = Mod_VarietyStockpile.currentTab;
        if (tab == null)
        {
            return;
        }

        rect.yMin += Mod_VarietyStockpile.minY;
    }

    public static void Postfix(ref Rect rect)
    {
        var tab = Mod_VarietyStockpile.currentTab;
        if (tab == null)
        {
            return;
        }

        var storeSettingsParent = (IStoreSettingsParent)typeof(ITab_Storage)
            .GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetGetMethod(true).Invoke(tab, []);
        var settings = storeSettingsParent?.GetStoreSettings();
        if (settings?.owner is not ISlotGroupParent slotGroupParent)
        {
            return;
        }

        var limitSettings = StorageLimits.GetLimitSettings(settings);

        //Duplicates
        var dupLimit = limitSettings.DupStackLimit;
        var limitDuplicates = dupLimit != -1;
        Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - Mod_VarietyStockpile.minY, (rect.width / 2) - 45f, 20f),
            "VMS.Duplicates".Translate(),
            ref limitDuplicates);
        if (limitDuplicates)
        {
            if (Mod_VarietyStockpile.oldSettings != settings)
            {
                Mod_VarietyStockpile.dupBuffer = dupLimit.ToString();
            }

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) - 40f, rect.yMin - Mod_VarietyStockpile.minY,
                    (rect.width / 2) - 115f, 20f),
                ref dupLimit, ref Mod_VarietyStockpile.dupBuffer, 1, Mod_VarietyStockpile.max);
        }
        else
        {
            dupLimit = -1;
        }

        //Stack Limit
        var sizeLimit = limitSettings.StackSizeLimit;
        var hasLimit = sizeLimit != -1;
        Widgets.CheckboxLabeled(
            new Rect(rect.xMin + (rect.width / 2) - 5f, rect.yMin - Mod_VarietyStockpile.minY, (rect.width / 2) - 45f,
                20f),
            "VMS.StackSize".Translate(),
            ref hasLimit);
        if (hasLimit)
        {
            if (Mod_VarietyStockpile.oldSettings != settings)
            {
                Mod_VarietyStockpile.sizeBuffer = sizeLimit.ToString();
            }

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) + 95f, rect.yMin - Mod_VarietyStockpile.minY,
                    (rect.width / 2) - 105f, 20f),
                ref sizeLimit, ref Mod_VarietyStockpile.sizeBuffer, 1, Mod_VarietyStockpile.max);
        }
        else
        {
            sizeLimit = -1;
        }

        //Refill
        var cellFillPercentage = limitSettings.CellFillPercentage * 100;
        var filledCells = 0;
        var numCells = 0;
        var foundBuilding = false;

        foreach (var intVec3 in slotGroupParent.AllSlotCells())
        {
            var thingList = intVec3.GetThingList(slotGroupParent.Map);
            filledCells += thingList.Count(t => t.def.EverStorable(false));

            foreach (var thing in thingList)
            {
                if (thing is not Building building)
                {
                    continue;
                }

                foundBuilding = true;
                numCells += building.MaxItemsInCell;
                break;
            }
        }

        if (!foundBuilding)
        {
            numCells = slotGroupParent.AllSlotCellsList().Count;
        }

        var numCellsStart = Mathf.CeilToInt((100 - cellFillPercentage) / 100 * numCells);
        var startFilling = numCellsStart == 0 || numCells - filledCells >= numCellsStart;
        string label;

        switch (numCellsStart)
        {
            case 0:
                label = "VMS.FullyStocked".Translate();
                break;
            case 1:
                label = "VMS.StartAtOne".Translate();
                break;
            default:
                label = numCellsStart == numCells
                    ? "VMS.RefillWhenEmpty".Translate()
                    : "VMS.RefillWhenAmount".Translate(numCellsStart.ToString("N0"));

                break;
        }

        cellFillPercentage = Widgets.HorizontalSlider(
            new Rect(0f, rect.yMin - Mod_VarietyStockpile.minY - 40, rect.width, 36f),
            cellFillPercentage, 0f, 100f, false, label);
        if (cellFillPercentage < 100 && limitSettings.CellsFilled != numCells)
        {
            Widgets.CheckboxLabeled(
                new Rect(rect.xMin + (rect.width * 0.6f), rect.yMin - Mod_VarietyStockpile.minY - 70, rect.width * .30f,
                    30f),
                "VMS.FillNow".Translate(),
                ref startFilling);
        }

        //Update Settings
        Mod_VarietyStockpile.oldSettings = settings;
        limitSettings.DupStackLimit = dupLimit;
        limitSettings.StackSizeLimit = sizeLimit;
        limitSettings.CellFillPercentage = cellFillPercentage / 100f;
        limitSettings.NeedsFilled = startFilling;
        StorageLimits.SetLimitSettings(settings, limitSettings);
    }
}