using UnityEngine;

public class PlayerUnitController : UnitController
{
    [SerializeField] private Camera m_cam;

    public override float GetRotationAngle()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_cam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mouseWorldPos - m_ctx.transform.position;

        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        PushCommand(new PlayerMovementCommand(), false);
    }

    private void FixedUpdate()
    {
        StepCommands();
    }
}
