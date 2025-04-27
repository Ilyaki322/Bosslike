using System;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Healthbar_Network : NetworkBehaviour
{
    [SerializeField] float m_maxHP = 100f;
    [SerializeField] Slider m_hpbar;

    NetworkVariable<float> m_currHP = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_currHP.Value = m_maxHP;
        }

        m_hpbar.maxValue = m_maxHP;
        m_hpbar.value = m_currHP.Value;

        m_currHP.OnValueChanged += OnHealthChanged;
    }

    public void TakeDamage(float damage)
    {
        TakeDamageServerRpc(damage);  
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(float damage)
    {
        m_currHP.Value -= damage;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        m_hpbar.value = newValue;
    }
}
