using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Button selectBTN;
    [SerializeField] private GameObject selectionBG;

    int m_characterId;
    System.Action<int> CharOnClick;

    public void SetCharacterData(CharacterData data, System.Action<int> onClick)
    {
        m_characterId = data.Id;
        CharOnClick = onClick;

        characterImage.sprite = data.Icon;
        characterName.text = data.Name;

        selectBTN.onClick.AddListener(() => CharOnClick?.Invoke(m_characterId));
    }

    public void SetSelectable(bool selectable)
    {
        selectBTN.enabled = selectable;
        selectionBG.SetActive(!selectable);
    }
}
