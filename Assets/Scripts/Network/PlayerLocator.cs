using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerLocator : MonoBehaviour, IPlayerLocator
{
    public IReadOnlyList<Transform> GetPlayers()
    {
        // Get all players in the scene
        var players = NetworkManager.Singleton.ConnectedClientsList
            .Select(client => client.PlayerObject)
            .Where(playerObject => playerObject != null)
            .Select(playerObject => playerObject.transform)
            .ToList();

        return players;
    }

    public Transform GetPlayerByID(ulong id)
    {
        return NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).transform;
    }

    public Dictionary<ulong, Transform> GetPlayersAndID()
    {
        Dictionary<ulong, Transform> d = new();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            d[client.ClientId] = client.PlayerObject.transform;
        }

        return d;
    }
}
