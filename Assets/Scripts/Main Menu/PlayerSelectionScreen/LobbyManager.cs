using System;
using Unity.Netcode;
using Unity.Services.Qos.V2.Models;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event Action<ulong> PlayerJoined;
    public event Action<ulong> PlayerLeft;
    public event Action<ulong, int> PlayerPicked;

    public NetworkList<PlayerSelection> PlayerSelections { get; private set; }
        = new NetworkList<PlayerSelection>(
              default,
              NetworkVariableReadPermission.Everyone,
              NetworkVariableWritePermission.Server
          );

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        RegisterServerCallbacks();
        PlayerSelections.OnListChanged += HandleListChanged;
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback
                -= OnClientConnected;

        PlayerSelections.OnListChanged -= HandleListChanged;
    }

    private void RegisterServerCallbacks()
    {
        if (!IsServer) return;

        foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
            AddSelection(c.ClientId);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        AddSelection(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        RemoveSelection(clientId);
    }

    private void AddSelection(ulong clientId)
    {
        // don't add twice
        for (int i = 0; i < PlayerSelections.Count; i++)
            if (PlayerSelections[i].ClientId == clientId)
                return;

        var netObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        var info = netObj?.GetComponent<PlayerNetworkInfo>();
        if (info == null) return;

        // 1) add the empty entry
        PlayerSelections.Add(new PlayerSelection
        {
            ClientId = clientId,
            HasPicked = false,
            PickedCharacterId = -1
        });

        // 2) now watch for when the name actually arrives
        info.DisplayName.OnValueChanged += (oldName, newName) =>
        {
            // fire only once
            info.DisplayName.OnValueChanged -= null;
            Debug.Log($"[{clientId}] name arrived: {newName}");
            PlayerJoined?.Invoke(clientId);
        };
    }


    private void RemoveSelection(ulong clientId)
    {
        for (int i = 0; i < PlayerSelections.Count; i++)
            if (PlayerSelections[i].ClientId == clientId)
            {
                PlayerSelections.RemoveAt(i);
                return;
            }
    }

    private void HandleListChanged(NetworkListEvent<PlayerSelection> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<PlayerSelection>.EventType.Remove:
                PlayerLeft?.Invoke(evt.Value.ClientId);
                break;

            case NetworkListEvent<PlayerSelection>.EventType.Value:
                if (evt.Value.HasPicked)
                    PlayerPicked?.Invoke(
                        evt.Value.ClientId,
                        evt.Value.PickedCharacterId
                    );
                break;
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void RequestPickCharacterServerRpc(int characterId, ServerRpcParams rpc = default)
    {
        if (!IsServer) return;

        ulong sender = rpc.Receive.SenderClientId;
        for (int i = 0; i < PlayerSelections.Count; i++)
        {
            if (PlayerSelections[i].ClientId != sender) continue;
            var sel = PlayerSelections[i];
            sel.HasPicked = true;
            sel.PickedCharacterId = characterId;
            PlayerSelections[i] = sel;
            break;
        }
    }
}
