using System.Reflection;
using HarmonyLib;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace VarietyMattersStockpile;

public class Mod_VarietyStockpile : Mod
{
    public const int max = 9999999;
    public const float minY = 85f;
    private static string currentVersion;

    //Stack Limits
    public static string dupBuffer = "";
    public static string sizeBuffer = "";
    public static StorageSettings oldSettings;

    public static ITab_Storage currentTab;

    public Mod_VarietyStockpile(ModContentPack content) : base(content)
    {
        //Log.Message("Stockpiles patched for variety");
        new Harmony("cozarkian.varietymattersstockpile").PatchAll(Assembly.GetExecutingAssembly());
        GetSettings<ModSettings_VarietyStockpile>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var rect = new Rect(260f, 50f, inRect.width * .4f, inRect.height);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.CheckboxLabeled("VMS.SingleDuplicates".Translate(),
            ref ModSettings_VarietyStockpile.LimitNonStackables);
        listingStandard.CheckboxLabeled("VMS.CheckReservations".Translate(),
            ref ModSettings_VarietyStockpile.CheckReservations);
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("VMS.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Variety Matters Stockpile";
    }

    public static bool StackSpaceAvailable(ref bool firstStack, int sizeLimit, IntVec3 c, Map map, Thing thing)
    {
        //Log.Message (thing.Label + "'s stack size limit is " + sizeLimit);
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
                (!ModSettings_VarietyStockpile.LimitNonStackables ||
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

        if (!ModSettings_VarietyStockpile.CheckReservations || map.reservationManager == null)
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
                (!ModSettings_VarietyStockpile.LimitNonStackables ||
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