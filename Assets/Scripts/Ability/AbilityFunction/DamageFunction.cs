using UnityEngine;

public class DamageFunction : AbilityFunction
{
    [SerializeField] DamageData m_data;
    BoxHitFunction m_boxhit;

    protected override void Use() {}

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_boxhit = GetComponent<BoxHitFunction>();
        m_boxhit.OnDetected += dealDamage;
        m_data = data as DamageData;
    }

    void dealDamage(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {
            if(hit.TryGetComponent<Healthbar_Network>(out Healthbar_Network hb))
            {
                hb.TakeDamage(m_data.Damage, 0); // fix user hb.TakeDamage(5f, user);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_boxhit.OnDetected -= dealDamage;
    }
}
