using UnityEngine;

[CreateAssetMenu(fileName = "DotEffect", menuName = "Ability/DotEffect")]
public class DotEffect : StatusEffectSO
{
    public int DamagePerTick;
    public float Tick;

    public override TimedEffect Init(GameObject obj)
    {
        return new TimedDotEffect(this, obj);
    }
}
