using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    //PlayerMovement_Network m_movement;
    //NetworkAnimator m_animator;

    //int m_LookNE;
    //int m_LookWS;
    //int m_LookSE;
    //int m_LookNW;


    //public void Awake()
    //{
    //    m_movement = GetComponentInParent<PlayerMovement_Network>();
    //    m_animator = GetComponent<NetworkAnimator>();

    //    m_LookNE = Animator.StringToHash("LookNE");
    //    m_LookWS = Animator.StringToHash("LookWS");
    //    m_LookSE = Animator.StringToHash("LookSE");
    //    m_LookNW = Animator.StringToHash("LookNW");
    //}

    //private void FixedUpdate()
    //{
    //    float angle = m_movement.GetRotationAngle();
    //    int sector = Mathf.FloorToInt((angle + 360) % 360 / 45f);

    //    if (sector == 6 || sector == 7) m_animator.SetTrigger(m_LookSE);
    //    if (sector == 0 || sector == 1) m_animator.SetTrigger(m_LookNE);
    //    if (sector == 2 || sector == 3) m_animator.SetTrigger(m_LookNW);
    //    if (sector == 4 || sector == 5) m_animator.SetTrigger(m_LookWS);
    //}

    PlayerMovement_Network m_movement;
    NetworkAnimator m_animator;

    int m_abilityTrigger;
    int m_idle;
    int m_blend;


    public void Awake()
    {
        m_movement = GetComponentInParent<PlayerMovement_Network>();
        m_animator = GetComponent<NetworkAnimator>();

        m_abilityTrigger = Animator.StringToHash("Ability");
        m_idle = Animator.StringToHash("Idle");
        m_blend = Animator.StringToHash("Blend");
    }

    private void FixedUpdate()
    {
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
