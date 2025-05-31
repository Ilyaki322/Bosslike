using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitContext
{
    public Transform Transform { get; }
    public UnitController Controller { get; }
    public IPlayerLocator PlayerLocator { get;}
    public int moveSpeed = 3;
    public UnitContext(Transform transform, UnitController controller, IPlayerLocator playerLocator, int speed)
    {
        Transform = transform;
        Controller = controller;
        PlayerLocator = playerLocator;
        moveSpeed = speed;
    }
}
