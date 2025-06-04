using System;
using UnityEngine;

public class AnimationAnnouncer : MonoBehaviour
{
    public event Action OnFinish;
    public event Action OnAttackAction;

    private void AnimationFinished() => OnFinish?.Invoke();
    private void AttackAction() => OnAttackAction?.Invoke();
}
