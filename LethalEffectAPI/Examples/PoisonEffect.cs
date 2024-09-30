using Discord;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEffectsAPI.Examples
{
    public class PoisonEffect : BaseEffect
    {
        public bool isFatal = true;

        public int playerDamage = 10;
        public int enemyDamage = 1;

        public override ulong ID => 936545836292648uL;

        public override void Initialize(UnityEngine.GameObject target)
        {
            base.Initialize(target);

            _effectInterval = 2f;
            EffectExecution = EffectExec.Owner;
        }

        public override void DoEffectInterval()
        {
            base.DoEffectInterval();
            if (_target.TryGetComponent<PlayerControllerB>(out PlayerControllerB player))
            {
                if (!isFatal && player.health - playerDamage <= 0) return;

                StartOfRound.Instance.localPlayerController.NetworkManager.LocalClientId = player.OwnerClientId;

                player.DamagePlayer(playerDamage);

                if (player.isPlayerDead || player.health <= 0f) Destroy();
            }

            else if (_target.TryGetComponent<EnemyAI>(out EnemyAI enemy))
            {
                if (!isFatal && enemy.enemyHP - enemyDamage <= 0) return;

                enemy.HitEnemy(enemyDamage);

                if (enemy.isEnemyDead || enemy.enemyHP <= 0) Destroy();
            }
        }
    }
}
