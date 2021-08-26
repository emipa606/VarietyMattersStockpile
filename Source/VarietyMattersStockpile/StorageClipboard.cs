using HarmonyLib;
using RimWorld;

namespace VarietyMattersStockpile
{
    [HarmonyPatch(typeof(StorageSettingsClipboard))]
    public static class StorageClipboard
    {
        [HarmonyPatch("Copy")]
        [HarmonyPostfix]
        public static void CopySettings(StorageSettings s, StorageSettings ___clipboard)
        {
            StorageLimits.SetLimitSettings(___clipboard, StorageLimits.GetLimitSettings(s));
        }

        [HarmonyPatch("PasteInto")]
        [HarmonyPostfix]
        public static void PasteSettings(StorageSettings s, StorageSettings ___clipboard)
        {
            var copySettings = StorageLimits.GetLimitSettings(___clipboard);
            StorageLimits.GetLimitSettings(s).dupStackLimit = copySettings.dupStackLimit;
            StorageLimits.GetLimitSettings(s).stackSizeLimit = copySettings.stackSizeLimit;
            StorageLimits.GetLimitSettings(s).cellFillPercentage = copySettings.cellFillPercentage;
        }
    }
}