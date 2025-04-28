using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    // Server‐owned write, everyone can read
    private NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    public NetworkVariable<FixedString32Bytes> NameVar => playerName;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string newName)
    {
        if (!IsOwner) return;
        playerName.Value = newName;
    }

    

}