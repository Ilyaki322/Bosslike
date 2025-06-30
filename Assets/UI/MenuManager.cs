using System.Collections;
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

    private VisualElement m_fadeOverlay;
    private Label m_gameOverLabel;

    private void Awake()
    {
        var root = m_document.rootVisualElement;
        m_title = root.Q<Label>("Title");
        m_title.text = "Monster Uprising";
        m_play = root.Q<Button>("Play");
        m_exit = root.Q<Button>("Exit");
        m_play.text = "Start";
        m_canvas.enabled = false;
        m_playerUI.SetActive(false);

        m_play.clicked += play;
        m_exit.clicked += exit;

        m_fadeOverlay = root.Q<VisualElement>("Container");
        m_gameOverLabel = root.Q<Label>("GameOverLabel");

        m_gameOverLabel.style.display = DisplayStyle.None;
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
        //NetworkManager.Singleton.Shutdown();
        //Destroy(NetworkManager.Singleton.gameObject);
        //SceneManager.LoadScene("BossTest");
        StartCoroutine(GameoverSequence());
    }

    private void exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator GameoverSequence()
    {
        var root = m_document.rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        m_play.style.display = DisplayStyle.None;
        m_exit.style.display = DisplayStyle.None;
        m_title.style.display = DisplayStyle.None;
        m_fadeOverlay.style.display = DisplayStyle.Flex;
        m_gameOverLabel.style.display = DisplayStyle.Flex;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = elapsed / duration;
            m_fadeOverlay.style.backgroundColor = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        m_fadeOverlay.style.backgroundColor = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(2f);

        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("BossTest");
    }
}
