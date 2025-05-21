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

    //protected override void Use()
    //{
    //    Vector3 spawnPosition = transform.position;
    //    Vector2 target = m_playerCombat.m_mousePosition;
    //    var go = m_playerCombat.m_objectPool.GetNetworkObject(m_data.m_projectile).gameObject;

    //    go.transform.position = spawnPosition;
    //    Vector2 direction = (target - (Vector2)spawnPosition).normalized;
    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    //    go.transform.rotation = Quaternion.Euler(0, 0, angle);

    //    var velocity = go.GetComponent<Rigidbody2D>().linearVelocity;
    //    velocity += direction * 10f;

    //    go.GetComponent<NetworkObject>().Spawn(true);
    //    var tp = go.GetComponent<TestProjectile>();
    //    tp.Config(m_playerCombat, 3f, m_ability.GetUser());
    //    tp.SetVelocity(velocity);
    //}
    protected override void Use()
    {
        m_playerCombat.RequestProjectile(m_data.m_projectile);
    }
}
