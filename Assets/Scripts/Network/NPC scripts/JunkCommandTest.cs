using UnityEngine;

public class JunkCommandTest : ICommand
{
    public void Enter(UnitContext context)
    {
        Debug.Log("[JunkCommandTest] Entering command.");
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        Debug.Log($"Executing JunkCommandTest: DeltaTime = {deltaTime}");
        return true;
    }

    public void Exit(UnitContext context)
    {
        context.Controller.PushCommand(new CircleWalk(context.Transform.position, 3f, Mathf.PI / 4f), true);
    }
}
