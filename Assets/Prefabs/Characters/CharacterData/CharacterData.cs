using System;
using UnityEngine;


[Serializable]
public class CharacterData
{
    public int Id; // unique index
    public string Name;
    public Sprite Icon;         // for UI
    public GameObject Prefab;   // in-game avatar
}

