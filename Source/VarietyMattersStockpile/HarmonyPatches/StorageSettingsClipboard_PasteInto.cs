using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile;

[HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.PasteInto))]
public static class StorageSettingsClipboard_PasteInto
{
    public static void Postfix(StorageSettings s, StorageSettings ___clipboard)
    {
        var copySettings = StorageLimits.GetLimitSettings(___clipboard);
        StorageLimits.GetLimitSettings(s).DupStackLimit = copySettings.DupStackLimit;
        StorageLimits.GetLimitSettings(s).StackSizeLimit = copySettings.StackSizeLimit;
        StorageLimits.GetLimitSettings(s).CellFillPercentage = copySettings.CellFillPercentage;
    }
}