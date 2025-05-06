using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiPlayerButton;

    [SerializeField] private string characterSelectionSceneName = "CharacterSelect";

    private void Awake()
    {
        singlePlayerButton.onClick.AddListener(OnHostClicked);
        multiPlayerButton.onClick.AddListener(OnClientClicked);
    }

    private void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(
            characterSelectionSceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    private void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
}
