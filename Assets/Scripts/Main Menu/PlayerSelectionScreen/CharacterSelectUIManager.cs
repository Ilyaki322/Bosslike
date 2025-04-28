using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform listContainer;
    [SerializeField] private GameObject listItemPrefab;

    private readonly Dictionary<ulong, PlayerListEntry> entries =
        new Dictionary<ulong, PlayerListEntry>();

    private void Update()
    {
        foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
        {
            var clientId = kvp.Key;
            if (entries.ContainsKey(clientId)) continue;

            var po = kvp.Value.PlayerObject;
            if (po == null) continue;

            var pi = po.GetComponent<PlayerInfo>();
            if (pi == null) continue;

            // Instantiate the UI entry
            var go = Instantiate(listItemPrefab, listContainer);
            var entry = go.GetComponent<PlayerListEntry>();
            entries[clientId] = entry;

            // Read and show initial name
            bool isHost = clientId == NetworkManager.ServerClientId;
            var fixedName = pi.NameVar.Value;
            var nameStr = fixedName.ToString();
            entry.Setup(string.IsNullOrEmpty(nameStr) ? "Guest" : nameStr, isHost);

            // Subscribe for any later changes
            pi.NameVar.OnValueChanged += (_, newName) =>
                entry.UpdateName(newName.ToString());
        }
    }
}