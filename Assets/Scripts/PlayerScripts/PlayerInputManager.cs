using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This class will contain all fields / functions related to user input
 * Classes that use user input will get the fields through this
 */
public class PlayerInputManager : MonoBehaviour
{
    [HideInInspector] public Vector2 moveUpdate;

    public void OnMove(InputValue context)
    {
        moveUpdate = context.Get<Vector2>();
    }
}
