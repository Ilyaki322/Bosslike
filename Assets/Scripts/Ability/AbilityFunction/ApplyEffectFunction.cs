using UnityEngine;

public class ApplyEffectFunction : AbilityFunction
{
    [SerializeField] ApplyEffectData m_data;
    BoxHitFunction m_boxhit;

    protected override void Use() { }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_boxhit = GetComponent<BoxHitFunction>();
        m_boxhit.OnDetected += applyEffect;
        m_data = data as ApplyEffectData;
    }

    void applyEffect(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {
            if (hit.TryGetComponent<StatusEffectManager>(out StatusEffectManager em))
            {
                em.AddEffect(m_data.Effect.Init(hit.gameObject, m_ability.GetUser()));
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_boxhit.OnDetected -= applyEffect;
    }
}
