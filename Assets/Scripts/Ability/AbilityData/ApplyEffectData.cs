using System;
using UnityEngine;

[Serializable]
public class ApplyEffectData : AbilityData
{
    [field: SerializeReference] public StatusEffectSO Effect;

    public override Type getFunction() => typeof(ApplyEffectFunction);
}
