using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    //private readonly Dictionary<StatusEffectSO, TimedEffect> m_effects = new();
    private readonly Dictionary<ulong, Dictionary<StatusEffectSO, TimedEffect>> m_userToDict = new();

    private void Update()
    {
        foreach (var dict in m_userToDict.Values.ToList())
        foreach (var effect in dict.Values.ToList())
        {
            effect.Tick(Time.deltaTime);
            if (effect.IsFinished) dict.Remove(effect.Buff);
        }
    }

    public void AddEffect(TimedEffect effect)
    {
        if (!m_userToDict.ContainsKey(effect.UserID))
        {
            m_userToDict[effect.UserID] = new();
        }

        var dict = m_userToDict[effect.UserID];
        if (dict.ContainsKey(effect.Buff))
        {
            dict[effect.Buff].Activate();
        }
        else
        {
            dict.Add(effect.Buff, effect);
            effect.Activate();
        }
    }
}
