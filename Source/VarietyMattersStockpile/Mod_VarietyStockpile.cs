using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace VarietyMattersStockpile;

public class Mod_VarietyStockpile : Mod
{
    public Mod_VarietyStockpile(ModContentPack content) : base(content)
    {
        //Log.Message("Stockpiles patched for variety");
        var harmony = new Harmony("cozarkian.varietymattersstockpile");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        GetSettings<ModSettings_VarietyStockpile>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var rect = new Rect(260f, 50f, inRect.width * .4f, inRect.height);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.CheckboxLabeled("VMS.SingleDuplicates".Translate(),
            ref ModSettings_VarietyStockpile.limitNonStackables);
        listingStandard.CheckboxLabeled("VMS.CheckReservations".Translate(),
            ref ModSettings_VarietyStockpile.checkReservations);
        listingStandard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Variety Matters Stockpile";
    }
}