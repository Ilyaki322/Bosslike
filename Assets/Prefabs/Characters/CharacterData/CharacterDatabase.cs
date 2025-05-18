using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterDatabase", fileName = "CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> charactersData;

    // Called when the SO is loaded (play mode or build)
    private void OnEnable()
    {
        PopulateIndexes();
    }

#if UNITY_EDITOR
    // Called in the editor whenever you modify the asset
    private void OnValidate()
    {
        PopulateIndexes();
    }
#endif

    // Ensure each CharacterData.Id matches its index in the list
    private void PopulateIndexes()
    {
        if (charactersData == null) return;
        for (int i = 0; i < charactersData.Count; i++)
        {
            charactersData[i].Id = i;
        }
    }
}
