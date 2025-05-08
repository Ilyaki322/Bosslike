using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class NameInputHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button saveNameButton;
    [SerializeField] private Button changeNameButton;
    [SerializeField] private TMP_Text greetingText;
    [SerializeField] private GameObject mainMenuContainerUI;

    private void Awake()
    {
        saveNameButton.onClick.AddListener(OnSaveNameClicked);
        changeNameButton.onClick.AddListener(ShowInputUI);
        ShowInputUI();
        DontDestroyOnLoad(gameObject);
    }

    private void OnSaveNameClicked()
    {
        // 1) Capture & validate
        var name = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(name)) name = "Guest";
        GetComponent<PlayerSettings>().m_PlayerName = name;

        // 2) Swap UI to greeting
        greetingText.text = $"Welcome, {name}!";
        greetingText.gameObject.SetActive(true);
        changeNameButton.gameObject.SetActive(true);

        nameInputField.gameObject.SetActive(false);
        saveNameButton.gameObject.SetActive(false);
        mainMenuContainerUI.SetActive(true);
    }

    private void ShowInputUI()
    {
        // Show only the input + save, hide the rest
        nameInputField.gameObject.SetActive(true);
        saveNameButton.gameObject.SetActive(true);

        greetingText.gameObject.SetActive(false);
        changeNameButton.gameObject.SetActive(false);
        mainMenuContainerUI.SetActive(false);
    }
}
