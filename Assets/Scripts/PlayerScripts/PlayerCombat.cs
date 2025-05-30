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

    List<Ability> m_abilities = new();
    AbilityBar m_abilityBarUI;

    [HideInInspector] public Vector2 m_mousePosition;

    private void Update()
    {
        if (!IsOwner) return;
        progressCooldown(Time.deltaTime);
        m_abilityBarUI.ProgressCooldown(Time.deltaTime);
    }

    private void progressCooldown(float time)
    {
        foreach (var ability in m_abilities)
        {
            ability.Cooldown(time);
        }
    }

    public override void OnNetworkSpawn()
    {
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        if (!IsOwner) return;
        initAbilities();
        initUI();
    }

    private void initUI()
    {
        m_abilityBarUI = GetComponentInChildren<AbilityBar>();
        m_abilityBarUI.Generate(m_abilitiesData);
    }

    private void initAbilities()
    {
        for(int i = 0; i < m_abilitiesData.Count; i++)
        {
            var newObj = new GameObject($"Ability{i}");
            newObj.transform.parent = transform;

            var ability = newObj.AddComponent<Ability>();
            ability.Init(NetworkManager.Singleton.LocalClientId, m_abilitiesData[i].AbilityCooldown);
            m_abilities.Add(ability);
            
            foreach (var data in m_abilitiesData[i].AbilityDatas)
            {
                var abilityComponent = newObj.AddComponent(data.getFunction()) as AbilityFunction;
                abilityComponent.Init(data);
            }
        }
    }

    public void TriggerAnimation(string animTrigger)
    {
        m_networkAnimator.SetTrigger(animTrigger);
    }

    public void RequestProjectile(GameObject projectile)
    {
        int index = m_objectPool.GetPrefabIndex(projectile);
        if (index == -1)
        {
            Debug.LogError("Projectile not registered in pool.");
            return;
        }
        UseProjectileRpc(transform.position, m_mousePosition, index, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    public void UseProjectileRpc(Vector3 spawn, Vector2 target, int projectileIndex, ulong user)
    {
        var playerObject = NetworkManager.ConnectedClients[user].PlayerObject;
        if (playerObject == null) return;
        Vector3 sspawn = playerObject.transform.position;

        GameObject prefab = m_objectPool.GetPrefabByIndex(projectileIndex);
        var go = m_objectPool.GetNetworkObject(prefab, sspawn, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn(true);

        var tp = go.GetComponent<Projectile>();
        tp.Config(user, sspawn, target);
    }

    public void UseAbility(int i, Vector3 mousePos)
    {
        m_mousePosition = (Vector2)mousePos;

        if (m_abilities.Count <= i)
        {
            Debug.Log("Trying to use not existant ability");
            return;
        }

        m_abilities[i].Use(m_abilityBarUI, i);
    }
}
