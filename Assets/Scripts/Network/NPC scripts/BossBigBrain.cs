using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BossBigBrain : MonoBehaviour, ICommand
{
    DamageLogger m_logger;
    BossAbilityController m_abilityManager;
    PlayerLocator m_playerLocator;

    Dictionary<ulong, Transform> m_playerDict;


    Transform m_mainTarget;

    public void Enter(UnitContext context)
    {
        print("ENTER: BossBigBrain");
        m_logger = GetComponent<DamageLogger>();
        m_abilityManager = GetComponent<BossAbilityController>();
        m_playerLocator = NetworkManager.Singleton.GetComponent<PlayerLocator>();
        m_playerDict = m_playerLocator.GetPlayersAndID();
    }

    public bool Execute(UnitContext context, float deltaTime)
    {
        if (m_mainTarget == null) aquireTarget();
        
        AbilityParameters bestAbility = aquireAbility();

        if (Vector3.Distance(transform.position, m_mainTarget.position) < bestAbility.range)
        {
            context.Controller.PushCommand(new StunCommand(2), true);
            context.Controller.PushCommand(new UseAbilityCommand(bestAbility.index, m_abilityManager) ,true);
            //context.Controller.PushCommand(new RotateToTargetCommand(m_mainTarget, context.RotationSpeed) ,true);
        }
        else
        {
            context.Controller.PushCommand(new MoveInRangeCommand(m_mainTarget, bestAbility.range), true);
        }

        return false;
    }

    public void Exit(UnitContext context)
    {
        throw new System.NotImplementedException();
    }

    private void aquireTarget()
    {
        ulong p = m_logger.GetHighestDamageUserAlltime();
        if (p == 322)
        {
            int index = Random.Range(0, m_playerDict.Count);
            m_mainTarget = m_playerDict.Values.ElementAt(index);
            return;
        }

        m_mainTarget = m_playerDict[p];
    }

    private AbilityParameters aquireAbility()
    {
        List<AbilityParameters> abilityList = m_abilityManager.GetAllAbilityParams();
        var usableAbilities = abilityList.Where(a => a.isUseable).OrderByDescending(a => a.prio);

        return usableAbilities.FirstOrDefault();
    }

    public Vector3 GetTarget() => m_mainTarget.position;
}
