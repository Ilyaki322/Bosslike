using Unity.Netcode.Components;
using UnityEngine;

public class AnimationFunction : AbilityFunction
{
    [SerializeField] AnimationData m_data;
    NetworkAnimator m_abilityAnimator;
    AnimationController m_charAnimator;
    AnimationAnnouncer m_announcer;

    int m_abilityHash;
    int m_blend;

    protected override void Use() {
        m_charAnimator.setAbilityBool(true);

        int sector = m_charAnimator.GetSector();
        m_abilityAnimator.Animator.SetFloat(m_blend, (float)sector / 7);
        m_abilityAnimator.Animator.SetBool(m_abilityHash, true);
    }
    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as AnimationData;

        m_abilityAnimator = transform.parent.Find("AbilityAnimation").GetComponent<NetworkAnimator>();
        m_announcer = transform.parent.Find("AbilityAnimation").GetComponent<AnimationAnnouncer>();
        m_announcer.OnFinish += onFinish;
        m_abilityHash = Animator.StringToHash("Ability");
        string blend = "Ability" + m_data.Animation.ToString() + "b";
        m_blend = Animator.StringToHash(blend);
        m_charAnimator = transform.parent.Find("Sprite").GetComponent<AnimationController>();
    }

    private void onFinish()
    {
        m_charAnimator.setAbilityBool(false);
        m_abilityAnimator.Animator.SetBool(m_abilityHash, false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_announcer.OnFinish -= onFinish;
    }
}
