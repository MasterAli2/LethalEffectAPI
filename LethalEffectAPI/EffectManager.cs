using HarmonyLib;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Reflection.Emit;

namespace LethalEffectsAPI
{
    [StaticNetcodeLib.StaticNetcode]
    public static class EffectManager
    {
        public static Dictionary<ulong, Type> EffectRegistry = new Dictionary<ulong, Type>();

        #region AddEffect
        public static BaseEffect AddEffect(Type type, GameObject target)
        {
            BaseEffect effect = (BaseEffect)target.AddComponent(type);
            effect.Initialize(target.gameObject);
            return effect;
        }
        public static BaseEffect AddEffect(ulong id, GameObject target)
        {
            return AddEffect(EffectRegistry[id], target);
        }
        public static T AddEffect<T>(GameObject target) where T : BaseEffect
        {
            return (T)AddEffect(typeof(T), target);
        }
        #endregion

        #region DestroyEffect
        public static void DestroyEffect(Type type, GameObject target)
        {
            foreach (Component component in target.GetComponents(type))
            {
                LethalEffectsAPI.Logger.LogDebug($"Destroying {component}.");
                UnityEngine.Object.Destroy(component);
            }
        }
        public static void DestroyEffect(ulong id, GameObject target)
        {
            DestroyEffect(EffectRegistry[id], target);
        }
        public static void DestroyEffect<T>(GameObject target) where T : BaseEffect
        {
            DestroyEffect(typeof(T), target);
        }
        #endregion

        #region Networking

        public static void RegisterEffect(Type type, ulong id)
        {
            if (EffectRegistry.ContainsKey(id) && EffectRegistry[id] != null)
            {
                // check if there is confliction
                if (EffectRegistry[id] != type)
                {
                    LethalEffectsAPI.Logger.LogWarning($"Effects named {EffectRegistry[id].Name} and {type.Name} conflict over id number: {id}.");
                    LethalEffectsAPI.Logger.LogWarning("Fixing conflict. (this might break networking)");

                    RegisterEffect(type, id + 1);
                    return;
                }
                // check if effect is registered twice
                else if (EffectRegistry[id] == type)
                {
                    LethalEffectsAPI.Logger.LogWarning($"Effect named {EffectRegistry[id].Name} is registered multiple times.");
                    return;
                }
            }

            EffectRegistry[id] = type;
        }

        // AddEffect()
        [ServerRpc]
        public static void AddEffectServerRpc(ulong id, NetworkObjectReference nor)
        {
            AddEffectClientRpc(id, nor);
        }
        [ClientRpc]
        public static void AddEffectClientRpc(ulong id, NetworkObjectReference nor)
        {
            NetworkObject no = (NetworkObject)nor;
            AddEffect(id, no.gameObject);
        }

        // DestroyEffect()
        [ServerRpc]
        public static void DestroyEffectServerRpc(ulong id, NetworkObjectReference nor)
        {
            DestroyEffectClientRpc(id, nor);
        }
        [ClientRpc]
        public static void DestroyEffectClientRpc(ulong id, NetworkObjectReference nor)
        {
            NetworkObject no = (NetworkObject)nor;
            DestroyEffect(id, no.gameObject);
        }

        #endregion


        [Obsolete("This has not been implemented yet.")]
        public static IEnumerable<CodeInstruction> effectTranspilerHelperMethod(IEnumerable<CodeInstruction> original, IEnumerable<CodeInstruction> modified, IEnumerable<CodeInstruction> condition, ILGenerator generator)
        {
            throw new NotImplementedException();

            /*
            IEnumerable<CodeInstruction> final = Enumerable.Empty<CodeInstruction>();

            Label ifLabel = generator.DefineLabel();

            // add condition
            foreach(CodeInstruction instruction in condition)
            {
                final.AddItem<CodeInstruction>(instruction);
            }

            CodeInstruction ifInstruction = new CodeInstruction(opcode: OpCodes.Brfalse_S, ifLabel);
            CodeInstruction ifInstru1ction = new CodeInstruction(opcode: OpCodes.Brfalse_S) { labels = new List<Label>() { ifLabel } };
            final.AddItem<CodeInstruction>(ifInstruction);
            */
        }


        #region tools
        public static BaseEffect getInstanceFromType(Type type)
        {
            return (BaseEffect)Activator.CreateInstance(type);
        }
        public static BaseEffect getInstanceFromId(ulong id)
        {
            return getInstanceFromType(EffectRegistry[id]);
        }
        #endregion
    }
}
