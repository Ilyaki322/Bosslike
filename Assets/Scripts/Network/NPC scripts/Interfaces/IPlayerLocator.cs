using System.Collections.Generic;
using UnityEngine;

public interface IPlayerLocator
{
    IReadOnlyList<Transform> GetPlayers();
}
