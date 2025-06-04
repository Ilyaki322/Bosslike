using UnityEngine;

public class AnimationFunction : AbilityFunction
{
    [SerializeField] AnimationData m_data;
    Animator m_animator;

    int m_abilityHash;

    protected override void Use() {
        m_animator.runtimeAnimatorController = m_data.Controller;
        m_animator.SetBool(m_abilityHash, true);
    }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as AnimationData;

        m_animator = transform.parent.Find("AbilityAnimation").GetComponent<Animator>();
        var ann = transform.parent.Find("AbilityAnimation").GetComponent<AnimationAnnouncer>();
        m_abilityHash = Animator.StringToHash("Ability");
    }
}
