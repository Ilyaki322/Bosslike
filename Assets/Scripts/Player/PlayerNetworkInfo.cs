using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Holds per-player data (e.g., player name) and broadcasts join/leave events.
/// </summary>
public class PlayerNetworkInfo : NetworkBehaviour
{
    public static event Action<PlayerNetworkInfo> OnPlayerJoined;
    public static event Action<ulong> OnPlayerLeft;

    public NetworkVariable<FixedString32Bytes> NameVar = new NetworkVariable<FixedString32Bytes>(
    default,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        OnPlayerJoined?.Invoke(this);
        NameVar.OnValueChanged += HandleNameChanged;
    }

    public override void OnNetworkDespawn()
    {
        NameVar.OnValueChanged -= HandleNameChanged;
        OnPlayerLeft?.Invoke(OwnerClientId);
    }

    public void InitPlayerInfo(PlayerSettings settings)
    {
        SetPlayerName(settings.m_PlayerName);
    }

    private void HandleNameChanged(FixedString32Bytes oldVal, FixedString32Bytes newVal)
    {
        // Optional: handle any logic when name updates
    }

    private void SetPlayerName(string newName)
    {
        if (!IsOwner)
            return;
        NameVar.Value = newName;
    }
}