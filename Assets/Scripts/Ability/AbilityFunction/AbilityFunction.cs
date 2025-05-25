using Unity.Netcode;
using UnityEngine;

public abstract class AbilityFunction : NetworkBehaviour
{
    [SerializeField] protected Ability m_ability;

    protected abstract void Use();

    public virtual void Init(AbilityData d)
    {
        m_ability = GetComponent<Ability>();
        m_ability.OnUse += Use;
    }

    public override void OnDestroy()
    {
        m_ability.OnUse -= Use;
    }
}
