using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;

[CustomEditor(typeof(AbilityDataSO))]
public class AbilityEditor : Editor
{
    private static List<Type> m_componentTypes;

    private AbilityDataSO m_data;


    private bool m_showForceUpdateButtons;
    private bool m_showAddComponentButtons;

    private void OnEnable()
    {
        m_data = target as AbilityDataSO;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        m_showAddComponentButtons = EditorGUILayout.Foldout(m_showAddComponentButtons, "Add Components");

        if (m_showAddComponentButtons)
        {
            foreach (var dataCompType in m_componentTypes)
            {
                if (GUILayout.Button(dataCompType.Name))
                {
                    var comp = Activator.CreateInstance(dataCompType) as AbilityData;

                    if (comp == null)
                        return;

                    //comp.InitializeAttackData(dataSO.NumberOfAttacks);

                    m_data.AddData(comp);

                    EditorUtility.SetDirty(m_data);
                }
            }
        }

        //showForceUpdateButtons = EditorGUILayout.Foldout(showForceUpdateButtons, "Force Update Buttons");

        //if (showForceUpdateButtons)
        //{
        //    if (GUILayout.Button("Force Update Component Names"))
        //    {
        //        foreach (var item in dataSO.ComponentData)
        //        {
        //            item.SetComponentName();
        //        }
        //    }

        //    if (GUILayout.Button("Force Update Attack Names"))
        //    {
        //        foreach (var item in dataSO.ComponentData)
        //        {
        //            item.SetAttackDataNames();
        //        }
        //    }
        //}
    }


    [DidReloadScripts]
    private static void OnRecompile()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(assembly => assembly.GetTypes());
        var filteredTypes = types.Where(
            type => type.IsSubclassOf(typeof(AbilityData)) && !type.ContainsGenericParameters && type.IsClass
        );
        m_componentTypes = filteredTypes.ToList();
    }
}
