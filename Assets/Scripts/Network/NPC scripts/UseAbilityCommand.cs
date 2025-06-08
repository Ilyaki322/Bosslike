using UnityEngine;

public class UseAbilityCommand : ICommand
{
    int m_abilityIndex;
    BossAbilityController m_abController;
    Ability m_ability;

    public UseAbilityCommand(int ability, BossAbilityController abController)
    {
        m_abilityIndex = ability;
        m_abController = abController;
        m_ability = abController.GetAbility(ability);
    }

    public void Enter(UnitContext context)
    {
        Debug.Log("ENTER: use ability");
        m_abController.UseAbility(m_abilityIndex);
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        return m_ability.HasEnded;
    }

    public void Exit(UnitContext context)
    {

    }
}
