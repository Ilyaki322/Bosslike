using UnityEngine;

public class StunCommand : ICommand
{
    int m_duration;
    float m_timer;

    public StunCommand(int duration)
    {
        m_duration = duration;
    }

    public void Enter(UnitContext context)
    {
        Debug.Log("ENTER: stun");
        m_timer = 0;
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        if (m_timer < m_duration)
        {
            m_timer += deltaTime;
            return false;
        }
        return true;
    }

    public void Exit(UnitContext context)
    {

    }
}
