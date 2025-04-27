using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

[Serializable]
struct PoolConfigObject
{
    public GameObject prefab;
    public int prewarmCount;
}

public class NetworkObjectPool : MonoBehaviour
{
    [SerializeField] NetworkManager m_networkManager;

    [SerializeField] List<PoolConfigObject> m_pooledPrefabList;

    HashSet<GameObject> m_prefabs = new HashSet<GameObject>();
    Dictionary<GameObject, Queue<NetworkObject>> m_pooledObjects = new Dictionary<GameObject, Queue<NetworkObject>>();

    private void Awake()
    {
        InitPool();
    }
    
    private void InitPool()
    {
        foreach (var configObject in m_pooledPrefabList)
        {
            RegisterPrefabInternal(configObject.prefab, configObject.prewarmCount);
        }
    }

    private void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
    {
        m_prefabs.Add(prefab);

        var prefabQueue = new Queue<NetworkObject>();
        m_pooledObjects[prefab] = prefabQueue;

        for (int i = 0; i < prewarmCount; i++)
        {
            var go = CreateInstance(prefab);
            ReturnNetworkObject(go.GetComponent<NetworkObject>(), prefab);
        }

        // Register Netcode Spawn handlers
        m_networkManager.PrefabHandler.AddHandler(prefab, new DummyPrefabInstanceHandler(prefab, this));
    }

    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        var go = networkObject.gameObject;

        go.SetActive(false);
        go.transform.SetParent(transform);
        m_pooledObjects[prefab].Enqueue(networkObject);
    }

    private void OnValidate()
    {
        for (var i = 0; i < m_pooledPrefabList.Count; i++)
        {
            var prefab = m_pooledPrefabList[i].prefab;
            if (prefab != null)
            {
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(),
                $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
            }
        }
    }

    public NetworkObject GetNetworkObject(GameObject prefab)
    {
        return GetNetworkObjectInternal(prefab, Vector3.zero, Quaternion.identity);
    }
    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GetNetworkObjectInternal(prefab, position, rotation);
    }
    private NetworkObject GetNetworkObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var queue = m_pooledObjects[prefab];

        NetworkObject networkObject;
        if (queue.Count > 0)
        {
            networkObject = queue.Dequeue();
        }
        else
        {
            networkObject = CreateInstance(prefab).GetComponent<NetworkObject>();
        }

        // Here we must reverse the logic in ReturnNetworkObject.
        var go = networkObject.gameObject;
        go.transform.SetParent(null);
        go.SetActive(true);

        go.transform.position = position;
        go.transform.rotation = rotation;

        return networkObject;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private GameObject CreateInstance(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    public void AddPrefab(GameObject prefab, int prewarmCount = 0)
    {
        var networkObject = prefab.GetComponent<NetworkObject>();

        Assert.IsNotNull(networkObject, $"{nameof(prefab)} must have {nameof(networkObject)} component.");
        Assert.IsFalse(m_prefabs.Contains(prefab), $"Prefab {prefab.name} is already registered in the pool.");

        RegisterPrefabInternal(prefab, prewarmCount);
    }
}

//==================================== Helper class ====================================

class DummyPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    GameObject m_Prefab;
    NetworkObjectPool m_Pool;

    public DummyPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
    {
        m_Prefab = prefab;
        m_Pool = pool;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return m_Pool.GetNetworkObject(m_Prefab, position, rotation);
    }

    public void Destroy(NetworkObject networkObject)
    {
        m_Pool.ReturnNetworkObject(networkObject, m_Prefab);
    }
}