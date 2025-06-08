using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct AbilityParameters
{
    public int index;
    public bool isUseable;
    public float prio;
    public float range;
};

public class BossAbilityController : NetworkBehaviour
{
    NetworkObjectPool m_objectPool;

    [SerializeField] List<AbilityDataSO> m_abilitiesData;
    List<Ability> m_abilities = new();

    private void Update()
    {
        if (!IsOwner) return;
        progressCooldown(Time.deltaTime);
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
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
        initAbilities();
    }

    // TODO: add all components --> only then init all.
    private void initAbilities()
    {
        for (int i = 0; i < m_abilitiesData.Count; i++)
        {
            var newObj = new GameObject($"Ability{i}");
            newObj.transform.parent = transform;
            newObj.transform.localPosition = Vector3.zero;

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

    public List<AbilityParameters> GetAllAbilityParams() 
    {
        List<AbilityParameters> parameters = new();

        for (int i = 0; i < m_abilitiesData.Count; i++)
        {
            if (m_abilitiesData[i].IsUseableByBoss)
            {
                AbilityParameters p = new AbilityParameters();
                p.index = i;
                p.prio = m_abilitiesData[i].AbilityPriority;
                p.range = m_abilitiesData[i].AbilityRange;
                p.isUseable = m_abilities[i].IsUseable();
                parameters.Add(p);
            }
        }

        return parameters;
    }

    //public void RequestProjectile(GameObject projectile)
    //{
    //    int index = m_objectPool.GetPrefabIndex(projectile);
    //    if (index == -1)
    //    {
    //        Debug.LogError("Projectile not registered in pool.");
    //        return;
    //    }
    //    //UseProjectileRpc(transform.position, m_mousePosition, index, NetworkManager.Singleton.LocalClientId);
    //}

    //[Rpc(SendTo.Server)]
    //public void UseProjectileRpc(Vector3 spawn, Vector2 target, int projectileIndex, ulong user)
    //{
    //    var playerObject = NetworkManager.ConnectedClients[user].PlayerObject;
    //    if (playerObject == null) return;
    //    Vector3 sspawn = playerObject.transform.position;

    //    GameObject prefab = m_objectPool.GetPrefabByIndex(projectileIndex);
    //    var go = m_objectPool.GetNetworkObject(prefab, sspawn, Quaternion.identity);
    //    go.GetComponent<NetworkObject>().Spawn(true);

    //    var tp = go.GetComponent<Projectile>();
    //    tp.Config(user, sspawn, target);
    //}

    public void UseAbility(int i)
    {
        if (m_abilities.Count <= i)
        {
            Debug.Log("Trying to use not existant ability");
            return;
        }

        m_abilities[i].Use();
    }

    public Ability GetAbility(int i) => m_abilities[i];
}
