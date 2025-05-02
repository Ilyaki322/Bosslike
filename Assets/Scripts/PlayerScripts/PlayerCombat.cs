using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField, HideInInspector] public NetworkObjectPool m_objectPool;
    [SerializeField] List<Ability> m_abilities;

    [HideInInspector] public Vector2 m_mousePosition;

    public void Awake()
    {
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();

        foreach (var ab in m_abilities)
        {
            ab.Init(this);
        }
    }

    public void UseAbility(int i, Vector3 mousePos)
    {
        m_mousePosition = new Vector2(mousePos.x, mousePos.y);

        if (m_abilities.Count <= i)
        {
            Debug.Log("Trying to use not existant ability");
            return;
        }

        UseAbilityServerRpc(new Vector2(mousePos.x, mousePos.y), i);
        //m_abilities[i].Use();
    }

    [Rpc(SendTo.Server)]
    private void UseAbilityServerRpc(Vector2 mousePos, int i)
    {
        m_mousePosition = mousePos;
        m_abilities[i].Use();
    }
    
    // Manager Ability Cooldown
}
