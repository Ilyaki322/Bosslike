using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] RectTransform listContainer;
    [SerializeField] GameObject listItemPrefab;

    // track one entry per clientId
    private readonly Dictionary<ulong, PlayerListEntry> _entries
        = new Dictionary<ulong, PlayerListEntry>();

    private NetworkManager nm;

    private void Start()
    {
        nm = NetworkManager.Singleton;

        // 1) Add a row for every connection we know about (host + all clients)
        foreach (var clientId in nm.ConnectedClients.Keys)
            TryAddEntry(clientId);

        // 2) Listen for new joins & leaves
        nm.OnClientConnectedCallback += TryAddEntry;
        nm.OnClientDisconnectCallback += RemoveEntry;
    }

    private void OnDestroy()
    {
        nm.OnClientConnectedCallback -= TryAddEntry;
        nm.OnClientDisconnectCallback -= RemoveEntry;
    }

    private void TryAddEntry(ulong clientId)
    {
        if (_entries.ContainsKey(clientId)) return;

        if (!nm.ConnectedClients.TryGetValue(clientId, out var cd)) return;
        var po = cd.PlayerObject;
        if (po == null) return;

        var pi = po.GetComponent<PlayerInfo>();
        if (pi == null) return;

        // instantiate UI
        var go = Instantiate(listItemPrefab, listContainer);
        var entry = go.GetComponent<PlayerListEntry>();
        _entries[clientId] = entry;

        // mark host vs client
        bool isHost = clientId == NetworkManager.ServerClientId;
        // read whatever name has arrived so far
        var raw = pi.NameVar.Value.ToString();
        entry.Setup(string.IsNullOrEmpty(raw) ? "Guest" : raw, isHost);

        // subscribe to updates
        pi.NameVar.OnValueChanged += (_, newName) =>
            entry.UpdateName(newName.ToString());
    }

    private void RemoveEntry(ulong clientId)
    {
        if (_entries.TryGetValue(clientId, out var entry))
        {
            Destroy(entry.gameObject);
            _entries.Remove(clientId);
        }
    }
}
