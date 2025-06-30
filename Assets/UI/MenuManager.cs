using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument m_document;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_playerUI;

    private Label m_title;
    private Button m_play;
    private Button m_exit;

    private void Awake()
    {
        var root = m_document.rootVisualElement;
        m_title = root.Q<Label>("Title");
        m_title.text = "Monster Uprising";
        m_play = root.Q<Button>("Play");
        m_exit = root.Q<Button>("Exit");
        m_exit.style.display = DisplayStyle.None;
        m_play.text = "Start";
        m_canvas.enabled = false;
        m_playerUI.SetActive(false);

        m_play.clicked += play;
    }

    private void play()
    {
        m_camera.enabled = false;
        m_canvas.enabled = true;
        var root = m_document.rootVisualElement;
        root.style.display = DisplayStyle.None;
        m_playerUI.SetActive(true);

        NetworkManager.Singleton.StartHost();
    }

    public void Gameover(string title, GameObject player)
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("Menu");
    }
}
