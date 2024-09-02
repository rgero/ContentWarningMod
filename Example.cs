using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

namespace ExampleCWPlugin;

[ContentWarningPlugin("ExampleCWPlugin", "0.1", false)]
public class PluginTy
{
    static PluginTy()
    {
        Debug.Log("Hello from plugin! This is called on plugin load");
        // Adding the [ContentWarningSetting] attribute to a setting class is basically the same as:
        // GameHandler.Instance.SettingsHandler.AddSetting(new ExampleSetting());
    }
}

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
    public string GetDisplayName() => "Example mod setting";
}
