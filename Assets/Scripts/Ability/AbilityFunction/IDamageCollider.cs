using System;
using UnityEngine;

public interface IDamageCollider
{
    public event Action<Collider2D[]> OnDetected;
}
