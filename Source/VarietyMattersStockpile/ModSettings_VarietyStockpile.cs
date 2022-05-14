using Verse;

namespace VarietyMattersStockpile;

internal class ModSettings_VarietyStockpile : ModSettings
{
    public static bool limitNonStackables = true;
    public static bool checkReservations = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref limitNonStackables, "limitNonStackables", true);
        Scribe_Values.Look(ref checkReservations, "checkReservations", true);
    }
}