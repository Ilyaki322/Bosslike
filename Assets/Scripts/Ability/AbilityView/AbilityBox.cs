using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityBox : VisualElement
{
    private VisualElement m_cooldownOverlay;
    private float m_totalCooldown;
    private float m_progress;

    public AbilityBox(Sprite abilitySprite, Sprite frameSprite, float cooldown)
    {
        var icon = this.CreateChild<Image>("tileIcon");
        icon.image = abilitySprite.texture;

        var frame = this.CreateChild("tileFrame");
        frame.style.backgroundImage = frameSprite.texture;

        if (cooldown > 0)
        {
            m_progress = 0;
            m_totalCooldown = cooldown;

            m_cooldownOverlay = new VisualElement();
            m_cooldownOverlay.AddClass("cooldown-overlay");
            this.Add(m_cooldownOverlay);
        }

    }

    public void setCooldown()
    {
        m_progress = m_totalCooldown;

        if (m_cooldownOverlay != null)
        {
            m_cooldownOverlay.style.height = new Length(60, LengthUnit.Pixel);
            m_cooldownOverlay.style.top = new Length(0, LengthUnit.Pixel);
            m_cooldownOverlay.style.display = DisplayStyle.Flex;
        }
    }

    public void Cooldown(float time)
    {
        if (m_cooldownOverlay != null)
        {
            m_progress -= time;

            float percent = Mathf.Clamp01(m_progress / m_totalCooldown);
            float remainingHeight = percent * 60;

            m_cooldownOverlay.style.height = new Length(remainingHeight, LengthUnit.Pixel);
            m_cooldownOverlay.style.top = new Length(60 - remainingHeight, LengthUnit.Pixel);

            if (m_progress <= 0)
            {
                m_cooldownOverlay.style.height = new Length(0, LengthUnit.Pixel);
                m_cooldownOverlay.style.top = new Length(60, LengthUnit.Pixel);
                m_cooldownOverlay.style.display = DisplayStyle.None;
            }
            else
            {
                m_cooldownOverlay.style.display = DisplayStyle.Flex;
            }
        }
    }
}
