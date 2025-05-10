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
    }

    private void StartHost()
    {
        SwitchToLobby();
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        SwitchToLobby();
        NetworkManager.Singleton.StartClient();
    }

    private void SwitchToLobby()
    {
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }
}