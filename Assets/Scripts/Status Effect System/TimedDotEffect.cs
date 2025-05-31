using UnityEngine;

public class TimedDotEffect : TimedEffect
{
    private readonly Healthbar_Network m_hpBar;

    DotEffect m_data;

    public TimedDotEffect(DotEffect buff, GameObject obj) : base(buff, obj)
    {
        m_data = buff;
        m_hpBar = obj.GetComponent<Healthbar_Network>();
    }

    public override void End() {}

    protected override void ApplyEffect() {}

    protected override void ApplyTick()
    {
        m_hpBar.TakeDamage(m_data.DamagePerTick, 0);
    }
}
