using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Ability/BaseProjectile")]
public class ProjectileDataSO : ScriptableObject
{
    public GameObject Prefab;

    [Space(10)]
    public int Damage;
    public float Speed;

    [Space(10)] public bool HasLifetime = false;
    public float LifeTime = -1;
}
