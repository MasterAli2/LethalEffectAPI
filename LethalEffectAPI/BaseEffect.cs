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
        public float Duration => _duration;
        public float maxDuration => _maxDuration;
        public bool doTime => _doTime;
        public GameObject Target => _target;
        public float effectIntervalTimer => _effectIntervalTimer;
        public float effectInterval => _effectInterval;
        public NetworkObject networkObject => _networkObject;

        protected float _duration = 0f;
        protected float _maxDuration = 10f;
        protected bool _doTime = false;
        protected GameObject _target;
        protected float _effectIntervalTimer;
        protected float _effectInterval;

        private bool _returnNextFrame = false;
        private NetworkObject? _networkObject;


        public virtual void Initialize(GameObject target)
        {
            LethalEffectsAPI.Logger.LogDebug($"Initializing {this}.");

            _target = target;

            if (TryGetComponent(out NetworkObject networkObject)) _networkObject = networkObject;
            else _networkObject = null;
        }

        protected virtual void Update()
        {
            if (_returnNextFrame)
            {
                enabled = false;
                return;
            }

            _duration += Time.deltaTime;
            _effectIntervalTimer += Time.deltaTime;

            if (_duration >= _maxDuration && doTime) Destroy();

            if (_effectIntervalTimer >= effectInterval)
            {
                _effectIntervalTimer = 0f;
                DoEffectInterval();
            }
        }

        public virtual bool Destroy()
        {
            LethalEffectsAPI.Logger.LogDebug($"Destroying {this}.");

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
            LethalEffectsAPI.Logger.LogDebug($"Looping {this}.");
        }


        #region Debug
        public virtual void OnDisable()
        {
            LethalEffectsAPI.Logger.LogDebug($"Disabling {this}.");
        }
        public virtual void OnEnable()
        {
            LethalEffectsAPI.Logger.LogDebug($"Enabling {this}.");
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
