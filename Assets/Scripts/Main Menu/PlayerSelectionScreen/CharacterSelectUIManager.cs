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
        LobbyManager.Instance.PlayerReady += OnPlayerReady;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        ClearList();

        foreach (var sel in netList)
            SpawnRow(sel);

    }

    private void OnDisable()
    {
        var netList = LobbyManager.Instance.PlayerSelections;
        netList.OnListChanged -= OnSelectionChanged;
        LobbyManager.Instance.PlayerReady -= OnPlayerReady;
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
                HandleValueChange(evt);
                break;
        }
    }

    private void HandleValueChange(NetworkListEvent<PlayerSelection> evt)
    {
        if (_entries.TryGetValue(evt.Value.ClientId, out var entry))
        {
            entry.UpdateName(evt.Value.DisplayName.ToString());

            // update pick text if they picked
            if (evt.Value.PickedCharacterId >= 0)
            {
                var name = m_characterDatabase
                             .charactersData[evt.Value.PickedCharacterId]
                             .Name;
                entry.SetCharacterName(name);
            }
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
        InitCharacterName(sel, entry);
        entry.SetReady(sel.isReady);

        _entries[sel.ClientId] = entry;
    }

    private void OnPlayerReady(ulong clientId, bool isReady)
    {
        if (_entries.TryGetValue(clientId, out var entry))
            entry.SetReady(isReady);
    }

    private void InitCharacterName(PlayerSelection sel, PlayerListEntry entry)
    {
        if (sel.PickedCharacterId >= 0)
        {
            var name = m_characterDatabase
                           .charactersData[sel.PickedCharacterId]
                           .Name;
            entry.SetCharacterName(name);
        }
    }

    private void RemovePlayerEntry(ulong clientId)
    {
        if (_entries.TryGetValue(clientId, out var entry))
        {
            Destroy(entry.gameObject);
            _entries.Remove(clientId);
        }
    }

    private void ClearList()
    {
        foreach (Transform t in listContainer)
            Destroy(t.gameObject);
        _entries.Clear();
    }
}
