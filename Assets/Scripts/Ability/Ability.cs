using System;
using Unity.Netcode;
using UnityEngine;

public class Ability : NetworkBehaviour
{
    public event Action OnUse;
    private ulong m_user;

    public void Use(ulong user)
    {
        m_user = user;
        OnUse?.Invoke();
    }

    public ulong GetUser() => m_user;
}
