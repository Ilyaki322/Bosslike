using UnityEngine;

public class RotateToTargetCommand : ICommand
{
    Transform m_target;
    float m_rotationSpeed;

    public RotateToTargetCommand(Transform target, float speed)
    {
        m_target = target;
        m_rotationSpeed = speed;
    }

    public void Enter(UnitContext context)
    {
        Debug.Log("ENTER: Rotate to target");
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        if (m_target == null) return true;

        Transform self = context.Transform;
        Vector3 direction = (m_target.position - self.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero) return true;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        self.rotation = Quaternion.RotateTowards(self.rotation, targetRotation, m_rotationSpeed * deltaTime);

        float angle = Quaternion.Angle(self.rotation, targetRotation);
        return !(angle < 5f);
    }

    public void Exit(UnitContext context)
    {
        
    }
}
