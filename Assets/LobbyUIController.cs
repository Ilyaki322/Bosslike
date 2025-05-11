using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_lobby;
    [SerializeField] private Button m_disconnectButton;

    private void OnEnable()
    {
        m_disconnectButton.onClick.AddListener(OnDisconnectClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleLocalDisconnect;
    }

    private void OnDisable()
    {
        m_disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleLocalDisconnect;
    }

    private void HandleLocalDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
            ShowMainMenu();
    }

    private void OnDisconnectClicked()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void ShowMainMenu()
    {
        m_lobby.SetActive(false);
        m_mainMenu.SetActive(true);
    }
}
