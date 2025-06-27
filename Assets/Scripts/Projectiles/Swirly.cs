using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Swirly : Projectile
{
    [SerializeField] CapsuleCollider2D m_capsule;
    [SerializeField] LayerMask m_mask;
    [SerializeField] private NetworkObjectPool m_pool;
    [SerializeField] GameObject m_slimePrefab;

    public override void Awake()
    {
        base.Awake();
        m_pool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
    }

    protected override void ConfigMovement(Vector3 pos, Vector3 target)
    {
        transform.position = target;
    }

    protected override void DestroyProjectile()
    {
        List<Collider2D> hits = new();
        m_capsule.Overlap(hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].TryGetComponent<Healthbar_Network>(out var enemy))
            {
                enemy.TakeDamage(m_data.Damage, m_ownerId);
            }
        }

        if (Random.value < 0.3f)
        {
            var go = m_pool.GetNetworkObject(m_slimePrefab, transform.position, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn(true);
        }

        base.DestroyProjectile();
    }
}
