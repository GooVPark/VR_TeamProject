using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationController : MonoBehaviour
{
    private enum Hand { Right, Left };
    [SerializeField] private Hand hand;
    public Animator animator;

    public int poseIndex = 0;
    public float flex;
    public float point;
    public float triggerTouched;
    public float thumbsUp;
    
    private void Start()
    {
        switch (hand)
        {
            case Hand.Right:

                InputManager.onRightTriggerValue += GetPoint;
                InputManager.onRightThumbsValue += GetThumsUp;
                InputManager.onRightGrabValue += GetFlex;
                InputManager.onRightTriggerTouched += GetTriggerTouched;

                break;
            case Hand.Left:

                InputManager.onLeftTriggerValue += GetPoint;
                InputManager.onLeftThumbsValue += GetThumsUp;
                InputManager.onLeftGrabValue += GetFlex;
                InputManager.onLeftTriggerTouched += GetTriggerTouched;

                break;
        }
    }

    private void Update()
    {
        if(animator.GetInteger("Pose") == 0)
        {
            animator.SetFloat("Flex", flex);
            animator.SetLayerWeight(2, triggerTouched == 0 ? 0 : point);
            animator.SetLayerWeight(1, thumbsUp);
        }
    }

    public void GetFlex(float value)
    {
        flex = value;
    }

    public void GetPoint(float value)
    {
        point = value;
    }

    public void GetThumsUp(float value)
    {
        thumbsUp = value;
    }

    public void GetTriggerTouched(float value)
    {
        triggerTouched = value;
    }

    public void SetHandPose(int poseIndex)
    {
        Debug.Log("Set Hand Pose: " + poseIndex);
        animator.SetLayerWeight(2, 0);
        animator.SetLayerWeight(1, 0);

        this.poseIndex = poseIndex;
        animator.SetInteger("Pose", poseIndex);
    }
}
