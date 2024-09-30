using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalEffectsAPI.Debug;
using LethalEffectsAPI.Examples;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;

namespace LethalEffectsAPI
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid)]
    public class LethalEffectAPI : BaseUnityPlugin
    {
        public const string GUID = "MasterAli2.LethalEffectAPI";
        public const string NAME = "LethalEffectAPI";
        public const string VERSION = "1.0.0";
        public static LethalEffectAPI Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            Patch();
            RegisterEffects();

            Logger.LogInfo($"{GUID} v{VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(GUID);

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
