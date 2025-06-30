using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager2 : MonoBehaviour
{
    [SerializeField] private UIDocument m_document;

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

        m_play.clicked += play;
    }

    private void play()
    {
        SceneManager.LoadScene("BossTest");
    }
}
