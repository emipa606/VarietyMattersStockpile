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
            return;

        rect.yMin += Mod_VarietyStockpile.minY;
    }

    public static void Postfix(ref Rect rect)
    {
        var tab = Mod_VarietyStockpile.currentTab;
        if (tab == null)
            return;

        var storeSettingsParent = (IStoreSettingsParent)typeof(ITab_Storage)
            .GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetGetMethod(true).Invoke(tab, []);
        var settings = storeSettingsParent?.GetStoreSettings();
        if (settings?.owner is not ISlotGroupParent slotGroupParent)
            return;

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
                Mod_VarietyStockpile.dupBuffer = dupLimit.ToString();

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) - 40f, rect.yMin - Mod_VarietyStockpile.minY, (rect.width / 2) - 115f, 20f),
                ref dupLimit,
                ref Mod_VarietyStockpile.dupBuffer,
                1,
                Mod_VarietyStockpile.max);

            if (dupLimit < 1)
                dupLimit = 1;
        }
        else
        {
            dupLimit = -1;
        }
        limitSettings.DupStackLimit = dupLimit;

        //Stack Limit
        var sizeLimit = limitSettings.StackSizeLimit;
        var hasLimit = sizeLimit != -1;
        Widgets.CheckboxLabeled(
            new Rect(rect.xMin + (rect.width / 2) - 5f, rect.yMin - Mod_VarietyStockpile.minY, (rect.width / 2) - 45f, 20f),
            "VMS.StackSize".Translate(),
            ref hasLimit);
        if (hasLimit)
        {
            if (Mod_VarietyStockpile.oldSettings != settings)
                Mod_VarietyStockpile.sizeBuffer = sizeLimit.ToString();

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) + 95f, rect.yMin - Mod_VarietyStockpile.minY, (rect.width / 2) - 105f, 20f),
                ref sizeLimit,
                ref Mod_VarietyStockpile.sizeBuffer,
                1,
                Mod_VarietyStockpile.max);

            if (sizeLimit < 1)
                sizeLimit = 1;
        }
        else
        {
            sizeLimit = -1;
        }
        limitSettings.StackSizeLimit = sizeLimit;

        //Refill & Fill Now
        var cellFillPct = limitSettings.CellFillPercentage * 100f;
        int filled = 0, total = 0;
        bool found = false;
        foreach (var c in slotGroupParent.AllSlotCells())
        {
            var list = c.GetThingList(slotGroupParent.Map);
            filled += list.Count(t => t.def.EverStorable(false));
            foreach (var t in list)
            {
                if (t is not Building b) continue;
                found = true;
                total += b.MaxItemsInCell;
                break;
            }
        }
        if (!found)
            total = slotGroupParent.AllSlotCellsList().Count;

        int startAt = Mathf.CeilToInt((100 - cellFillPct) / 100 * total);
        string label = startAt == 0 ? "VMS.FullyStocked".Translate() :
                        startAt == 1 ? "VMS.StartAtOne".Translate() :
                        startAt == total ? "VMS.RefillWhenEmpty".Translate() : "VMS.RefillWhenAmount".Translate(startAt.ToString("N0"));

        cellFillPct = Widgets.HorizontalSlider(new Rect(0f, rect.yMin - Mod_VarietyStockpile.minY - 40f, rect.width, 36f), cellFillPct, 0f, 100f, false, label);

        if (cellFillPct < 100f && limitSettings.CellsFilled != total)
        {
            Widgets.CheckboxLabeled(new Rect(rect.xMin + (rect.width * 0.6f), rect.yMin - Mod_VarietyStockpile.minY - 70f, rect.width * 0.30f, 30f),
                "VMS.FillNow".Translate(),
                ref limitSettings.NeedsFilled);
        }

        //Update Settings
        Mod_VarietyStockpile.oldSettings = settings;
        limitSettings.CellFillPercentage = cellFillPct / 100f;
        StorageLimits.SetLimitSettings(settings, limitSettings);
    }
}