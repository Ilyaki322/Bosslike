using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class PlayerNetworkInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> DisplayName =
    new NetworkVariable<FixedString64Bytes>(
      default,
      NetworkVariableReadPermission.Everyone,
      NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var name = PlayerSettings.LocalPlayerName;
            DisplayName.Value = name;

            Debug.Log($"Player {name} connected");
        }
    }
}