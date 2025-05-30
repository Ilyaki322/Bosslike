using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class Projectile : NetworkBehaviour
{
    [SerializeField] protected ProjectileDataSO m_data;
    protected Rigidbody2D m_rb;

    protected ulong m_ownerId;

    public virtual void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void Config(ulong user, Vector3 pos, Vector3 target)
    {
        m_ownerId = user;

        ConfigMovementRpc(pos, target);
        if (IsServer && m_data.HasLifetime)
        {
            StartCoroutine(ProjectileDestroyCoroutine(m_data.LifeTime));
        }
    }

    IEnumerator ProjectileDestroyCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        DestroyProjectile();
    }

    protected virtual void DestroyProjectile()
    {
        if (!NetworkObject.IsSpawned) return;
        NetworkObject.Despawn(true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void ConfigMovementRpc(Vector3 pos, Vector3 target) => ConfigMovement(pos, target);

    protected abstract void ConfigMovement(Vector3 pos, Vector3 target);
}
