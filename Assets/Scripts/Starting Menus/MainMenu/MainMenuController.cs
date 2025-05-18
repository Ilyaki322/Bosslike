using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiPlayerButton;

    [Header("UI Panels")]
    [SerializeField] private GameObject lobbyPanel;

    private void Awake()
    {
        if (singlePlayerButton == null || multiPlayerButton == null)
        {
            Debug.LogError("MainMenuController: Assign all UI Button and Panel references in the Inspector.");
        }
    }

    private void OnEnable()
    {
        singlePlayerButton.onClick.AddListener(StartHost);
        multiPlayerButton.onClick.AddListener(StartClient);
    }

    private void OnDisable()
    {
        singlePlayerButton.onClick.RemoveListener(StartHost);
        multiPlayerButton.onClick.RemoveListener(StartClient);

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void StartHost()
    {
        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SwitchToLobby();
        }
    }

    private void SwitchToLobby()
    {
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }
}