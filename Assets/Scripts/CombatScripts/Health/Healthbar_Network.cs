using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class Healthbar_Network : NetworkBehaviour
{
    [SerializeField] private Slider m_hpbar;
    [SerializeField] private GameObject m_dmgPopup;
    [SerializeField] private bool m_showDmg = false;

    private NetworkObjectPool m_objectPool;
    private DamageLogger m_log;
    private UnitContext m_ctx;

    // Track only current HP here
    public NetworkVariable<float> CurrHP { get; } =
        new NetworkVariable<float>(0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public int MaxHealth => m_ctx.MaxHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_ctx = GetComponent<UnitContext>();
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool")
                                  .GetComponent<NetworkObjectPool>();
        TryGetComponent(out m_log);

        if (IsServer)
            CurrHP.Value = m_ctx.MaxHealth;

        // setup slider on all peers
        if (m_hpbar != null)
        {
            m_hpbar.maxValue = m_ctx.MaxHealth;
            m_hpbar.value = CurrHP.Value;
        }

        // subscribe to HP changes
        CurrHP.OnValueChanged += OnHealthChanged;
    }

    public void TakeDamage(float damage, ulong attackerID)
    {
        TakeDamageServerRpc(damage, attackerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong attackerID)
    {
        CurrHP.Value = Mathf.Clamp(CurrHP.Value - damage, 0f, m_ctx.MaxHealth);
        if (m_log) m_log.RegisterDamage(attackerID, Mathf.RoundToInt(damage));

        if (m_showDmg)
        {
            var popup = m_objectPool.GetNetworkObject(m_dmgPopup).gameObject;
            popup.GetComponent<NetworkObject>().Spawn(true);
            popup.GetComponent<DamagePopup>()
                 .Config(0.75f, transform.position, damage, attackerID);
        }
    }

    private void OnHealthChanged(float oldVal, float newVal)
    {
        if (m_hpbar != null)
            m_hpbar.value = newVal;
    }
}