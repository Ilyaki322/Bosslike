using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(NetworkObject))]
public class UnitController : NetworkBehaviour
{
    private readonly Queue<ICommand> m_commands = new Queue<ICommand>();
    private ICommand m_current;
    private UnitContext m_ctx;
    private int m_moveSpeed = 5;
    public override void OnNetworkSpawn()
    {
        // If this is a pure client, just kill or disable the script now.
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        var locator = NetworkManager.Singleton.GetComponent<PlayerLocator>();
        m_ctx = new UnitContext(transform, this, locator, m_moveSpeed);
        var center = (Vector2)transform.position;
        EnqueueCommand(new CircleWalk(center, 3f, Mathf.PI / 4f));
    }

    void Update()
    {
        if (!IsServer) return;
        getNextCommand();
        executeCommand();
    }

    private void executeCommand()
    {
        if (m_current != null)
        {
            float deltaTime = Time.deltaTime;
            if (m_current.Execute(m_ctx, deltaTime))
            {
                m_current.Exit(m_ctx);
                m_current = null;
            }
        }
    }

    public void EnqueueCommand(ICommand cmd)
    {
        m_commands.Enqueue(cmd);
    }

    public void ClearCommands()
    {
        if( m_current != null)
        {
            m_current.Exit(m_ctx);
            m_current = null;
        }
    }

    private void getNextCommand()
    {
        if (m_current == null && m_commands.Count > 0)
        {
            m_current = m_commands.Dequeue();
            m_current.Enter(m_ctx);
        }
    }
}
