using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TestProjectile : NetworkBehaviour
{
    //[SerializeField] float m_speed = 5f;
    [SerializeField] float m_damage = 10f;
    [SerializeField] Rigidbody2D m_rb;

    PlayerCombat m_owner;
    ulong m_ownerId;

    public void Config(PlayerCombat owner, float lifetime, ulong user)
    {
        m_owner = owner;
        m_ownerId = user;

        if (IsServer)
        {
            StartCoroutine(ProjectileDestroyCoroutine(lifetime));
        }
    }

    IEnumerator ProjectileDestroyCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (!NetworkObject.IsSpawned) return;
        NetworkObject.Despawn(true);
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (IsServer)
        {
            m_rb.linearVelocity = velocity;
            ClientSetVelocityRpc(velocity);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ClientSetVelocityRpc(Vector2 velocity)
    {
        if (!IsHost)
        {
            m_rb.linearVelocity = velocity;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    var other = collision.gameObject;

    //    if (other.TryGetComponent<Healthbar_Network>(out var enemy))
    //    {
    //        enemy.TakeDamage(m_damage);
    //        DestroyProjectile();
    //        return;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer || !NetworkObject.IsSpawned)
        {
            return;
        }

        var other = collision.gameObject;

        if (other.TryGetComponent<Healthbar_Network>(out var enemy))
        {
            enemy.TakeDamage(m_damage, m_ownerId);
            DestroyProjectile();
            return;
        }
    }
}
