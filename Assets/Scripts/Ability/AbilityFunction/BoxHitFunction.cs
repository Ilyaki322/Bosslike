using System;
using UnityEngine;

public class BoxHitFunction : AbilityFunction
{
    public event Action<Collider2D[]> OnDetected;

    [SerializeField] BoxHitData m_data;

    Collider2D[] m_detected;
    Vector2 m_offset;

    PlayerMovement_Network m_movement;

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_movement = GetComponent<PlayerMovement_Network>();
        m_data = data as BoxHitData;
    }

    protected override void Use()
    {
        float angle = m_movement.GetRotationAngle();
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector2 rotatedCenter = rotation * m_data.m_hitbox.center;

        m_offset = (Vector2)transform.position + rotatedCenter;
        m_detected = Physics2D.OverlapBoxAll(m_offset, m_data.m_hitbox.size, m_movement.GetRotationAngle(), m_data.m_hitLayerMask);

        if (m_detected.Length == 0) return;

        OnDetected?.Invoke(m_detected);
    }

    private void OnDrawGizmosSelected()
    {
        if ((m_data == null || m_movement == null) && !m_data.m_debug) return;

        Vector3 position = m_offset;
        Quaternion rotation = Quaternion.Euler(0, 0, m_movement.GetRotationAngle());

        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, m_data.m_hitbox.size);

        Gizmos.matrix = originalMatrix;
    }
}
