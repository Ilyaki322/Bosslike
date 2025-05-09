using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiPlayerButton;

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
        PlayerNetworkInfo.OnPlayerJoined += HandleLocalPlayerJoined;
    }

    private void OnDisable()
    {
        singlePlayerButton.onClick.RemoveListener(StartHost);
        multiPlayerButton.onClick.RemoveListener(StartClient);
        PlayerNetworkInfo.OnPlayerJoined -= HandleLocalPlayerJoined;
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void HandleLocalPlayerJoined(PlayerNetworkInfo info)
    {
        if (!info.IsOwner) return;
        var settings = GetComponent<PlayerSettings>();
        if (settings != null)
            info.InitPlayerInfo(settings);
    }
}