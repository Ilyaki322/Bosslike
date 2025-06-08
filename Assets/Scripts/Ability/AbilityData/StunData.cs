using System;
using UnityEngine;

[Serializable]
public class StunData : AbilityData
{
    [field: SerializeField] public int Duration { get; private set; }

    public override Type getFunction()
    {
        return typeof(StunFunction);
    }
}
