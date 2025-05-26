using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class AbilityBar : MonoBehaviour
{
    [SerializeField] private UIDocument m_document;
    [SerializeField] private StyleSheet m_styleSheet;

    [SerializeField] private Sprite m_frame;

    private List<AbilityBox> m_boxList = new();

    private VisualElement m_container;


    public void Generate(List<AbilityDataSO> abilityList)
    {
        var root = m_document.rootVisualElement;
        root.Clear();
        root.styleSheets.Add(m_styleSheet);

        m_container = root.CreateChild("container");
        var header = m_container.CreateChild("header");

        var grid = m_container.CreateChild("grid");
        int boxWidth = 60 + 2 * 4;
        int gridWidth = abilityList.Count * boxWidth;
        grid.style.width = gridWidth;

        var title = new Label("Abilities");
        header.Add(title);

        for (int i = 0; i < abilityList.Count; i++)
        {
            var t = new AbilityBox(abilityList[i].AbilityIcon, m_frame);
            m_boxList.Add(t);
            t.AddClass("box");
            grid.Add(t);
        }
    }
}
