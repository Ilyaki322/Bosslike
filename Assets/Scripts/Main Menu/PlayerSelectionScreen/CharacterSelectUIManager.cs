using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

/// <summary>
/// Listens to player join/leave events and maintains the UI list.
/// </summary>
public class CharacterSelectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform listContainer;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private Button m_disconnectButton;

    private readonly Dictionary<ulong, PlayerListEntry> _entries = new();

    private void Awake()
    {
        if (listContainer == null || listItemPrefab == null || m_disconnectButton == null)
            Debug.LogError("CharacterSelectUIManager: Assign listContainer and listItemPrefab in the Inspector.");

        m_disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
    }

    private void OnEnable()
    {
        PlayerNetworkInfo.OnPlayerJoined += HandlePlayerJoined;
        PlayerNetworkInfo.OnPlayerLeft += HandlePlayerLeft;

        foreach (var netObj in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            var info = netObj.GetComponent<PlayerNetworkInfo>();
            if (info != null)
                HandlePlayerJoined(info);
        }
    }

    private void OnDisable()
    {
        PlayerNetworkInfo.OnPlayerJoined -= HandlePlayerJoined;
        PlayerNetworkInfo.OnPlayerLeft -= HandlePlayerLeft;
    }

    private void HandlePlayerJoined(PlayerNetworkInfo info)
    {
        var clientId = info.OwnerClientId;
        if (_entries.ContainsKey(clientId))
            return;

        var go = Instantiate(listItemPrefab, listContainer);
        var entry = go.GetComponent<PlayerListEntry>();
        if (entry == null)
        {
            Debug.LogError("CharacterSelectUIManager: listItemPrefab missing PlayerListEntry script.");
            return;
        }

        _entries[clientId] = entry;
        bool isHost = clientId == NetworkManager.ServerClientId;
        entry.Setup(info.NameVar.Value.ToString(), isHost);

        info.NameVar.OnValueChanged += (oldName, newName) =>
            entry.UpdateName(newName.ToString());
    }

    private void HandlePlayerLeft(ulong clientId)
    {
        if (_entries.TryGetValue(clientId, out var entry))
        {
            Destroy(entry.gameObject);
            _entries.Remove(clientId);
        }
    }

    public void OnDisconnectButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
    }
}