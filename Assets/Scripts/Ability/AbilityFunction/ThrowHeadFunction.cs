using Unity.Netcode;
using UnityEngine;

public class ThrowHeadFunction : AbilityFunction
{
    ThrowHeadData m_data;
    PlayerCombat m_playerCombat;
    NetworkObjectPool m_pool;
    BossBigBrain m_brain;
    AnimationFunction m_anim;

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as ThrowHeadData;
        m_pool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        m_brain = transform.parent.GetComponent<BossBigBrain>();
        m_anim = GetComponent<AnimationFunction>();
    }

    protected override void Use()
    {
        var go = m_pool.GetNetworkObject(m_data.m_projectile);
        var head = go.GetComponent<HeadProjectile>();
        head.GetComponent<NetworkObject>().Spawn(true);
        head.ConfigMovement(m_brain.transform.position, m_brain.GetTarget(), m_brain.transform, endAbility);
    }

    private void endAbility()
    {
        m_ability.HasEnded = true;
        m_anim.onFinish();
    }
}
