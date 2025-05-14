using System;
using UnityEngine;

[Serializable]
public class DamageData : AbilityData
{
    [field: SerializeField] public int Damage { get; private set; }
}
