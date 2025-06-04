using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class BossUnitController : UnitController
{
    private Vector2 m_prevPos;
    private Vector2 m_moveDirection;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        var locator = NetworkManager.Singleton.GetComponent<PlayerLocator>();
        m_ctx.PlayerLocator = locator;
        m_prevPos = m_ctx.transform.position;
        var center = (Vector2)transform.position;
        PushCommand(new CircleWalk(center, 3f, Mathf.PI / 4f), true);
    }

    private void calculatePositions()
    {
        Vector2 currentPos = m_ctx.transform.position;
        Vector2 delta = currentPos - m_prevPos;

        if (delta.sqrMagnitude > 0.01f)
        {
            m_moveDirection = delta.normalized;
        }

        m_prevPos = currentPos;
    }

    public override float GetRotationAngle()
    {
        if(m_moveDirection.sqrMagnitude > 0.01f)
            return Mathf.Atan2(m_moveDirection.y, m_moveDirection.x) * Mathf.Rad2Deg;
        
        return 0f;
    }

    void Update()
    {
        if (!IsServer) return;
        StepCommands();
        executeCommand();
    }
}
