using Unity.Netcode;
using UnityEngine;

public abstract class AbilityFunction : NetworkBehaviour
{
    [SerializeField] protected Ability m_ability;

    protected virtual void Awake()
    {
        m_ability = GetComponent<Ability>();
    }

    protected virtual void Start()
    {
        m_ability.OnUse += Use;
    }

    protected abstract void Use();
    protected abstract void Init();

    public override void OnDestroy()
    {
        m_ability.OnUse -= Use;
    }
}
