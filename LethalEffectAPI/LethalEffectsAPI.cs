using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalEffectsAPI.Debug;
using LethalEffectsAPI.Examples;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;

namespace LethalEffectsAPI
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid)]
    public class LethalEffectsAPI : BaseUnityPlugin
    {
        public static LethalEffectsAPI Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            Patch();
            RegisterEffects();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll(typeof(Debug.Patches));

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }

        private static void RegisterEffects()
        {
            PoisonEffect poisonEffect = new PoisonEffect();
            EffectManager.RegisterEffect(poisonEffect.GetType(), poisonEffect.ID);
        }
    }
}
