using System;
using UnityEngine;

public class KnockData : AbilityData
{
    [field: SerializeField] public float Force { get; private set; }
    public override Type getFunction() => typeof(KnockFunction);
}
