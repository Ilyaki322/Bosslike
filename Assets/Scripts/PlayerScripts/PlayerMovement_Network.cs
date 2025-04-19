using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Network : NetworkBehaviour {

    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private Transform m_player;
    [SerializeField] private Camera m_cam;

    private Vector2 m_moveUpdate;

    private void FixedUpdate() {
        ProcessMovement();
        LookAtMouse();
    }

    private void ProcessMovement()
    {
        if (m_moveUpdate.sqrMagnitude > 0.1f) {
            m_rb.linearVelocity = m_moveUpdate.normalized * m_speed;
        }
        else m_rb.linearVelocity = Vector2.zero;

        Debug.DrawLine(transform.position, transform.position + new Vector3(m_rb.linearVelocity.x, m_rb.linearVelocity.y, 0));
    }

    private void LookAtMouse()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_cam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mouseWorldPos - m_player.position;

        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            m_player.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void OnMove(InputValue context)
    {
        m_moveUpdate = context.Get<Vector2>();
    }
}
