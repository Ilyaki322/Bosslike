using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : NetworkBehaviour
{
    [Tooltip("Same CharacterDatabase SO assigned to your Player prefab")]
    public CharacterDatabase characterDatabase;

    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float delayBeforeGame = 2.5f;

    private void Start()
    {
        if (IsClient)
            SpawnLocalCharacter();

        if (IsServer)
            StartCoroutine(ServerLoadGameSceneAfterDelay());
    }

    private void SpawnLocalCharacter()
    {
        var myId = NetworkManager.Singleton.LocalClientId;
        Debug.Log($"SpawnLocalCharacter for client {myId}");

        var sel = LobbyManager.Instance.GetSelection(myId);
        if (sel.PickedCharacterId < 0)
        {
            Debug.LogWarning($"Client {myId} has no selected character yet.");
            return;
        }

        // Issue ServerRpc; let server do the spawn
        SpawnCharacterOnServerRpc(sel.PickedCharacterId);
    }

    private IEnumerator ServerLoadGameSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGame);

        NetworkManager.Singleton.SceneManager.LoadScene(
            gameSceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCharacterOnServerRpc(int characterId, ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;

        var data = characterDatabase.charactersData[characterId];
        if (data.Prefab == null)
        {
            Debug.LogError($"CharacterDatabase entry {characterId} has no prefab!");
            return;
        }

        var instance = Instantiate(data.Prefab);
        var netObj = instance.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(clientId);
    }
}
