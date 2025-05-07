using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "TestProjectileAbility", menuName = "Ability/TestProjectileAbility")]
public class TestProjectileSO : AbilitySO
{
    [SerializeField] GameObject m_projectile;

    public override void Use(PlayerCombat pc, ulong user)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Vector3 spawnPosition = pc.transform.position;
        Vector2 target = pc.m_mousePosition;
        var go = pc.m_objectPool.GetNetworkObject(m_projectile).gameObject;

        go.transform.position = spawnPosition;
        Vector2 direction = (target - (Vector2)spawnPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        var velocity = go.GetComponent<Rigidbody2D>().linearVelocity;
        velocity += direction * 10f;

        go.GetComponent<NetworkObject>().Spawn(true);
        var tp = go.GetComponent<TestProjectile>();
        tp.Config(pc, 3f, user);
        tp.SetVelocity(velocity);
    }
}
