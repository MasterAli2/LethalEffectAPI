using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LethalEffectsAPI
{
    public abstract class BaseEffect : MonoBehaviour
    {

        public EffectExec EffectExecution
        {
            set
            {
                switch ((int)value)
                {
                    case 1:
                        if (!StartOfRound.Instance.IsHost)
                        {
                            _returnNextFrame = true;
                            break;
                        }
                        _returnNextFrame = false;
                        break;
                    case 2:
                        if (StartOfRound.Instance.IsHost)
                        {
                            _returnNextFrame = true;
                            break;
                        }
                        _returnNextFrame = false;
                        break;
                    case 3:
                        if (_networkObject != null)
                        {
                            if (!_networkObject.IsOwner)
                            {
                                _returnNextFrame = true;

                                break;
                            }
                        }
                        _returnNextFrame = false;
                        break;
                    default:
                        _returnNextFrame = false;
                        break;
                }
            }
        }

        public virtual ulong ID => 0uL;
        public float effectDestructionTimer => _effectDestructionTimer;
        public float effectDestruction => _effectDestruction;
        public bool doEffectDestruction => _doEffectDestruction;
        public GameObject Target => _target;
        public float effectIntervalTimer => _effectIntervalTimer;
        public float effectInterval => _effectInterval;
        public NetworkObject? networkObject => _networkObject;

        protected float _effectDestructionTimer = 0f;
        protected float _effectDestruction = 10f;
        protected bool _doEffectDestruction = false;
        protected GameObject _target;
        protected float _effectIntervalTimer;
        protected float _effectInterval;
        protected bool _returnNextFrame = false;

        private NetworkObject? _networkObject;


        public virtual void Initialize(GameObject target)
        {
            LethalEffectAPI.Logger.LogDebug($"Initializing {this}.");

            _target = target;

            if (_target.TryGetComponent(out NetworkObject networkObject)) _networkObject = networkObject;
            else _networkObject = null;
        }

        protected virtual void Update()
        {
            if (_returnNextFrame)
            {
                enabled = false;
                return;
            }

            _effectDestructionTimer += Time.deltaTime;
            _effectIntervalTimer += Time.deltaTime;

            if (_effectDestructionTimer >= _effectDestruction && doEffectDestruction) Destroy();

            if (_effectIntervalTimer >= effectInterval)
            {
                _effectIntervalTimer = 0f;
                DoEffectInterval();
            }
        }

        public virtual bool Destroy()
        {
            LethalEffectAPI.Logger.LogDebug($"Destroying {this}.");

            if (_networkObject != null)
            {
                EffectManager.DestroyEffectServerRpc(ID, (NetworkObjectReference)_networkObject);
                return true;
            }

            Destroy(this);
            return true;
        }

        public virtual void DoEffectInterval()
        {
            LethalEffectAPI.Logger.LogDebug($"Looping {this}.");
        }


        #region Debug
        public virtual void OnDisable()
        {
            LethalEffectAPI.Logger.LogDebug($"Disabling {this}.");
        }
        public virtual void OnEnable()
        {
            LethalEffectAPI.Logger.LogDebug($"Enabling {this}.");
        }
        #endregion


        public enum EffectExec
        {
            All, // 0
            Host, // 1
            NonHost, // 2
            Owner // 3
        }
    }



}
