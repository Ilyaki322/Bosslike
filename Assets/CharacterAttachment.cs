using Unity.Netcode;
using UnityEngine;

public class CharacterAttachment : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        var playerNetObj = NetworkManager.Singleton
                              .SpawnManager
                              .GetPlayerNetworkObject(OwnerClientId);

        if (playerNetObj != null)
        {
            transform.SetParent(playerNetObj.transform, worldPositionStays: true);
        }
    }
}
