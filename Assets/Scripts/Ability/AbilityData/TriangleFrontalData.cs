using System;
using UnityEngine;

[Serializable]
public class TriangleFrontalData : AbilityData
{
    [field: SerializeField] public float ChargeTime { get; private set; }
    [field: SerializeField] public float Distance { get; private set; }
    [field: SerializeField] public float Fov { get; private set; }
    [field: SerializeField] public Material Mat { get; private set; }
    [field: SerializeField] public LayerMask HitLayerMask { get; private set; }

    public override Type getFunction() => typeof(TriangleFrontalFunction);
}
