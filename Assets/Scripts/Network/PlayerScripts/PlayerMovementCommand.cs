using UnityEngine;

public class PlayerMovementCommand : ICommand
{
    private Camera m_cam;
    private PlayerInputManager m_input;
    private Rigidbody2D m_rb;
    private Transform m_transform;
    private float m_speed = 5f;

    public void Enter(UnitContext context)
    {
        m_cam = Camera.main;
        m_input = context.Controller.GetComponent<PlayerInputManager>();
        m_rb = context.GetComponent<Rigidbody2D>();
        m_transform = context.Transform;
        m_speed = context.MoveSpeed;
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        ProcessMovement();
        LookAtMouse(context);
        return false;
    }

    public void Exit(UnitContext context)
    {
        if (m_rb != null)
        {
            m_rb.linearVelocity = Vector2.zero;
        }
    }

    private void ProcessMovement()
    {
        if (m_input.moveUpdate.sqrMagnitude > 0.1f)
        {
            m_rb.linearVelocity = m_input.moveUpdate.normalized * m_speed;
        }
        else m_rb.linearVelocity = Vector2.zero;
    }

    private void LookAtMouse(UnitContext context)
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_cam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mouseWorldPos - context.Transform.position;

        if (direction == Vector2.zero) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg * Time.deltaTime;
        context.Transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
