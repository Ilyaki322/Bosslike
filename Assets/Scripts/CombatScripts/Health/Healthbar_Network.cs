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

    [SerializeField] GameObject m_dmgPopup;
    NetworkObjectPool m_objectPool;

    NetworkVariable<float> m_currHP = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_currHP.Value = m_maxHP;
            m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        }

        m_hpbar.maxValue = m_maxHP;
        m_hpbar.value = m_currHP.Value;

        m_currHP.OnValueChanged += OnHealthChanged;
    }

    //public void TakeDamage(float damage)
    //{
    //    TakeDamageServerRpc(damage);  
    //}

    public void TakeDamage(float damage, ulong user)
    {
        TakeDamageServerRpc(damage, user);
    }

    //[Rpc(SendTo.Server)]
    //public void TakeDamageServerRpc(float damage)
    //{
    //    m_currHP.Value -= damage;

    //    var popup = m_objectPool.GetNetworkObject(m_dmgPopup).gameObject;
    //    popup.GetComponent<NetworkObject>().Spawn(true);
    //    popup.GetComponent<DamagePopup>().Config(1f, transform.position, damage);
    //}
    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(float damage, ulong attackerID)
    {
        m_currHP.Value -= damage;

        var popup = m_objectPool.GetNetworkObject(m_dmgPopup).gameObject;
        popup.GetComponent<NetworkObject>().Spawn(true);
        popup.GetComponent<DamagePopup>().Config(0.75f, transform.position, damage, attackerID);
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        m_hpbar.value = newValue;
    }
}
