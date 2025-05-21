using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField, HideInInspector] public NetworkObjectPool m_objectPool;

    [SerializeField] Animator m_animator;
    [SerializeField] NetworkAnimator m_networkAnimator;
    [SerializeField] List<AbilityDataSO> m_abilitiesData;
    [SerializeField] List<Ability> m_abilities;

    [HideInInspector] public Vector2 m_mousePosition;

    //public void Awake()
    //{
    //    if (!IsOwner) return;
    //    m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
    //    initAbilities();
    //}

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        initAbilities();
    }

    private void initAbilities()
    {
        foreach (var ability in m_abilitiesData)
        {
            foreach (var data in ability.AbilityDatas)
            {
                var abilityComponent = gameObject.AddComponent(data.getFunction()) as AbilityFunction;
                abilityComponent.Init(data);
            }
        }
    }

    public void TriggerAnimation(string animTrigger)
    {
        m_networkAnimator.SetTrigger(animTrigger);
    }

    public void UseAbility(int i, Vector3 mousePos)
    {
        m_mousePosition = new Vector2(mousePos.x, mousePos.y);

        if (m_abilities.Count <= i)
        {
            Debug.Log("Trying to use not existant ability");
            return;
        }

        //UseAbilityServerRpc(new Vector2(mousePos.x, mousePos.y), i, NetworkManager.Singleton.LocalClientId);
        m_abilities[i].Use(NetworkManager.Singleton.LocalClientId);
    }

    //[Rpc(SendTo.Server)]
    //private void UseAbilityServerRpc(Vector2 mousePos, int i, ulong user)
    //{
    //    print("HUI");
    //    m_mousePosition = mousePos;
    //    m_abilities[i].Use(this, user);
    //}

    // Manage Ability Cooldown
}
