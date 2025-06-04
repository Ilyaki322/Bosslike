using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkObject))]
public abstract class UnitController : NetworkBehaviour
{
    protected readonly List<CommandEntry> m_commands = new List<CommandEntry>();
    protected CommandEntry m_current = null;
    protected UnitContext m_ctx;

    public void Awake()
    {
        m_ctx = GetComponent<UnitContext>();
        if (m_ctx == null)
        {
            throw new System.Exception("[UnitController] UnitContext component is missing on the GameObject.");
        }
    }

    public Vector2 GetPosition { get {return m_ctx.Transform.position; } }
    public abstract float GetRotationAngle();

    public void ClearCurrentCommands()
    {
        if (m_current != null)
        {
            m_current.Command.Exit(m_ctx);
            m_current = null;
        }
    }

    public void PushCommand(ICommand cmd, bool removeOnExit)
    {
        var entry = new CommandEntry(cmd, removeOnExit);
        m_commands.Add(entry);
    }

    protected void StepCommands()
    {
        TryActivateNextCommand();
        TryEnterCurrentCommand();
        TryExecuteCurrentCommand();
    }

    private void TryActivateNextCommand()
    {
        if (m_current == null && m_commands.Count > 0)
        {
            m_current = m_commands[m_commands.Count - 1];
        }
    }

    private void TryEnterCurrentCommand()
    {
        if (m_current != null && !m_current.HasEntered)
        {
            m_current.Command.Enter(m_ctx);
            m_current.HasEntered = true;
        }
    }
    private void TryExecuteCurrentCommand()
    {
        if (m_current == null) return;

        if (!m_current.Command.Execute(m_ctx, Time.deltaTime)) return;

        m_current.Command.Exit(m_ctx);

        if (m_current.RemoveOnExit)
        {
            m_commands.Remove(m_current);
        }
        else
        {
            m_current.HasEntered = false;
        }

        m_current = null;
    }
}
