using UnityEngine;

public class CircleWalk : ICommand
{
    Vector3 m_center;
    float m_radius;
    float m_angularSpeed;
    float m_duration;
    float m_moveSpeed;
    float m_angle;
    float m_elapsedTime;

    public CircleWalk(
        Vector3 center,
        float radius,
        float angularSpeed,
        float duration = 5f)
    {
        m_center = center;
        m_radius = radius;
        m_angularSpeed = angularSpeed;
        m_duration = duration;
        m_angle = 0f;
        m_elapsedTime = 0f;
    }

    public void Enter(UnitContext ctx)
    {
        m_moveSpeed = ctx.moveSpeed;
        // set the starting angle so you don't snap
        Vector2 offset = (Vector2)ctx.Transform.position
                       - (Vector2)m_center;
        m_angle = offset.sqrMagnitude > 0.001f
            ? Mathf.Atan2(offset.y, offset.x)
            : 0f;

        // reset the timer
        m_elapsedTime = 0f;
    }

    public bool Execute(UnitContext ctx, float dt)
    {
        m_elapsedTime += dt;

        if (m_elapsedTime < m_duration)
        {
            m_angle += m_angularSpeed * dt;
            float x = Mathf.Cos(m_angle) * m_radius;
            float y = Mathf.Sin(m_angle) * m_radius;
            // snap position
            Vector3 newPos = new Vector3(
                m_center.x + x,
                m_center.y + y,
                ctx.Transform.position.z);

            ctx.Transform.position = newPos;
            return false;
        }
        else
        {
            ctx.Controller.EnqueueCommand(new MoveToClosest());
            return true;
        }
    }

    public void Exit(UnitContext ctx)
    {

    }
}
