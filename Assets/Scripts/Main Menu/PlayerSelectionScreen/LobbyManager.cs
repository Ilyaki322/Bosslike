using System;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event Action<ulong> PlayerJoined;
    public event Action<ulong> PlayerLeft;
    public event Action<ulong, int> PlayerPicked;

    public NetworkList<PlayerSelection> PlayerSelections
      = new NetworkList<PlayerSelection>(
          default,
          NetworkVariableReadPermission.Everyone,
          NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        // Listen *before* seeding so host (ID 0) fires too
        PlayerSelections.OnListChanged += HandleListChanged;

        if (!IsServer) return;
        PlayerSelections.Clear();
        // Seed existing (including host)
        foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
            AddSelection(c.ClientId);

        // Future joins/leaves
        NetworkManager.Singleton.OnClientConnectedCallback += AddSelection;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemoveSelection;
    }

    public override void OnDestroy()
    {
        PlayerSelections.OnListChanged -= HandleListChanged;

        if (NetworkManager.Singleton != null && IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= AddSelection;
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemoveSelection;
        }
    }

    private void AddSelection(ulong clientId)
    {
        if (FindPlayerIndex(clientId) >= 0) return;

        var netObj = NetworkManager.Singleton.SpawnManager
                             .GetPlayerNetworkObject(clientId);
        var info = netObj.GetComponent<PlayerNetworkInfo>();
        var sel = new PlayerSelection
        {
            ClientId = clientId,
            DisplayName = info.DisplayName.Value,
            HasPicked = false,
            PickedCharacterId = -1
        };

        PlayerSelections.Add(sel);

        info.DisplayName.OnValueChanged += (oldName, newName) =>
        {
            var idx = FindPlayerIndex(clientId);
            if (idx < 0) return;
            var updated = PlayerSelections[idx];
            updated.DisplayName = newName;
            PlayerSelections[idx] = updated;
        };
    }

    private void RemoveSelection(ulong clientId)
    {
        int idx = FindPlayerIndex(clientId);
        if (idx >= 0)
            PlayerSelections.RemoveAt(idx);
    }

    int FindPlayerIndex(ulong clientId)
    {
        for (int i = 0; i < PlayerSelections.Count; i++)
            if (PlayerSelections[i].ClientId == clientId) return i;
        return -1;
    }

    private void HandleListChanged(NetworkListEvent<PlayerSelection> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<PlayerSelection>.EventType.Add:
                PlayerJoined?.Invoke(evt.Value.ClientId);
                break;

            case NetworkListEvent<PlayerSelection>.EventType.RemoveAt:
                Debug.Log($"[LobbyManager] 👉 firing PlayerLeft for {evt.Value.ClientId}");
                PlayerLeft?.Invoke(evt.Value.ClientId);
                break;

            case NetworkListEvent<PlayerSelection>.EventType.Value:
                if (evt.Value.PickedCharacterId >= 0)
                    PlayerPicked?.Invoke(evt.Value.ClientId, evt.Value.PickedCharacterId);
                break;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void PickCharacterServerRpc(int characterId, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int selectionIdx = FindPlayerIndex(clientId);
        if (selectionIdx < 0) return;

        var sel = PlayerSelections[selectionIdx];
        sel.PickedCharacterId = characterId;
        PlayerSelections[selectionIdx] = sel;
    }
}
