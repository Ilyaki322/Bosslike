using System;
using UnityEngine;

public class BoxHitOnTargetFunction : AbilityFunction, IDamageCollider
{
    public event Action<Collider2D[]> OnDetected;

    [SerializeField] BoxHitOnTargetData m_data;

    Collider2D[] m_detected;
    Vector2 m_offset;

    UnitController m_movement;
    BossBigBrain m_bigBrain;

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_movement = GetComponentInParent<UnitController>();
        m_data = data as BoxHitOnTargetData;
        m_bigBrain = GetComponentInParent<BossBigBrain>();
    }

    protected override void Use()
    {
        Vector3 target = m_bigBrain.GetTarget();
        if (Vector3.Distance(m_bigBrain.transform.position, target) < m_data.Range)
        {
            m_offset = target;
        }
        else
        {
            m_offset = (target - m_bigBrain.transform.position).normalized * m_data.Range;
        }

        m_detected = Physics2D.OverlapBoxAll(m_offset, m_data.HitBox.size, m_movement.GetRotationAngle(), m_data.LayerM);

        if (m_detected.Length == 0)
        {
            m_ability.HasEnded = true;
            return;
        }

        OnDetected?.Invoke(m_detected);
    }

    private void OnDrawGizmosSelected()
    {
        if ((m_data == null || m_movement == null) && !m_data.Debug) return;

        Vector3 position = m_offset;
        Quaternion rotation = Quaternion.Euler(0, 0, m_movement.GetRotationAngle());

        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, m_data.HitBox.size);

        Gizmos.matrix = originalMatrix;
    }
}
