using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(Zone_Stockpile), nameof(Zone_Stockpile.AddCell))]
public class Zone_Stockpile_AddCell
{
    public static void Postfix(Zone_Stockpile __instance)
    {
        var slotGroup = __instance.GetSlotGroup();
        if (slotGroup != null)
        {
            StorageLimits.GetLimitSettings(slotGroup.Settings).NeedsFilled = true;
        }
    }
}