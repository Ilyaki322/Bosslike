using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Network : NetworkBehaviour {

    [SerializeField, Header("References")] private Rigidbody2D m_rb;
    [SerializeField] private Transform m_player;
    [SerializeField] private Camera m_cam;
    [SerializeField] private PlayerInputManager m_input;

    [Space]
    [SerializeField, Header("Properties")] private float m_speed = 5f;

    private void FixedUpdate() {
        ProcessMovement();
        LookAtMouse();
    }

    private void ProcessMovement()
    {
        if (m_input.moveUpdate.sqrMagnitude > 0.1f) {
            m_rb.linearVelocity = m_input.moveUpdate.normalized * m_speed;
        }
        else m_rb.linearVelocity = Vector2.zero;
    }

    private void LookAtMouse()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_cam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mouseWorldPos - m_player.position;

        if (direction == Vector2.zero) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        m_player.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public Vector3 GetLookDirection()
    {
        return transform.up;
    }

    public float GetRotationAngle()
    {
        return Mathf.Atan2(m_player.transform.up.y, m_player.transform.up.x) * Mathf.Rad2Deg;
    }
}
