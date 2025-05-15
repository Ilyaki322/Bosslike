using System.Collections;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [Header("Countdown Text")]
    [SerializeField] private TMP_Text m_countdownText;

    [Header("Next Scene")]
    [SerializeField] private string m_nextSceneName = "GameScene";

    [Header("UI References")]
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_lobby;
    [SerializeField] private Button m_disconnectButton;
    [SerializeField] private Button m_readyButton;

    private string initCountdownText;

    private void OnEnable()
    {
        m_countdownText.gameObject.SetActive(false);
        initCountdownText = m_countdownText.text;

        m_disconnectButton.onClick.AddListener(OnDisconnectClicked);
        m_readyButton.onClick.AddListener(OnReadyClicked);
        m_readyButton.interactable = true;

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleLocalDisconnect;
        LobbyManager.Instance.PlayerPicked += OnPlayerPicked;
        LobbyManager.Instance.CountdownStarted += BeginCountdown;
    }

    private void OnDisable()
    {
        m_disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
        m_readyButton.onClick.RemoveListener(OnReadyClicked);

        LobbyManager.Instance.CountdownStarted -= BeginCountdown;
        LobbyManager.Instance.PlayerPicked -= OnPlayerPicked;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleLocalDisconnect;
    }

    private void BeginCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        m_countdownText.gameObject.SetActive(true);

        for (int i = 5; i >= 1; i--)
        {
            m_countdownText.text = initCountdownText + i.ToString() + "...";
            yield return new WaitForSeconds(1f);
        }
        m_countdownText.gameObject.SetActive(false);

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                m_nextSceneName,
                LoadSceneMode.Single
            );
        }
    }

    private void OnReadyClicked()
    {
        var list = LobbyManager.Instance.PlayerSelections;

        foreach(var sels in list)
        {
            if (sels.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                if (sels.PickedCharacterId < 0)
                {
                    Debug.LogWarning("Select a character before you can ready up!");
                    return;
                }
                break;
            }
        }

        LobbyManager.Instance.SetReadyServerRpc(true);
        m_readyButton.interactable = false;
    }

    private void OnPlayerPicked(ulong clientId, int newCharacterId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        m_readyButton.interactable = true;
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
