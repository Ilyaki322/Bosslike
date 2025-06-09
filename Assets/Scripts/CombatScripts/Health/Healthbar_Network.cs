using System;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Healthbar_Network : NetworkBehaviour
{
    [SerializeField] Slider m_hpbar;

    [SerializeField] GameObject m_dmgPopup;
    [SerializeField] bool m_showDmg = false;
    NetworkObjectPool m_objectPool;

    private UnitContext m_ctx;

    DamageLogger m_log;

    NetworkVariable<float> m_currHP = new NetworkVariable<float>();
    public NetworkVariable<float> CurrHP => m_currHP;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            m_currHP.Value = GetComponent<UnitContext>().MaxHealth;
            m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
            TryGetComponent<DamageLogger>(out m_log);
        }

        m_hpbar.maxValue = m_currHP.Value;
        m_hpbar.value = m_currHP.Value;

        m_currHP.OnValueChanged += OnHealthChanged;
    }

    public void TakeDamage(float damage, ulong user)
    {
        TakeDamageServerRpc(damage, user);
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(float damage, ulong attackerID)
    {
        m_currHP.Value -= damage;
        if (m_log) m_log.RegisterDamage(attackerID, Mathf.RoundToInt(damage));

        var popup = m_objectPool.GetNetworkObject(m_dmgPopup).gameObject;
        popup.GetComponent<NetworkObject>().Spawn(true);
        popup.GetComponent<DamagePopup>().Config(0.75f, transform.position, damage, attackerID);
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        m_hpbar.value = Mathf.Clamp(0,newValue, m_ctx.MaxHealth);
    }
}
