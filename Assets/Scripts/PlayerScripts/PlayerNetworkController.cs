using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This disables some scripts / components on the player,
 * and if the player is the owner will enable it.
 * This way each player controlls only his character.
 */
public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private UnitController m_movement;
    [SerializeField] private PlayerInput m_input;
    [SerializeField] private Camera m_camera;
    [SerializeField] private PlayerCombat m_combat;
    [SerializeField] private PlayerInputManager m_pim;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private AnimationController m_animController;
    [SerializeField] private UnitContext m_uc;
    [SerializeField] private AudioListener m_audioListener;

    private void Awake()
    {
        m_input.enabled = false;
        m_movement.enabled = false;
        m_camera.enabled = false;
        m_combat.enabled = false;
        m_pim.enabled = false;
        m_animController.enabled = false;
        m_uc.enabled = false;
        m_audioListener.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ClipPlayer.Instance().OffListener(); // TO DISABLE NO LISTNERS ON SCENE
        if (IsOwner)
        {
            m_input.enabled = true;
            m_movement.enabled = true;
            m_camera.enabled = true;
            m_combat.enabled = true;
            m_pim.enabled = true;
            m_animController.enabled = true;
            m_uc.enabled = true;
            m_audioListener.enabled = true;

            m_camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
