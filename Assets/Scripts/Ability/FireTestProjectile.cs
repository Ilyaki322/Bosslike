using Unity.Netcode;
using UnityEngine;

public class FireTestProjectile : Ability
{
    [SerializeField] GameObject m_projectile;

    public override void Use()
    {
        if (!IsServer) return;

        Vector2 target = m_playerCombat.m_mousePosition;
        var go = m_playerCombat.m_objectPool.GetNetworkObject(m_projectile).gameObject;

        go.transform.position = transform.position + transform.forward;
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        var velocity = go.GetComponent<Rigidbody2D>().linearVelocity;
        velocity += direction * 10f;

        go.GetComponent<NetworkObject>().Spawn(true);
        var tp = go.GetComponent<TestProjectile>();
        tp.Config(m_playerCombat, 3f);
        tp.SetVelocity(velocity);
    }
}
