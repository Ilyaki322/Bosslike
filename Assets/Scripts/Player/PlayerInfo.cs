using Unity.Collections;
using Unity.Netcode;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> NameVar =
        new NetworkVariable<FixedString32Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string newName)
    {
        if (!IsOwner) return;
        SubmitNameServerRpc(newName);
    }

    [ServerRpc]
    private void SubmitNameServerRpc(string newName, ServerRpcParams rpcParams = default)
    {
        NameVar.Value = newName;
    }
}