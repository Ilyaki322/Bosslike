using Unity.Netcode;
using UnityEngine;

public abstract class Ability : NetworkBehaviour
{
    protected PlayerCombat m_playerCombat;

    public void Init(PlayerCombat pc)
    {
        m_playerCombat = pc;
    }

    abstract public void Use();
}
