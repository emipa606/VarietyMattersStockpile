using System.Collections.Generic;
using RimWorld;

namespace VarietyMattersStockpile;

public class TrackLimitSettings
{
    private static readonly Dictionary<StorageSettings, StorageLimits> limitSettings =
        new Dictionary<StorageSettings, StorageLimits>();

    public static StorageLimits GetLimitSettings(StorageSettings settings)
    {
        if (limitSettings.ContainsKey(settings))
        {
            return limitSettings[settings];
        }

        return new StorageLimits();
    }

    public static void SetLimitSettings(StorageSettings settings, StorageLimits newSettings)
    {
        if (limitSettings.ContainsKey(settings))
        {
            limitSettings[settings] = newSettings;
        }
        else
        {
            limitSettings.Add(settings, newSettings);
        }
    }
}