using UnityEngine;

public class KnockFunction : AbilityFunction
{
    [SerializeField] KnockData m_data;
    BoxHitFunction m_boxhit;

    protected override void Use() { }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_boxhit = GetComponent<BoxHitFunction>();
        m_boxhit.OnDetected += knockBack;
        m_data = data as KnockData;
    }

    void knockBack(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {
            if (hit.TryGetComponent<IKnockable>(out IKnockable target))
            {
                Vector3 direction = (hit.transform.position - transform.position);
                target.Knock(direction, m_data.Force);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_boxhit.OnDetected -= knockBack;
    }
}
