using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform listContainer;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private Button disconnectButton;

    [Header("Character Selection")]
    [SerializeField] private CharacterDatabase m_characterDatabase;

    // clientId → UI row
    private readonly Dictionary<ulong, PlayerListEntry> _entries = new();

    private void Awake()
    {
        disconnectButton.onClick.AddListener(() =>
            NetworkManager.Singleton.Shutdown());
    }

    private void OnEnable()
    {
        var lm = LobbyManager.Instance;
        lm.PlayerJoined += AddPlayerEntry;
        lm.PlayerLeft += RemovePlayerEntry;
        lm.PlayerPicked += UpdatePlayerEntry;

        foreach (var sel in lm.PlayerSelections)
            AddPlayerEntry(sel.ClientId);
    }

    private void OnDisable()
    {
        var lm = LobbyManager.Instance;
        lm.PlayerJoined -= AddPlayerEntry;
        lm.PlayerLeft -= RemovePlayerEntry;
        lm.PlayerPicked -= UpdatePlayerEntry;
        ClearList();
    }

    private void AddPlayerEntry(ulong clientId)
    {

        var netObj = NetworkManager.Singleton
                      .SpawnManager
                      .GetPlayerNetworkObject(clientId);
        if (netObj == null) return;

        var info = netObj.GetComponent<PlayerNetworkInfo>();
        if (info == null) return;

        string playerName = info.DisplayName.Value.ToString();

        if (_entries.ContainsKey(clientId)) return;

        var go = Instantiate(listItemPrefab, listContainer);
        var entry = go.GetComponent<PlayerListEntry>();
        entry.Setup(playerName, clientId == NetworkManager.ServerClientId);
        _entries[clientId] = entry;
    }

    private void RemovePlayerEntry(ulong clientId)
    {
        if (_entries.TryGetValue(clientId, out var entry))
        {
            Destroy(entry.gameObject);
            _entries.Remove(clientId);
        }
    }

    private void UpdatePlayerEntry(ulong clientId, int characterId)
    {
        if (!_entries.TryGetValue(clientId, out var entry))
            return;

        var data = m_characterDatabase.charactersData[characterId];
        entry.SetCharacterName(data.Name);
    }

    private void ClearList()
    {
        foreach (Transform t in listContainer)
            Destroy(t.gameObject);
        _entries.Clear();
    }
}
