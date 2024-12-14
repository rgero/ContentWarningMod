using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

// RoymondCWPlugin is mostly an example of how to use the modding API, not an actual serious mod.
// It adds a setting to the Mods settings page, which is a slider from 0 to 100.
// It then edits the Flashlight.Update method to prevent the battery of the flashlight
// from falling below that setting value.

namespace RoymondCWPlugin;

// The first argument is the GUID for this mod. This must be globally unique across all mods.
// Consider prefixing your name/etc. to the GUID. (or generate an actual GUID)
[ContentWarningPlugin("RoysPlayingMod", "0.1", false)]
public class PluginTy
{
    static PluginTy()
    {
        // Static constructors of types marked with ContentWarningPluginAttribute are automatically invoked on load.
        // Register callbacks, construct stuff, etc. here.
        Debug.Log("Hello Roy!");
        // Adding the [ContentWarningSetting] attribute to a setting class is basically the same as:
        // GameHandler.Instance.SettingsHandler.AddSetting(new ExampleSetting());
    }
}

// Harmony patches are automatically applied by the vanilla modloader.
// If 0Harmony.dll is already loaded by BepInEx, then BepInEx's harmony will be used instead.
[HarmonyPatch(typeof(Flashlight))]
[HarmonyPatch("Update")]
public class Patch
{
    static AccessTools.FieldRef<Flashlight, BatteryEntry> batteryEntry =
        AccessTools.FieldRefAccess<Flashlight, BatteryEntry>("m_batteryEntry");
    static ExampleSetting? exampleSetting;

    static bool Prefix(Flashlight __instance)
    {
        exampleSetting ??= GameHandler.Instance.SettingsHandler.GetSetting<ExampleSetting>();
        var bat = batteryEntry(__instance);
        bat.m_charge = Mathf.Max(bat.m_charge, bat.m_maxCharge * (exampleSetting.Value / 100));
        return true;
    }
}

// Don't forget to inherit from IExposedSetting too!
[ContentWarningSetting]
public class ExampleSetting : FloatSetting, IExposedSetting {
    public override void ApplyValue() => Debug.Log($"omg, mod setting changed to {Value}");
    protected override float GetDefaultValue() => 100;
    protected override float2 GetMinMaxValue() => new(0, 100);
    // Prefer using the Mods category
    public SettingCategory GetSettingCategory() => SettingCategory.Mods;
    public string GetDisplayName() => "Roy's Playing around with this Modding stuff";
}
