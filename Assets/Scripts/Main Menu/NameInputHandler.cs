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

    private string pendingName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Wire up buttons
        saveNameButton.onClick.AddListener(OnSaveNameClicked);
        changeNameButton.onClick.AddListener(ShowInputUI);

        // Initially, only show the input/save
        ShowInputUI();
    }

    private void OnSaveNameClicked()
    {
        // 1) Capture & validate
        var n = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(n)) n = "Guest";
        pendingName = n;

        // 2) Swap UI to greeting
        greetingText.text = $"Welcome, {n}!";
        greetingText.gameObject.SetActive(true);
        changeNameButton.gameObject.SetActive(true);

        nameInputField.gameObject.SetActive(false);
        saveNameButton.gameObject.SetActive(false);
        mainMenuContainerUI.SetActive(true);

        // 3) Wait for our local PlayerObject to spawn
        NetworkManager.Singleton.OnClientConnectedCallback += TransferNameToPlayerInfo;
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

    private void TransferNameToPlayerInfo(ulong clientId)
    {
        // Only run for _this_ client
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        // Grab our PlayerObject and set the network variable
        var po = NetworkManager.Singleton
                      .SpawnManager
                      .GetLocalPlayerObject();
        if (po != null && po.TryGetComponent<PlayerInfo>(out var pi))
        {
            pi.SetPlayerName(pendingName);
        }
        Debug.Log($"Name set to {pendingName} for client {clientId}");
        // Clean up
        NetworkManager.Singleton.OnClientConnectedCallback -= TransferNameToPlayerInfo;
        Destroy(gameObject);
    }
}
