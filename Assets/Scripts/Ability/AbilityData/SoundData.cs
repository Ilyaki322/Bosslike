using System;
using UnityEngine;

[Serializable]
public class SoundData : AbilityData
{
    [field: SerializeField] public AudioClip Clip { get; private set; }
    [field: SerializeField] public bool Multiplayer { get; private set; }

    public override Type getFunction()
    {
        return typeof(SoundFunction);
    }
}
