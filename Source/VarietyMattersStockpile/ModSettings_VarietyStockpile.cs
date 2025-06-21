using Verse;

namespace VarietyMattersStockpile;

internal class ModSettings_VarietyStockpile : ModSettings
{
    public static bool LimitNonStackables = true;
    public static bool CheckReservations = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref LimitNonStackables, "limitNonStackables", true);
        Scribe_Values.Look(ref CheckReservations, "checkReservations", true);
    }
}