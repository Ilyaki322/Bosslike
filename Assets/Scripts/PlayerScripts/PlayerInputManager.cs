using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This class will contain all fields / functions related to user input
 * Classes that use user input will get the fields through this
 */
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] PlayerCombat m_playerCombat;
    [SerializeField] Camera m_playerCam;

    [HideInInspector] public Vector2 moveUpdate;

    public void OnMove(InputValue context)
    {
        moveUpdate = context.Get<Vector2>();
    }

    public void OnBasicAttack()
    {
        m_playerCombat.UseAbility(0, m_playerCam.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnSecondaryAttack()
    {
        m_playerCombat.UseAbility(1, m_playerCam.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnThirdAttack()
    {
        m_playerCombat.UseAbility(2, m_playerCam.ScreenToWorldPoint(Input.mousePosition));
    }
}
