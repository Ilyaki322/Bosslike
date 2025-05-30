using System;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;

public class TestProjectile : Projectile
{
    protected override void ConfigMovement(Vector3 pos, Vector3 target)
    {
        transform.position = pos;
        Vector2 direction = ((Vector2)target - (Vector2)pos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Vector2 velocity = direction * m_data.Speed;
        m_rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer || !NetworkObject.IsSpawned)
        {
            return;
        }

        var other = collision.gameObject;

        if (other.TryGetComponent<Healthbar_Network>(out var enemy))
        {
            enemy.TakeDamage(m_data.Damage, m_ownerId);
            DestroyProjectile();
            return;
        }
    }
}
