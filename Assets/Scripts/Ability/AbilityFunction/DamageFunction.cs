using Unity.Netcode;
using UnityEngine;

public class DamageFunction : AbilityFunction
{
    [SerializeField] DamageData m_data;
    IDamageCollider m_dmgCollider;

    protected override void Use() {}

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_dmgCollider = GetComponent<IDamageCollider>();
        m_dmgCollider.OnDetected += dealDamage;
        
        m_data = data as DamageData;
    }

    void dealDamage(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {
            if (hit.TryGetComponent<Healthbar_Network>(out Healthbar_Network hb))
            {
                hb.TakeDamage(m_data.Damage, m_ability.GetUser());
            }
        }

        m_ability.HasEnded = true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_dmgCollider.OnDetected -= dealDamage;
    }
}
