using System;
using UnityEngine;

[Serializable]
public class ProjectileData : AbilityData
{
    [field: SerializeField] public GameObject m_projectile;

    public override Type getFunction() => typeof(ProjectileFunction);
}
