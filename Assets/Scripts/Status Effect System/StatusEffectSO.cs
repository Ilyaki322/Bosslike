using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffectSO : ScriptableObject
{
    public float Duration;
    public bool IsDurationStacked;
    public bool IsEffectStacked;
    public abstract TimedEffect Init(GameObject obj);
}
