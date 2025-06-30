using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Windows;

public class Slime : NetworkBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private int m_dmg;

    private Pathfinder m_pathfinder;
    private Rigidbody2D m_rb;
    private Animator m_animator;
    private Healthbar_Network m_hp;
    private int m_animMove;

    private int m_frameCounter = 0;
    private int m_frameUpdate = 60;

    private float m_dmgInterval = 0.5f;
    private float m_dmgIntervalTimer = 0f;
    private bool m_intervalOn = false;

    Vector3 m_movement;
    GameObject m_target;

    private void Awake()
    {
        m_hp = GetComponent<Healthbar_Network>();
        m_animMove = Animator.StringToHash("isWalking");
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    public override void OnNetworkSpawn()
    {
        m_movement = m_pathfinder.GetFlowDirection(transform.position);
        m_target = m_pathfinder.GetTarget();
    }

    private void Update()
    {
        if (m_hp.CurrHP.Value <= 0) NetworkObject.Despawn(true);
        m_frameCounter++;
        if (m_frameCounter >= m_frameUpdate)
        {
            m_frameCounter = 0;
            m_movement = m_pathfinder.GetFlowDirection(transform.position);
            m_target = m_pathfinder.GetTarget();
        }

        move(Time.deltaTime);
        if (!m_intervalOn) attack();

        if (m_intervalOn) m_dmgIntervalTimer -= Time.deltaTime;
        if (m_dmgIntervalTimer < 0)
        {
            m_intervalOn = false;
            m_dmgIntervalTimer = m_dmgInterval;
        }
    }

    private void attack()
    {
        if (m_target == null) return;

        if (Vector3.Distance(transform.position, m_target.transform.position) < 1f)
        {
            if (m_target.TryGetComponent<Healthbar_Network>(out Healthbar_Network hb))
            {
                hb.TakeDamage(m_dmg, 0); // change to get user somehow
                m_intervalOn = true;
            }
        }
    }

    private void move(float delta)
    {
        if (m_movement.sqrMagnitude > 0.1f)
        {
            m_rb.linearVelocity = m_movement * m_speed;
            m_animator.SetBool(m_animMove, true);
        }
        else
        {
            m_rb.linearVelocity = Vector2.zero;
            m_animator.SetBool(m_animMove, false);
        }
    }
}
