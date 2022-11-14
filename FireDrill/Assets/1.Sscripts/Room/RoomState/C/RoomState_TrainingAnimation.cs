using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_TrainingAnimation : RoomState
{
    public RoomState_StartTraining roomStateStartTraining;
    public TrainingManager trainingManager;

    [Header("NPC")]
    public GameObject npcObject;
    public Animator npcAnimator;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        npcAnimator.SetInteger("AnimationState", 2);

        if (user.hasExtingisher)
        {
            trainingManager.SpawnFire();
        }
        roomSceneManager.RoomState = roomStateStartTraining;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
