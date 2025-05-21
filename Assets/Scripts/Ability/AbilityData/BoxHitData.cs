using System;
using UnityEngine;

[Serializable]
public class BoxHitData : AbilityData
{
    [field: SerializeField] public Rect m_hitbox;
    [field: SerializeField] public LayerMask m_hitLayerMask;
    [field: SerializeField] public bool m_debug;

    public override Type getFunction()
    {
        return typeof(BoxHitFunction);
    }
}
