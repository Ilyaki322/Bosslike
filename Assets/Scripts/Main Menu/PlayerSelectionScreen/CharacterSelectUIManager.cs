using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterSelectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform listContainer;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private CharacterDatabase m_characterDatabase;

    // clientId → UI row
    private readonly Dictionary<ulong, PlayerListEntry> _entries = new();

    private void OnEnable()
    {
        var netList = LobbyManager.Instance.PlayerSelections;
        netList.OnListChanged += OnSelectionChanged;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        ClearList();

        foreach (var sel in netList)
            SpawnRow(sel);

    }

    private void OnDisable()
    {
        var netList = LobbyManager.Instance.PlayerSelections;
        netList.OnListChanged -= OnSelectionChanged;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        ClearList();
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        RemovePlayerEntry(clientId);
    }

    private void OnSelectionChanged(NetworkListEvent<PlayerSelection> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<PlayerSelection>.EventType.Add:
                SpawnRow(evt.Value);
                break;

            case NetworkListEvent<PlayerSelection>.EventType.RemoveAt:
                RemovePlayerEntry(evt.Value.ClientId);
                break;

            case NetworkListEvent<PlayerSelection>.EventType.Value:
                if (_entries.TryGetValue(evt.Value.ClientId, out var entry))
                {
                    entry.UpdateName(evt.Value.DisplayName.ToString());

                    // update pick text if they picked
                    if (evt.Value.HasPicked)
                    {
                        var name = m_characterDatabase
                                     .charactersData[evt.Value.PickedCharacterId]
                                     .Name;
                        entry.SetCharacterName(name);
                    }
                }
                break;
        }
    }

    private void SpawnRow(PlayerSelection sel)
    {
        if (_entries.ContainsKey(sel.ClientId)) return;

        var go = Instantiate(listItemPrefab, listContainer);
        var entry = go.GetComponent<PlayerListEntry>();
        bool isHost = sel.ClientId == NetworkManager.ServerClientId;

        // name comes straight out of the shared list
        entry.Setup(sel.DisplayName.ToString(), isHost);

        _entries[sel.ClientId] = entry;
    }

    private void RemovePlayerEntry(ulong clientId)
    {
        Debug.Log($"[UI#{NetworkManager.Singleton.LocalClientId}] RemovePlayerEntry({clientId}) — keys = {string.Join(",", _entries.Keys)}");

        if (_entries.TryGetValue(clientId, out var entry))
        {
            Debug.Log($"[UI#{NetworkManager.Singleton.LocalClientId}]   ‣ Found it, destroying {entry.gameObject.name}");
            Destroy(entry.gameObject);
            _entries.Remove(clientId);
        }
        else
        {
            Debug.Log($"[UI#{NetworkManager.Singleton.LocalClientId}]   ‣ ❌ No entry found for {clientId}");
        }
    }

    private void ClearList()
    {
        foreach (Transform t in listContainer)
            Destroy(t.gameObject);
        _entries.Clear();
    }
}
