using System.Linq;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    [Tooltip("Same CharacterDatabase SO assigned to your Player prefab")]
    public CharacterDatabase characterDatabase;

    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float delayBeforeGame = 2.5f;

    private void Start()
    {
        // 1) Every client (and host) spawns its own character
        if (NetworkManager.Singleton.IsClient)
            SpawnLocalCharacter();

        // 2) Only the server kicks off the final load
        if (NetworkManager.Singleton.IsServer)
            StartCoroutine(DeferredLoadGameScene());
    }

    private void SpawnLocalCharacter()
    {
        // who am I?
        var myId = NetworkManager.Singleton.LocalClientId;
        if(myId != NetworkManager.Singleton.LocalClientId) return;

        var sel = LobbyManager.Instance.GetSelection(myId);
        if (sel.PickedCharacterId < 0) return;

        // find my spawned PlayerObject (client‐side API!)
        var clientData = NetworkManager.Singleton.ConnectedClients[myId];
        var playerObj = clientData.PlayerObject;
        if (playerObj == null) return;                  // not spawned yet?

        // look up the prefab I should child under it
        var data = characterDatabase.charactersData[sel.PickedCharacterId];
        if (data.Prefab == null) return;

        // instantiate under my player object
        var go = Instantiate(data.Prefab, playerObj.transform);
        go.name = "CharacterModel";
    }

    private IEnumerator DeferredLoadGameScene()
    {
        // (optional) give clients a moment to finish
        yield return new WaitForSeconds(delayBeforeGame);

        // now load the real GameScene for everyone
        NetworkManager
          .Singleton
          .SceneManager
          .LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}
