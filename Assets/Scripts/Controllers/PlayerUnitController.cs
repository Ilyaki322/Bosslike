using UnityEngine;

public class PlayerUnitController : UnitController
{
    [SerializeField] private Camera m_cam;
    private MenuManager m_menuManager;
    private Healthbar_Network m_hpbar;

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

        m_hpbar = GetComponent<Healthbar_Network>();
        m_menuManager = GameObject.Find("Menu").GetComponent<MenuManager>();
        PushCommand(new PlayerMovementCommand(), false);
    }

    private void FixedUpdate()
    {
        if (m_hpbar.CurrHP.Value <= 0) m_menuManager.Gameover("Game Over", gameObject);
        StepCommands();
    }
}
