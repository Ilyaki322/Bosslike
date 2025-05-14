using UnityEngine;

public class DamageFunction : AbilityFunction
{
    [SerializeField] DamageData m_data;
    [SerializeField] BoxHitFunction m_boxhit;

    protected override void Start()
    {
        base.Start();
        m_boxhit.OnDetected += dealDamage;
    }
    protected override void Use() {}

    void dealDamage(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {
            if(hit.TryGetComponent<Healthbar_Network>(out Healthbar_Network hb))
            {
                hb.TakeDamage(5f, 0); // fix user hb.TakeDamage(5f, user);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_boxhit.OnDetected -= dealDamage;
    }
}
