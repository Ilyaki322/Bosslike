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
}
