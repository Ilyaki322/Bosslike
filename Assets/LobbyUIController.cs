using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject))]
public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_lobby;
    [SerializeField] private Button m_disconnectButton;

    private void Awake()
    {
        m_disconnectButton.onClick.AddListener(OnDisconnectClicked);
    }

    private void OnEnable()
    {
        var net = LobbyManager.Instance;
        net.PlayerJoined += OnPlayerJoined;
        net.PlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        var net = LobbyManager.Instance;
        if (net != null)
        {
            net.PlayerJoined -= OnPlayerJoined;
            net.PlayerLeft -= OnPlayerLeft;
        }
    }

    private void OnDisconnectClicked()
    {
        ShowMainMenu();
        NetworkManager.Singleton.Shutdown();
    }

    private void OnPlayerJoined(ulong clientId)
    {
    }

    private void OnPlayerLeft(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
            ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        DespawnAllSceneObjects();
        m_lobby.SetActive(false);
        m_mainMenu.SetActive(true);
    }

    public static void DespawnAllSceneObjects()
    {
        var spawnMgr = NetworkManager.Singleton.SpawnManager;

        // Filter out only the true scene-objects
        var sceneObjs = spawnMgr.SpawnedObjects.Values
            .Where(no => no.IsSceneObject.GetValueOrDefault(false))
            .ToList();

        foreach (var no in sceneObjs)
            no.Despawn(destroy: false);
    }
}
