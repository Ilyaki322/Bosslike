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

    private PlayerInfo localPlayerInfo;
    private string pendingName;

    private void Awake()
    {
        // Wire up UI buttons immediately
        saveNameButton.onClick.AddListener(OnSaveNameClicked);
        changeNameButton.onClick.AddListener(ShowInputUI);
    }

    private void Start()
    {
        // Subscribe to the Netcode callback *after* the singleton is ready
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Try to cache PlayerInfo if it already exists
        TryAssignLocalPlayerInfo();

        // If they've already set a name, show the greeting
        if (IsNameAlreadySet())
        {
            var existing = localPlayerInfo.NameVar.Value.ToString();
            pendingName = existing;
            ShowGreetingUI(existing);
        }
        else
        {
            ShowInputUI();
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only care about our own local client
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        TryAssignLocalPlayerInfo();

        // If the player already clicked Save before PlayerObject existed, apply now
        if (!string.IsNullOrEmpty(pendingName) && localPlayerInfo != null)
            localPlayerInfo.SetPlayerName(pendingName);
    }

    private void OnSaveNameClicked()
    {
        var entered = nameInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(entered))
            return;

        pendingName = entered;

        // If PlayerInfo is ready, set immediately
        TryAssignLocalPlayerInfo();
        if (localPlayerInfo != null)
            localPlayerInfo.SetPlayerName(pendingName);

        ShowGreetingUI(entered);
    }

    private void TryAssignLocalPlayerInfo()
    {
        if (localPlayerInfo != null) return;
        var clientObj = NetworkManager.Singleton?.LocalClient?.PlayerObject;
        if (clientObj != null)
            localPlayerInfo = clientObj.GetComponent<PlayerInfo>();
    }

    private bool IsNameAlreadySet()
    {
        return localPlayerInfo != null
            && !string.IsNullOrEmpty(localPlayerInfo.NameVar.Value.ToString());
    }

    private void ShowInputUI()
    {
        nameInputField.gameObject.SetActive(true);
        saveNameButton.gameObject.SetActive(true);

        greetingText.gameObject.SetActive(false);
        changeNameButton.gameObject.SetActive(false);
        mainMenuContainerUI.SetActive(false);
    }

    private void ShowGreetingUI(string name)
    {
        greetingText.text = $"Welcome, {name}!";
        greetingText.gameObject.SetActive(true);
        changeNameButton.gameObject.SetActive(true);
        mainMenuContainerUI.SetActive(true);

        nameInputField.gameObject.SetActive(false);
        saveNameButton.gameObject.SetActive(false);
    }
}