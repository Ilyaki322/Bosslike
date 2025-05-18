using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Base")]
public class AbilityDataSO : ScriptableObject
{
	[field: SerializeReference] public List<AbilityData> Abilities;

	[ContextMenu("Add TEST")]
	private void test() => Abilities.Add(new BoxHitData());
}
