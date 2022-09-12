using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/*
 * ȭ���� �����ϴ� �ܰ�
 * 
 * ȭ�� ���� ������Ʈ�� Ư�� ��ġ�� 0�̵Ǹ� ���� �ܰ�� �Ѿ����
 * �޾ƿ��°� ��ġ�� ȭ�� ������Ʈ���� ����ȭ �ϰ� �޾ƿ� ��ġ�� uiǥ�ø�
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

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        trainingManager = FindObjectOfType<TrainingManager>();
        trainingManager.GetTrainingProgress += GetProgress;

        progressUIObject.SetActive(true);
    }

    public override void OnStateExit()
    {
        trainingManager.GetTrainingProgress -= GetProgress;
        progressUIObject.SetActive(false);
        base.OnStateExit();
    }

    public void GetProgress(float progress)
    {
        progressUI.fillAmount = progress;
    }
}
