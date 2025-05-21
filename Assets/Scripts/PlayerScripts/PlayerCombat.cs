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

    List<Ability> m_abilities = new();

    [HideInInspector] public Vector2 m_mousePosition;

    //public void Awake()
    //{
    //    if (!IsOwner) return;
    //    m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
    //    initAbilities();
    //}

    public override void OnNetworkSpawn()
    {
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        if (!IsOwner) return;
        initAbilities();
    }

    private void initAbilities()
    {
        for(int i = 0; i < m_abilitiesData.Count; i++)
        {
            var newObj = new GameObject($"Ability{i}");
            newObj.transform.parent = transform;
            m_abilities.Add(newObj.AddComponent<Ability>());
            
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
        GameObject prefab = m_objectPool.GetPrefabByIndex(projectileIndex);

        var go = m_objectPool.GetNetworkObject(prefab);
        go.transform.position = spawn;
        Vector2 direction = (target - (Vector2)spawn).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        var rb = go.GetComponent<Rigidbody2D>();
        Vector2 velocity = direction * 10f;

        go.GetComponent<NetworkObject>().Spawn(true);

        var tp = go.GetComponent<TestProjectile>();
        tp.Config(this, 3f, user);
        tp.SetVelocity(velocity);
    }

    public void UseAbility(int i, Vector3 mousePos)
    {
        m_mousePosition = (Vector2)mousePos;

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
    //    m_mousePosition = mousePos;
    //    m_abilities[i].Use(this, user);
    //}

    // Manage Ability Cooldown
}
