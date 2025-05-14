using Unity.Netcode.Components;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerMovement_Network m_movement;
    NetworkAnimator m_animator;

    int m_LookNE;
    int m_LookWS;
    int m_LookSE;
    int m_LookNW;


    public void Awake()
    {
        m_movement = GetComponentInParent<PlayerMovement_Network>();
        m_animator = GetComponent<NetworkAnimator>();

        m_LookNE = Animator.StringToHash("LookNE");
        m_LookWS = Animator.StringToHash("LookWS");
        m_LookSE = Animator.StringToHash("LookSE");
        m_LookNW = Animator.StringToHash("LookNW");
    }

    private void FixedUpdate()
    {
        float angle = m_movement.GetRotationAngle();
        int sector = Mathf.FloorToInt((angle + 360) % 360 / 45f);

        print(sector);

        if (sector == 6 || sector == 7) m_animator.SetTrigger(m_LookSE);
        if (sector == 0 || sector == 1) m_animator.SetTrigger(m_LookNE);
        if (sector == 2 || sector == 3) m_animator.SetTrigger(m_LookNW);
        if (sector == 4 || sector == 5) m_animator.SetTrigger(m_LookWS);
    }
}
