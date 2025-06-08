using Unity.Netcode;
using UnityEngine;

public class StunFunction : AbilityFunction
{
    [SerializeField] StunData m_data;
    IDamageCollider m_dmgCollider;

    protected override void Use() { }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_dmgCollider = GetComponent<IDamageCollider>();
        m_dmgCollider.OnDetected += stun;

        m_data = data as StunData;
    }

    void stun(Collider2D[] targets)
    {
        foreach (Collider2D hit in targets)
        {

            if (hit.TryGetComponent<UnitController>(out UnitController uc))
            {
                uc.PushCommand(new StunCommand(m_data.Duration), true);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_dmgCollider.OnDetected -= stun;
    }
}
