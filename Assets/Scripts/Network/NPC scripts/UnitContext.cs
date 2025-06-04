using UnityEngine;
using Unity.Netcode;

public class UnitContext : NetworkBehaviour
{
    [Header("Unit Properties")]
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private float m_moveSpeed = 3f;

    private Transform m_transform;
    private UnitController m_controller;
    private IPlayerLocator m_playerLocator;
    public readonly NetworkVariable<int> m_currHealth = new NetworkVariable<int>();

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        m_controller = GetComponent<UnitController>();
        m_playerLocator = null;
    }

    public override void OnNetworkSpawn()
    {
        m_currHealth.Value = m_maxHealth;
    }

    public int Health { get { return m_currHealth.Value; } set { setHealthServerRpc(value); } }
    public Transform Transform { get { return m_transform; } }
    public float MoveSpeed { get { return m_moveSpeed; } set { m_moveSpeed = value; } }
    public UnitController Controller { get { return m_controller;} }

    //Used by AI to locate all players
    public IPlayerLocator PlayerLocator { set { m_playerLocator = value; } get { return m_playerLocator; } }

    [ServerRpc]
    private void setHealthServerRpc(int health)
    {
       m_currHealth.Value = Mathf.Clamp((m_currHealth.Value + health), 0, m_maxHealth);
    }
}
