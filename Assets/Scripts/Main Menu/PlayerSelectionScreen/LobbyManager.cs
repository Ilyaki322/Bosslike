using System;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event Action<ulong> PlayerJoined;
    public event Action<ulong> PlayerLeft;
    public event Action<ulong, int> PlayerPicked;
    public event Action<ulong, bool> PlayerReady;
    public event Action CountdownStarted;
    public event Action CountdownCanceled;

    private bool _countdownInProgress;

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
        PlayerSelections.OnListChanged += HandleListChanged;

        if (!IsServer) return;
        PlayerSelections.Clear();

        foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
            AddSelection(c.ClientId);

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
            isReady = false,
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

        ClearAllReady();
    }

    private void RemoveSelection(ulong clientId)
    {
        int idx = FindPlayerIndex(clientId);
        if (idx >= 0)
            PlayerSelections.RemoveAt(idx);

        ClearAllReady();
    }

    private void ClearAllReady()
    {

        if (!IsServer) return;

        for (int i = 0; i < PlayerSelections.Count; i++)
        {
            var sel = PlayerSelections[i];
            if (sel.isReady)
            {
                sel.isReady = false;
                PlayerSelections[i] = sel;
            }
        }

        if (_countdownInProgress)
        {
            _countdownInProgress = false;
            CancelCountdownClientRpc();
        }
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
                ClearAllReady();
                break;

            case NetworkListEvent<PlayerSelection>.EventType.RemoveAt:
                PlayerLeft?.Invoke(evt.Value.ClientId);
                ClearAllReady();
                break;

            case NetworkListEvent<PlayerSelection>.EventType.Value:
                if (evt.Value.PickedCharacterId >= 0)
                {
                    PlayerPicked?.Invoke(evt.Value.ClientId, evt.Value.PickedCharacterId);
                }

                PlayerReady?.Invoke(evt.Value.ClientId, evt.Value.isReady);

                if (IsServer)
                    TryStartGameCountdown();
                break;
        }
    }

    private void TryStartGameCountdown()
    {
        if (_countdownInProgress) return;

        if (PlayerSelections.Count <= 0) return;

        foreach (var sel in PlayerSelections)
        {
            if (!sel.isReady) return;
        }
        _countdownInProgress = true;

        StartGameCountdownClientRpc();
    }

    [ClientRpc]
    private void StartGameCountdownClientRpc()
    {
        CountdownStarted?.Invoke();
    }

    [ClientRpc]
    private void CancelCountdownClientRpc()
    {
        CountdownCanceled?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PickCharacterServerRpc(int characterId, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int selectionIdx = FindPlayerIndex(clientId);
        if (selectionIdx < 0) return;

        var sel = PlayerSelections[selectionIdx];
        sel.PickedCharacterId = characterId;
        sel.isReady = false;
        PlayerSelections[selectionIdx] = sel;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetReadyServerRpc(bool isReady, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int idx = FindPlayerIndex(clientId);
        if (idx < 0) return;

        var sel = PlayerSelections[idx];
        sel.isReady = isReady;
        PlayerSelections[idx] = sel;
    }
}
