using UnityEngine;
using Unity.Netcode;

public class GwerbobProjectile : Projectile
{
    [SerializeField] float m_offset;
    Vector2 m_middlePoint;
    Vector2 m_target;
    Vector2 m_start;

    float m_time;

    bool m_back = false;
    Transform m_backTransform = null;

    protected override void ConfigMovement(Vector3 pos, Vector3 target)
    {
        m_target = (Vector2)target;
        m_start = (Vector2)pos;
        transform.position = pos;
        m_time = 0;
        Vector2 direction = ((Vector2)target - (Vector2)pos) / 2;

        // throw left or right
        int left = UnityEngine.Random.Range(0, 2);
        if (left == 0) m_middlePoint = new Vector2(direction.y, -direction.x);
        else m_middlePoint = new Vector2(-direction.y, direction.x);
    }

    private void Update()
    {
        if (!IsServer) return;

        m_time += Time.deltaTime;
        if (m_time < m_data.LifeTime) {
            float t = m_time / m_data.LifeTime;
            transform.position = Mathf.Pow((1 - t), 2) * m_start + 2 * (1 - t) * t * m_middlePoint + Mathf.Pow(t, 2) * m_target;
            return;
        }

        if (!m_back) DestroyProjectile();
        if (m_backTransform == null)
        {
            m_backTransform = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(m_ownerId).transform;
            if (m_backTransform == null)
            {
                DestroyRpc();
                return;
            }
        }
        
        Vector2 direction = ((Vector2)m_backTransform.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Vector2 velocity = direction * m_data.Speed;
        m_rb.linearVelocity = velocity;

        if (Vector2.Distance(m_backTransform.position, transform.position) < 0.2f) DestroyRpc();
    }

    [Rpc(SendTo.Server)]
    public void DestroyRpc() => base.DestroyProjectile();

    protected override void DestroyProjectile()
    {
        if (!NetworkObject.IsSpawned) return;
        m_back = true;
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
        }

        DestroyProjectile();
    }
}
