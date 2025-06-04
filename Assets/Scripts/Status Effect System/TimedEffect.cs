using UnityEngine;

public abstract class TimedEffect
{
    private float m_timeSinceLastTick = 0f;

    protected float m_tickRate = 0.5f;
    protected float m_duration;
    protected int m_stacks;

    public StatusEffectSO Buff { get; }
    protected readonly GameObject Obj;
    public bool IsFinished;

    public ulong UserID { get; private set; }

    public TimedEffect(StatusEffectSO buff, GameObject obj, ulong u)
    {
        Buff = buff;
        Obj = obj;
        UserID = u;
    }

    public void Tick(float delta)
    {
        m_duration -= delta;
        m_timeSinceLastTick += delta;

        if (m_timeSinceLastTick >= m_tickRate)
        {
            ApplyTick();
            m_timeSinceLastTick = 0;
        }
        if (m_duration <= 0)
        {
            End();
            IsFinished = true;
        }
    }

    public void Activate()
    {
        if (Buff.IsEffectStacked || m_duration <= 0)
        {
            ApplyEffect();
            m_stacks++;
        }

        if (Buff.IsDurationStacked || m_duration <= 0)
        {
            m_duration += Buff.Duration;
        }
    }

    protected abstract void ApplyEffect();
    protected abstract void ApplyTick();
    public abstract void End();
}
