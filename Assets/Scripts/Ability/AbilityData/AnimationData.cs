using System;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class AnimationData : AbilityData
{
    [field: SerializeField] public RuntimeAnimatorController Controller;
    //[field: SerializeField] public AnimatorController Controller;
    public override Type getFunction() => typeof(AnimationFunction);
}
