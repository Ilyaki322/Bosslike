using UnityEngine;
using Unity.Netcode;

public class UnitContext : NetworkBehaviour
{
    [Header("Unit Properties")]
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private float m_moveSpeed = 3f;
    [SerializeField] private float m_rotationSpeed = 5f;

    private Transform m_transform;
    private UnitController m_controller;
    private BossAbilityController m_bossAbilityController;

    public int MaxHealth => m_maxHealth;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        m_controller = GetComponent<UnitController>();
        m_bossAbilityController = GetComponent<BossAbilityController>();
    }

    public Transform Transform { get { return m_transform; } }
    public float MoveSpeed { get { return m_moveSpeed; } set { m_moveSpeed = value; } }
    public float RotationSpeed { get { return m_rotationSpeed; } set { m_rotationSpeed = value; } }
    public UnitController Controller { get { return m_controller;} }
    public BossAbilityController AbilityController 
    {
        get { return m_bossAbilityController; }
        private set { m_bossAbilityController = value; }
    }
}
