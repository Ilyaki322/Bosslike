using System;
using Unity.Netcode;
using UnityEngine;

public class Ability : NetworkBehaviour
{
    public event Action OnUse;

    public void Use(PlayerCombat pc, ulong user)
    {
        OnUse?.Invoke();
    }
}
