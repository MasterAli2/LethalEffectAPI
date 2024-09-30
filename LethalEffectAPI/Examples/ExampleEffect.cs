using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalEffectsAPI.Examples
{
    public class ExampleEffect : BaseEffect
    {
        public override ulong ID => 12345678910uL;

        public override void Initialize(UnityEngine.GameObject target)
        {
            base.Initialize(target);

            _effectInterval = 1f;
            EffectExecution = EffectExec.All;
        }

        public override void DoEffectInterval()
        {
            base.DoEffectInterval();
            
        }
    }
}
