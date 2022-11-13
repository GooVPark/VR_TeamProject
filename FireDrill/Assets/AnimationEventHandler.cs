using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public delegate void AnimationEndEvent();
    public AnimationEndEvent animationEndEvent;

    public void OnAnimationEnd()
    {
        animationEndEvent?.Invoke();
    }
}
