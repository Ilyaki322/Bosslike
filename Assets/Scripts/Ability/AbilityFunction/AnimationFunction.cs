using UnityEngine;

public class AnimationFunction : AbilityFunction
{
    [SerializeField] AnimationData m_data;
    Animator m_abilityAnimator;
    AnimationController m_charAnimator;
    AnimationAnnouncer m_announcer;

    int m_abilityHash;
    int m_blend;

    protected override void Use() {
        m_abilityAnimator.runtimeAnimatorController = m_data.Controller;

        m_charAnimator.setAbilityBool(true);

        int sector = m_charAnimator.GetSector();
        m_abilityAnimator.SetFloat(m_blend, (float)sector / 7);
        m_abilityAnimator.SetBool(m_abilityHash, true);
    }

    private void onFinish()
    {
        m_charAnimator.setAbilityBool(false);
        m_abilityAnimator.SetBool(m_abilityHash, false);
    }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as AnimationData;

        m_abilityAnimator = transform.parent.Find("AbilityAnimation").GetComponent<Animator>();
        m_announcer = transform.parent.Find("AbilityAnimation").GetComponent<AnimationAnnouncer>();
        m_announcer.OnFinish += onFinish;
        m_abilityHash = Animator.StringToHash("Ability");
        m_blend = Animator.StringToHash("Blend");
        m_charAnimator = transform.parent.Find("Sprite").GetComponent<AnimationController>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_announcer.OnFinish -= onFinish;
    }
}
