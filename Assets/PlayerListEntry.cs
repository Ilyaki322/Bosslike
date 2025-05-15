using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text selectedCharacterText;
    [SerializeField] private GameObject hostBadge;
    [SerializeField] private GameObject readyIcon;

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

    public void SetReady(bool isReady)
    {
        readyIcon.SetActive(isReady);
    }
}
