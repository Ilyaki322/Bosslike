
public interface ICommand
{
    void Enter(UnitContext context);

    bool Execute(UnitContext context, float deltaTime);

    void Exit(UnitContext context);

}
