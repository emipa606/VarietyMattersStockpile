using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VarietyMattersStockpile
{
    [HarmonyPatch]
    public class StorageUI
    {
        private const int max = 9999999;

        //Stack Limits
        private static string dupBuffer = "";
        private static string sizeBuffer = "";
        private static StorageSettings oldSettings;

        private static ITab_Storage currentTab;

        [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
        public static void Prefix(ITab_Storage __instance)
        {
            currentTab = __instance;
        }

        [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
        public static void Postfix()
        {
            currentTab = null;
        }

        [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
        [HarmonyPrefix]
        public static void ExpandWindow(ref Rect rect)
        {
            var tab = currentTab;
            if (tab == null)
            {
                return;
            }

            rect.yMin += 55f;
        }

        [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
        [HarmonyPostfix]
        public static void AddFilters(ref Rect rect)
        {
            var tab = currentTab;
            if (tab == null)
            {
                return;
            }

            var storeSettingsParent = (IStoreSettingsParent)typeof(ITab_Storage)
                .GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetGetMethod(true).Invoke(tab, Array.Empty<object>());
            var settings = storeSettingsParent?.GetStoreSettings();
            if (settings?.owner is not ISlotGroupParent slotGroupParent)
            {
                return;
            }

            var limitSettings = StorageLimits.GetLimitSettings(settings);
            //Text.Font = GameFont.Tiny;

            //Duplicates
            var dupLimit = limitSettings.dupStackLimit;
            var limitDuplicates = dupLimit != -1;
            Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - 75f, (rect.width / 2) - 45f, 20f), "Duplicates",
                ref limitDuplicates);
            if (limitDuplicates)
            {
                if (oldSettings != settings)
                {
                    dupBuffer = dupLimit.ToString();
                }

                Widgets.TextFieldNumeric(
                    new Rect(rect.xMin + (rect.width / 2) - 40f, rect.yMin - 75f, (rect.width / 2) - 115f, 20f),
                    ref dupLimit, ref dupBuffer, 1, max);
            }
            else
            {
                dupLimit = -1;
            }

            //Stack Limit
            var sizeLimit = limitSettings.stackSizeLimit;
            var hasLimit = sizeLimit != -1;
            Widgets.CheckboxLabeled(
                new Rect(rect.xMin + (rect.width / 2) - 5f, rect.yMin - 75f, (rect.width / 2) - 45f, 20f), "Stack size",
                ref hasLimit);
            if (hasLimit)
            {
                if (oldSettings != settings)
                {
                    sizeBuffer = sizeLimit.ToString();
                }

                Widgets.TextFieldNumeric(
                    new Rect(rect.xMin + (rect.width / 2) + 95f, rect.yMin - 75f, (rect.width / 2) - 105f, 20f),
                    ref sizeLimit, ref sizeBuffer, 1, max);
            }
            else
            {
                sizeLimit = -1;
            }

            //Refill
            var cellFillPercentage = limitSettings.cellFillPercentage * 100;
            var numCells = slotGroupParent.AllSlotCellsList().Count;
            var numCellsStart = Mathf.CeilToInt((100 - cellFillPercentage) / 100 * numCells);
            var startFilling = limitSettings.needsFilled;
            string label;
            switch (numCellsStart)
            {
                case 0:
                    label = "Always keep fully stocked";
                    break;
                case 1:
                    label = "Start refilling when 1 space is available";
                    break;
                default:
                    if (numCellsStart == numCells)
                    {
                        label = "Start refilling when empty";
                    }
                    else
                    {
                        label = "Start refilling when " + numCellsStart.ToString("N0") + " spaces are available";
                    }

                    break;
            }

            cellFillPercentage = Widgets.HorizontalSlider(new Rect(0f, rect.yMin - 105, rect.width, 36f),
                cellFillPercentage, 0f, 100f, false, label);
            if (cellFillPercentage < 100 && limitSettings.cellsFilled != numCells)
            {
                Widgets.CheckboxLabeled(
                    new Rect(rect.xMin + (rect.width * 0.6f), rect.yMin - 135f, rect.width * .30f, 20f), "Fill now",
                    ref startFilling);
            }

            //Update Settings
            oldSettings = settings;
            limitSettings.dupStackLimit = dupLimit;
            limitSettings.stackSizeLimit = sizeLimit;
            limitSettings.cellFillPercentage = cellFillPercentage / 100f;
            limitSettings.needsFilled = startFilling;
            StorageLimits.SetLimitSettings(settings, limitSettings);
            //Log.Message("Set stack size limit of " + limitSettings.stackSizeLimit);
        }
    }
}