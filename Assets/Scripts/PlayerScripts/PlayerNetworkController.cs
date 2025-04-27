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
    [SerializeField] private PlayerMovement_Network m_movement;
    [SerializeField] private PlayerInput m_input;
    [SerializeField] private Camera m_camera;

    private void Awake()
    {
        m_input.enabled = false;
        m_movement.enabled = false;
        m_camera.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_input.enabled = true;
            m_movement.enabled = true;
            m_camera.enabled = true;

            m_camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
