using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.Copy))]
public static class StorageSettingsClipboard_Copy
{
    public static void Postfix(StorageSettings s, StorageSettings ___clipboard)
    {
        StorageLimits.SetLimitSettings(___clipboard, StorageLimits.GetLimitSettings(s));
    }
}