using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    UnitController m_movement;
    NetworkAnimator m_animator;

    int m_abilityTrigger;
    int m_idle;
    int m_blend;

    public void Awake()
    {
        m_movement = GetComponentInParent<UnitController>();
        m_animator = GetComponent<NetworkAnimator>();

        m_abilityTrigger = Animator.StringToHash("Ability");
        m_idle = Animator.StringToHash("Idle");
        m_blend = Animator.StringToHash("Blend");
    }

    private void FixedUpdate()
    {
        if(!NetworkObject.IsSpawned) return;
        float angle = m_movement.GetRotationAngle();
        int sector = Mathf.FloorToInt((angle + 360) % 360 / 45f);

        m_animator.Animator.SetFloat(m_blend, (float)sector / 7);
    }

    public void setAbilityBool(bool b)
    {
        m_animator.Animator.SetBool(m_idle, !b);
        m_animator.Animator.SetBool(m_abilityTrigger, b);
    }
}
