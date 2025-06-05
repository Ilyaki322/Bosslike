using System;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class AnimationData : AbilityData
{
    [field: SerializeField, Range(1,4)] public int Animation;
    public override Type getFunction() => typeof(AnimationFunction);
}
