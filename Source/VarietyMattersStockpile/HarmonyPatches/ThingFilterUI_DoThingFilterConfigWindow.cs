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
        var tab = Mod_VarietyStockpile.CurrentTab;
        if (tab == null)
        {
            return;
        }

        rect.yMin += Mod_VarietyStockpile.MinY;
    }

    public static void Postfix(ref Rect rect)
    {
        var tab = Mod_VarietyStockpile.CurrentTab;
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

        var baseFont = Text.Font;
        var limitSettings = StorageLimits.GetLimitSettings(settings);

        //Duplicates
        var dupLimit = limitSettings.DupStackLimit;
        var limitDuplicates = dupLimit != -1;
        var duplicatesRect = new Rect(rect.xMin, rect.yMin - Mod_VarietyStockpile.MinY, (rect.width / 2) - 45f, 20f);
        var duplicatesLabel = "VMS.Duplicates".Translate();
        if (Text.CalcSize(duplicatesLabel).x > duplicatesRect.width - duplicatesRect.height)
        {
            Text.Font = GameFont.Tiny;
        }

        Widgets.CheckboxLabeled(duplicatesRect, duplicatesLabel, ref limitDuplicates);
        Text.Font = baseFont;
        if (limitDuplicates)
        {
            if (Mod_VarietyStockpile.OldSettings != settings)
            {
                Mod_VarietyStockpile.DupBuffer = dupLimit.ToString();
            }

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) - 40f, rect.yMin - Mod_VarietyStockpile.MinY,
                    (rect.width / 2) - 115f, 20f),
                ref dupLimit,
                ref Mod_VarietyStockpile.DupBuffer,
                1,
                Mod_VarietyStockpile.Max);

            if (dupLimit < 1)
            {
                dupLimit = 1;
            }
        }
        else
        {
            dupLimit = -1;
        }

        limitSettings.DupStackLimit = dupLimit;

        //Stack Limit
        var sizeLimit = limitSettings.StackSizeLimit;
        var hasLimit = sizeLimit != -1;
        var stackSizeRect = new Rect(rect.xMin + (rect.width / 2) - 5f, rect.yMin - Mod_VarietyStockpile.MinY,
            (rect.width / 2) - 45f, 20f);
        var stackSizeLabel = "VMS.StackSize".Translate();
        if (Text.CalcSize(stackSizeLabel).x > stackSizeRect.width - stackSizeRect.height)
        {
            Text.Font = GameFont.Tiny;
        }

        Widgets.CheckboxLabeled(stackSizeRect, stackSizeLabel, ref hasLimit);
        Text.Font = baseFont;

        if (hasLimit)
        {
            if (Mod_VarietyStockpile.OldSettings != settings)
            {
                Mod_VarietyStockpile.SizeBuffer = sizeLimit.ToString();
            }

            Widgets.TextFieldNumeric(
                new Rect(rect.xMin + (rect.width / 2) + 95f, rect.yMin - Mod_VarietyStockpile.MinY,
                    (rect.width / 2) - 105f, 20f),
                ref sizeLimit,
                ref Mod_VarietyStockpile.SizeBuffer,
                1,
                Mod_VarietyStockpile.Max);

            if (sizeLimit < 1)
            {
                sizeLimit = 1;
            }
        }
        else
        {
            sizeLimit = -1;
        }

        limitSettings.StackSizeLimit = sizeLimit;

        //Refill & Fill Now
        var cellFillPct = limitSettings.CellFillPercentage * 100f;
        var total = 0;
        var found = false;
        foreach (var c in slotGroupParent.AllSlotCells())
        {
            var list = c.GetThingList(slotGroupParent.Map);
            list.Count(t => t.def.EverStorable(false));
            foreach (var t in list)
            {
                if (t is not Building b)
                {
                    continue;
                }

                found = true;
                total += b.MaxItemsInCell;
                break;
            }
        }

        if (!found)
        {
            total = slotGroupParent.AllSlotCellsList().Count;
        }

        var startAt = Mathf.CeilToInt((100 - cellFillPct) / 100 * total);
        string label = startAt == 0 ? "VMS.FullyStocked".Translate() :
            startAt == 1 ? "VMS.StartAtOne".Translate() :
            startAt == total ? "VMS.RefillWhenEmpty".Translate() :
            "VMS.RefillWhenAmount".Translate(startAt.ToString("N0"));
        var cellFillRect = new Rect(0f, rect.yMin - Mod_VarietyStockpile.MinY - 40f, rect.width, 36f);
        if (Text.CalcHeight(label, cellFillRect.width) > 30)
        {
            Text.Font = GameFont.Tiny;
        }

        cellFillPct = Widgets.HorizontalSlider(cellFillRect.BottomHalf(), cellFillPct, 0f, 100f);
        var anchor = Text.Anchor;
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(cellFillRect, label);
        Text.Font = baseFont;
        Text.Anchor = anchor;

        if (cellFillPct < 100f && limitSettings.CellsFilled != total)
        {
            var fillNowText = "VMS.FillNow".Translate();
            var fillNowRect = new Rect(rect.xMin + (rect.width * 0.6f), rect.yMin - Mod_VarietyStockpile.MinY - 70f,
                rect.width * 0.30f, 30f);
            if (Text.CalcSize(fillNowText).x > fillNowRect.width - fillNowRect.height)
            {
                Text.Font = GameFont.Tiny;
            }

            Widgets.CheckboxLabeled(fillNowRect, fillNowText, ref limitSettings.NeedsFilled);
            Text.Font = baseFont;
        }

        //Update Settings
        Mod_VarietyStockpile.OldSettings = settings;
        limitSettings.CellFillPercentage = cellFillPct / 100f;
        StorageLimits.SetLimitSettings(settings, limitSettings);
    }
}