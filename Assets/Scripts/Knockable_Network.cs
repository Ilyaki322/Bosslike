using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Knockable_Network : NetworkBehaviour, IKnockable
{
    [SerializeField] private float m_knockTime = 1.0f;
    private Vector3 m_direction;
    private float m_force;

    public void Knock(Vector3 knockVector, float force)
    {
        knockRpc(knockVector, force);
    }

    [Rpc(SendTo.Server)]
    private void knockRpc(Vector3 knockVector, float force)
    {
        m_direction = knockVector.normalized;
        m_force = force;
        StartCoroutine(knockOverTime());
    }

    IEnumerator knockOverTime()
    {
        float time = 0f;
        while (time < m_knockTime)
        {
            time += Time.deltaTime;
            transform.position += m_force * Time.deltaTime * m_direction;
            yield return null;
        }
    }
}
