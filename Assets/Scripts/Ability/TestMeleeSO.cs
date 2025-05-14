using UnityEngine;

[CreateAssetMenu(fileName = "TestMeleeAbility", menuName = "Ability/TestMeleeAbility")]
public class TestMeleeSO : AbilitySO
{
    public override void Use(PlayerCombat pc, ulong user)
    {
        pc.TriggerAnimation("MeleeAttack");

        Vector2 origin = pc.transform.position;
        Vector2 direction = pc.transform.right;
        float radius = 1.5f;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin + direction, radius, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            hit.collider.TryGetComponent<Healthbar_Network>(out Healthbar_Network hb);

            if (hb != null)
            {
                hb.TakeDamage(5f, user);
            }
        }
    }
}