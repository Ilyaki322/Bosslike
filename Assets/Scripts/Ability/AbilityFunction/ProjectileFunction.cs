using Unity.Netcode;
using UnityEngine;

public class ProjectileFunction : AbilityFunction
{
    ProjectileData m_data;
    PlayerCombat m_playerCombat;

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_playerCombat = GetComponentInParent<PlayerCombat>();
        m_data = data as ProjectileData;
    }

    protected override void Use()
    {
        m_playerCombat.RequestProjectile(m_data.m_projectile);
    }
}
