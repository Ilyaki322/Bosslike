using System;
using UnityEngine;

public class PossionEruptionData : AbilityData
{
    [field: SerializeField] public GameObject Projectile;
    [field: SerializeField] public float Radius;
    [field: SerializeField] public float MinRange;
    [field: SerializeField] public int ProjectileAmount;

    public override Type getFunction() => typeof(PossionEruptionFunction);
}
