using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private readonly Dictionary<StatusEffectSO, TimedEffect> m_effects = new();

    private void Update()
    {
        foreach (var effect in m_effects.Values.ToList())
        {
            effect.Tick(Time.deltaTime);
            if (effect.IsFinished) m_effects.Remove(effect.Buff);
        }
    }

    public void AddEffect(TimedEffect effect)
    {
        if (m_effects.ContainsKey(effect.Buff))
        {
            m_effects[effect.Buff].Activate();
        }
        else
        {
            m_effects.Add(effect.Buff, effect);
            effect.Activate();
        }
    }
}
