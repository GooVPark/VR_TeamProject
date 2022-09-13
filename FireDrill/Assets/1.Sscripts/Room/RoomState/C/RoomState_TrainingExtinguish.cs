using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

/*
 * 화제를 진압하는 단계
 * 
 * 화제 관련 오브젝트의 특정 수치가 0이되면 다음 단계로 넘어가도록
 * 받아오는건 수치는 화제 오브젝트에서 동기화 하고 받아온 수치로 ui표시만
 */

public class RoomState_TrainingExtinguish : RoomState
{
    [Header("Room State")]
    public RoomState_EndTraining roomStateEndTraining;
    [Space(5)]

    [Header("UI")]
    public Image progressUI;
    public GameObject progressUIObject;
    [Space(5)]

    [Header("Training Manager")]
    public TrainingManager trainingManager;
    [Header("Controller")]
    public XRDirectInteractor controller;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        trainingManager = FindObjectOfType<TrainingManager>();
        trainingManager.GetTrainingProgress += GetProgress;

        //progressUIObject.SetActive(true);

        if(user.hasExtingisher)
        { 
            InputManager.rightTriggerButton += OnExtinguisher;
        }
    }

    public override void OnStateExit()
    {
        if(user.hasExtingisher)
        {
            roomSceneManager.player.hose.SetActive(false);
            roomSceneManager.player.extinguisher.SetActive(false);
            InputManager.rightTriggerButton -= OnExtinguisher;
        }
        trainingManager.GetTrainingProgress -= GetProgress;
        progressUIObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        
    }

    public void GetProgress(float progress)
    {
        progressUI.fillAmount = progress;
    }

    public void OnExtinguisher(bool value)
    {
        Debug.Log("Spread");
        roomSceneManager.player.Spread(value);
    }
}
