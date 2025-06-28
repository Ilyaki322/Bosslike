using System;
using UnityEngine;

public class ThrowHeadData : AbilityData
{
    [field: SerializeField] public GameObject m_projectile;

    public override Type getFunction() => typeof(ThrowHeadFunction);
}
