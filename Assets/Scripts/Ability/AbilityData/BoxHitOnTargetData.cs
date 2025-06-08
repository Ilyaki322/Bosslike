using System;
using UnityEngine;

[Serializable]
public class BoxHitOnTargetData : AbilityData
{
    [field: SerializeField] public Rect HitBox;
    [field: SerializeField] public LayerMask LayerM;
    [field: SerializeField] public bool Debug;
    [field: SerializeField] public float Range;

    public override Type getFunction()
    {
        return typeof(BoxHitOnTargetFunction);
    }
}
