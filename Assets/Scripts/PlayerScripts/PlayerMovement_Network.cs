using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Network : NetworkBehaviour {

    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private float m_speed = 5f;
    private Vector2 m_moveUpdate;

    public override void OnNetworkSpawn() {
        
    }

    private void FixedUpdate() {
        if (!IsOwner) return;

        ProcessMovement();
    }

    private void ProcessMovement()
    {
        if (m_moveUpdate.sqrMagnitude > 0.1f) {
            m_rb.linearVelocity = m_moveUpdate.normalized * m_speed;
        }
        else m_rb.linearVelocity = Vector2.zero;
    }

    public void OnMove(InputValue context)
    {
        m_moveUpdate = context.Get<Vector2>();
    }
}
