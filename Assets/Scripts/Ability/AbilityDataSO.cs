using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Base")]
public class AbilityDataSO : ScriptableObject
{
	[field: SerializeReference] public List<AbilityData> AbilityDatas;

    private void OnEnable()
    {
        if (AbilityDatas == null) AbilityDatas = new List<AbilityData>();
    }

    public void AddData(AbilityData data)
	{
        if (AbilityDatas.FirstOrDefault(t => t.GetType() == data.GetType()) != null)
            return;

        AbilityDatas.Add(data);
    }

    public List<Type> GetFunctions()
    {
        List<Type> functions = new List<Type>();
        foreach (AbilityData data in AbilityDatas) 
        { 
            functions.Add(data.getFunction());
        }

        return functions;
    }
}
