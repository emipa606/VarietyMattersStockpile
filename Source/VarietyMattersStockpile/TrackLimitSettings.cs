using System.Collections.Generic;
using RimWorld;

namespace VarietyMattersStockpile;

public class TrackLimitSettings
{
    private static readonly Dictionary<StorageSettings, StorageLimits> limitSettings = new();

    public static StorageLimits GetLimitSettings(StorageSettings settings)
    {
        return limitSettings.TryGetValue(settings, out var setting) ? setting : new StorageLimits();
    }

    public static void SetLimitSettings(StorageSettings settings, StorageLimits newSettings)
    {
        limitSettings[settings] = newSettings;
    }
}