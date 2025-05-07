using UnityEngine;

//[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Base")]
public abstract class AbilitySO : ScriptableObject
{
    abstract public void Use(PlayerCombat pc, ulong user);
}
