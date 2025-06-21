using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(ITab_Storage), "FillTab")]
public class ITab_Storage_FillTab
{
    public static void Prefix(ITab_Storage __instance)
    {
        Mod_VarietyStockpile.currentTab = __instance;
    }

    public static void Postfix()
    {
        Mod_VarietyStockpile.currentTab = null;
    }
}