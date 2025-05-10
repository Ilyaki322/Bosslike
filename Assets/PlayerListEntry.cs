using TMPro;
using UnityEngine;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text selectedCharacterText;
    [SerializeField] private GameObject hostBadge;

    public void Setup(string playerName, bool isHost)
    {
        nameText.text = playerName;
        hostBadge.SetActive(isHost);
    }

    public void UpdateName(string newName)
    {
        nameText.text = newName;
    }

    public void SetCharacterName(string charName)
    {
        selectedCharacterText.text = charName;
    }
}
