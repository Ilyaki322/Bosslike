using UnityEngine;

//[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Base")]
public abstract class AbilitySO : ScriptableObject
{
    protected PlayerCombat m_playerCombat;

    public void Init(PlayerCombat pc)
    {
        m_playerCombat = pc;
    }

    abstract public void Use();
}
