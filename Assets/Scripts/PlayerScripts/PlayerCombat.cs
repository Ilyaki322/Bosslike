using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField, HideInInspector] public NetworkObjectPool m_objectPool;

    [SerializeField] Animator m_animator;
    [SerializeField] NetworkAnimator m_networkAnimator;
    [SerializeField] List<AbilitySO> m_abilities;

    [HideInInspector] public Vector2 m_mousePosition;

    public void Awake()
    {
        m_objectPool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
    }

    public void TriggerAnimation(string animTrigger)
    {
        //m_animator.SetTrigger(animTrigger);
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

        UseAbilityServerRpc(new Vector2(mousePos.x, mousePos.y), i, NetworkManager.Singleton.LocalClientId);
        //m_abilities[i].Use();
    }

    [Rpc(SendTo.Server)]
    private void UseAbilityServerRpc(Vector2 mousePos, int i, ulong user)
    {
        m_mousePosition = mousePos;
        m_abilities[i].Use(this, user);
    }

    // Manage Ability Cooldown
}
