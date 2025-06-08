using System;
using Unity.Netcode;
using UnityEngine;

public class Ability : NetworkBehaviour
{
    public event Action OnUse;

    private ulong m_user;

    private bool m_hasCD;
    private float m_cooldown;
    private float m_remaining;

    public bool HasEnded;

    public void Init(ulong user, float cd = 0)
    {
        m_user = user;
        if (cd != 0) m_hasCD = true;
        m_cooldown = cd;
        m_remaining = 0;
    }

    public void Use(AbilityBar ui, int i)
    {
        if (m_hasCD) {
            if (m_remaining > 0.1f) return;
            m_remaining = m_cooldown;
            ui.SetCooldown(i);
        }
        HasEnded = false;
        OnUse?.Invoke();   
    }

    public void Use()
    {
        if (m_hasCD)
        {
            if (m_remaining > 0.1f) return;
            m_remaining = m_cooldown;
        }
        HasEnded = false;
        OnUse?.Invoke();
    }

    public void Cooldown(float time)
    {
        if (m_remaining > 0) m_remaining -= time;
    }

    public bool IsUseable() => m_remaining < 0.1f;

    public ulong GetUser() => m_user;
}
