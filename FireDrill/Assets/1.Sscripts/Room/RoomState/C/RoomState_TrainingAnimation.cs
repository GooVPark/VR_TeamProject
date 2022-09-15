using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_TrainingAnimation : RoomState
{
    public RoomState_StartTraining roomStateStartTraining;
    public TrainingManager trainingManager;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if(user.hasExtingisher)
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
