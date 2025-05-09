using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_lobby;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;
        m_mainMenu.SetActive(false);
        m_lobby.SetActive(true);
        Debug.Log($"Server: Client connected {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;
        m_mainMenu.SetActive(true);
        m_lobby.SetActive(false);
        Debug.Log($"Server: Client disconnected {clientId}");
    }
}
