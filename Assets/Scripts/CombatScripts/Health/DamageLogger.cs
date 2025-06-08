using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageLogger : MonoBehaviour
{
    [SerializeField] int m_timeFrame = 30;
    float m_timer = 0;

    Dictionary<ulong, int> m_TotalDamageDict = new Dictionary<ulong, int>();
    Dictionary<ulong, int> m_DamageLastTimeFrame = new Dictionary<ulong, int>();

    public void RegisterDamage(ulong user, int damage)
    {
        if (!m_TotalDamageDict.ContainsKey(user))
        {
            m_TotalDamageDict[user] = damage;
            m_DamageLastTimeFrame[user] = damage;
        }
        else
        {
            m_TotalDamageDict[user] += damage;
            m_DamageLastTimeFrame[user] += damage;
        }
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > m_timeFrame)
        {
            m_timer = 0;
            m_DamageLastTimeFrame.Clear();
        }
    }

    // 322 means error
    public ulong GetHighestDamageUserAlltime()
    {
        if (m_TotalDamageDict.Count == 0) return 322;

        return m_TotalDamageDict
            .OrderByDescending(pair => pair.Value)
            .FirstOrDefault().Key;
    }

    // 322 means error
    public ulong GetHighestDamageUserLastFrame()
    {
        if (m_DamageLastTimeFrame.Count == 0) return 322;

        return m_DamageLastTimeFrame
            .OrderByDescending(pair => pair.Value)
            .FirstOrDefault().Key;
    }
}
